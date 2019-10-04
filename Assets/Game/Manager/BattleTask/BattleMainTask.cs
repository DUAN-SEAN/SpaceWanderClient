using Crazy.Main;
using System;
using UnityEngine;
using Crazy.ClientNet;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Game.Manager.BattleManager;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using Assets.Game.Manager.BattleTask.Controller;
using Box2DSharp.External;
using Cinemachine;
using UnityEngine.SceneManagement;
using GameActorLogic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Vector2 = System.Numerics.Vector2;
using Vector3 = UnityEngine.Vector3;
using Box2DSharp;
using Box2DSharp.Inspection;

namespace Assets.Game.Manager.BattleTask
{
    internal class PredictInfo
    {
        /// <summary>
        /// 预测坐标
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 预测朝向
        /// </summary>
        public float forwoardAngle;
        /// <summary>
        /// 预测速度
        /// </summary>
        public Vector2 velocity;
        /// <summary>
        /// 预测角速度
        /// </summary>
        public float angularVelocity;
        /// <summary>
        /// 预测合力
        /// </summary>
        public Vector2 force;
        /// <summary>
        /// 预测时间段
        /// </summary>
        public float predictDampTime;
    }
    public class BattleMainTask : ITaskEventSystem
    {
        /// <summary>
        /// 玩家现场信息
        /// </summary>
        private SpacePlayerContext _plx;
        /// <summary>
        /// 战斗管理器
        /// </summary>
        private BattleManager _bm;
        /// <summary>
        /// 关卡信息
        /// </summary>
        private ClientLevel level;
        /// <summary>
        /// 
        /// </summary>
        public static bool IsOnReadyBattle = false;
        private CinemachineCameraController m_cinemachineCameraController;
        /// <summary>
        /// actor对应的Unity世界物体
        /// </summary>
        private Dictionary<ulong, GameObject> m_actorDic = new Dictionary<ulong, GameObject>();
        private Dictionary<ulong, GameObject> m_actorRealDataDic = new Dictionary<ulong, GameObject>();
        private Dictionary<ulong, GameObject> m_actorPredictDic = new Dictionary<ulong, GameObject>();
        private Queue<ulong> m_DestoryQue = new Queue<ulong>();
        /// <summary>
        /// 保存预测信息
        /// </summary>
        private Dictionary<ulong, PredictInfo> m_predictInfoDic = new Dictionary<ulong, PredictInfo>();
        private float tickTime = 0f;
        private float interval = 0.1f;
        private long frame = 0;

        private Stopwatch m_stopWatch = new Stopwatch();
        //private BodyRenderer bodyRenderer;

        private BoxDrawer boxDrawer;
        private BodyRender BodyRender;


        public Dictionary<ulong, GameObject> ActorDic => m_actorDic;
        public BattleMainTask(BattleManager bm, SpacePlayerContext plx)
        {
            _bm = bm;
            _plx = plx;

        }
        public void Init()
        {
         
        }
        public void Start()
        {
            _plx = GameManager.Instance.CurrentPlayerContext;
            m_stopWatch.Start();
            m_actorDic = new Dictionary<ulong, GameObject>();
            m_predictInfoDic = new Dictionary<ulong, PredictInfo>();
            m_DestoryQue = new Queue<ulong>();
            frame = 0;
            tickTime = 0f;
        }
        
        public void Update()
        {
            //Debug.Log("帧速度 = "+Time.deltaTime);
            try
            {
                if (!IsOnReadyBattle) return;


                TickLevel();//驱动Level执行逻辑

                TickDelay();//驱动延迟

                TickCheck();//驱动检测

                TickMove();//驱动移动转向

                //TickGPS();//驱动Gps


            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            //TODO 朱颖的BodyRender
            //if (BodyRender == null)
            //    if (GameObject.FindWithTag("SpaceCamera") != null)
            //    {
            //        boxDrawer = new BoxDrawer();
            //        boxDrawer.Drawer = UnityDrawer.GetDrawer();
            //        BodyRender = new BodyRender(boxDrawer);
            //    }

            //BodyRender?.DrawBody(level.GetAllActors().ToBodyList());






        }

        private void TickCheck()
        {
            if (m_DestoryQue != null && m_DestoryQue.Count > 0)
            {
                while (m_DestoryQue.Count > 0)
                {
                    var actorId = m_DestoryQue.Dequeue();
                    
                    if (m_actorDic.TryGetValue(actorId, out var actorGameObject))
                    {
                        m_actorDic.Remove(actorId);
                        GameObject.Destroy(actorGameObject);
                    }
                    
                }
            }


            var array = m_actorDic.Keys.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                var key = array[i];
                if (_plx.GetActorBase(key)==null || !m_actorDic.TryGetValue(key,out var value))
                {
                    Debug.Log("检查到物体为空，字典清除 actorId = "+key);
                    m_actorDic.Remove(key);
                }
                else
                {
                    //todo 做某些事情
                }
            }
        }

        private void TickGPS()
        {
            var playerActor = level.GetPlayerActorByString(_plx.PlayerId);
            if (playerActor == null) return;

            if (!m_actorDic.TryGetValue(playerActor.GetActorID(), out var player))
            {
                return;
            }

            GPSController gpsController = player?.GetComponent<GPSController>();
            if (gpsController == null)
                return;
            var positions = level.GetShipActors();

            List<Vector3> posList = new List<Vector3>();
            foreach (var actor in positions)
            {
                posList.Add(BattleHelper.Point2Vector3(actor.GetPosition()));
            }
            gpsController.CheckPosition(posList);


        }

        /// <summary>
        /// 更新显示层玩家位置
        /// </summary>
        private void TickMove()
        {
            
            m_stopWatch.Restart();//计算执行本方法的延迟
            GameObject gameObject = null;
            tickTime += Time.deltaTime;
            var serverFrame = level.GetCurrentFrame();
            if (frame == serverFrame)
            {
                //测延迟
                if (tickTime > 0.2f)//true 延迟大于200ms 进行预测
                {
                    Log.Info("服务器未发来新数据 帧率+延迟= " + tickTime);
                    foreach (var key in m_actorDic.Keys)
                    {
                        var actor = _plx.GetActorBase(key);
                        if (actor == null) continue;
                        if (!m_actorDic.TryGetValue(key, out gameObject) || gameObject == null)
                        {
                            Debug.Log("失去GameObject" + key);
                            continue;
                        }
                        var moveController = gameObject?.GetComponent<MoveController>();
                        if (moveController == null)
                            continue;
                        moveController.Clear();//清空一次队列

                        DoPredict(actor, moveController, tickTime);
                    }
                }
                else//大部分帧都落入这里
                {
                    m_stopWatch.Stop();
                    //需要加上处理间隔
                    tickTime  += m_stopWatch.ElapsedTicks/10000000f;
                    //Debug.Log(m_stopWatch.ElapsedTicks);
                    return;
                }

            }
            else//新的一帧位置，TickTime为200ms内的一个值
            {
                frame = serverFrame;//更新当前数据
                                    //Debug.Log("serverFrame:" + frame);
                foreach (var key in m_actorDic.Keys)
                {
                    if (!m_actorDic.TryGetValue(key, out gameObject)||gameObject == null)
                    {
                        Debug.Log("失去GameObject" + key);
                        continue;
                    }
                    var actor = _plx.GetActorBase(key);
                    if (actor == null)
                        continue;
                    var moveController = gameObject?.GetComponent<MoveController>();
                    if (moveController == null)
                        continue;
                    //Debug.Log("下一帧 Tick "+ tickTime);
                    DoPredict(actor, moveController,tickTime);


                    //{ //预测飞船 直接赋值
                    //    moveController = null;
                    //    if (m_actorPredictDic.TryGetValue(actor_gameobj.Key, out var preGo))
                    //    {
                    //        moveController = preGo.GetComponent<MoveController>();
                    //        if (moveController == null) return;
                    //        moveController.transform.position =
                    //            BattleHelper.Point2Vector3(prePos);
                    //        moveController.transform.forward = BattleHelper.Point2Vector3(preforward);
                    //    }

                    //}


                    //{ //真实飞船 直接赋值
                    //    moveController = null;
                    //    if (m_actorRealDataDic.TryGetValue(actor_gameobj.Key, out var preGo))
                    //    {
                    //        moveController = preGo.GetComponent<MoveController>();
                    //        if (moveController == null) return;
                    //        moveController.transform.position =
                    //            BattleHelper.Point2Vector3(actor.GetPosition());
                    //        moveController.transform.forward = BattleHelper.Point2Vector3(position: actor.GetForward());
                    //    }

                    //}

                }
            }


            m_stopWatch.Stop();
            //tickTime = (endTime - beginTime) <10000L?0f:(endTime-beginTime)/10000000f;
            tickTime = (float)m_stopWatch.ElapsedTicks / 10000000f;
            //Debug.Log("TickMove逻辑延迟 = "+tickTime);
        }

        private void DoPredict(ActorBase actor, MoveController moveController,float tickTime)
        {
            var preTime = tickTime;
            //Debug.Log("预测参数 速度："+actor.GetVelocity()+"受力："+actor.GetForce()+"质量："+actor.GetBodyMassByActor());
            PredictInfo predictInfo = m_predictInfoDic[actor.GetActorID()];
            if (predictInfo == null) predictInfo = new PredictInfo();
            predictInfo.position = actor.GetPosition();
            predictInfo.forwoardAngle = actor.GetForwardAngle();
            predictInfo.velocity = actor.GetVelocity();
            predictInfo.force = actor.GetForce();
            predictInfo.predictDampTime = preTime;
            predictInfo.angularVelocity = actor.GetAngleVelocity();

            var prePos = PredictPosition(ref predictInfo.position, ref predictInfo.velocity, predictInfo.force, preTime,
                1f / actor.GetBodyMassByActor(), LinearDamping: actor.GetLinerDamping());
            var preforward = PredictForward(ref predictInfo.forwoardAngle, ref predictInfo.angularVelocity, preTime);

            moveController.AddDumpInfo(new DumpInfo
            {
                Dump = preTime, Position = BattleHelper.Point2Vector3(prePos), Forward = BattleHelper.Point2Vector3(preforward)
            });
        }


        /// <summary>
        /// 更新level
        /// </summary>
        private void TickLevel()
        {
            if (_plx.GetCurrentLevel() != null)
                _plx.GetCurrentLevel().Update();
            else
            {
                Log.Info("GetCurrentLevel NULL");
            }
        }
        /// <summary>
        /// 更新延迟信息
        /// </summary>
        private void TickDelay()
        {
            _plx.CallDelayReq();
        }
        public void Dispose()
        {
 
            ReleaseBattleEvent();
        }

   
        private void BindBattleEvent()
        {
            //Log.Debug("bind 成功");
            _plx.OnInitActorMessageEvent += OnInitActorMessage;
            _plx.OnDestroyMessageEvent += OnDestroyMessage;
        }
        private void ReleaseBattleEvent()
        {
            _plx.OnInitActorMessageEvent -= OnInitActorMessage;
            _plx.OnDestroyMessageEvent -= OnDestroyMessage;
        }
        public void OnCreateBattle()
        {
            BindBattleEvent();
            //段瑞加
        }
        
        public void InitCamera()
        {
            m_cinemachineCameraController = GameObject.FindWithTag("SpaceCamera").GetComponent<CinemachineCameraController>();
            m_cinemachineCameraController.CreateCinemachineVircualCamera();
        }
        public void Stop()
        {
            IsOnReadyBattle = false;

            ReleaseBattleEvent();
        }
        private void OnCloseBattle()
        {
            IsOnReadyBattle = false;
            ReleaseBattleEvent();
            Log.Info("￥￥￥￥￥CloseBattle");
        }
        public void OnReadyBattle()
        {
            Log.Info("BattleTask::收到了ReadyBattle");
            level = _plx.GetCurrentLevel();
            _bm.UIInBattleTask.setState(true);
            IsOnReadyBattle = true;
            Log.Info("￥￥￥￥￥￥￥￥￥######ReadyBattle");

        }
        private void OnInitActorMessage(ulong actorId)
        {
            if (!IsOnReadyBattle) return;
            //Debug.Log("ActorId:" + actorId);
            ActorBase actor = _plx.GetActorBase(actorId);
            //Debug.Log("ActorType:" + actor.GetActorType());
            GameObject gameObject = LoadFromActorType(actor.GetActorType(), actor);
            if (gameObject == null) return;
            gameObject.AddComponent<MoveController>();
            m_actorDic.Add(actorId, gameObject);
            m_predictInfoDic.Add(actorId,new PredictInfo
            {
                position = actor.GetPosition(),
                forwoardAngle =  actor.GetForwardAngle(),
                velocity = actor.GetVelocity(),
                angularVelocity = actor.GetAngleVelocity(),
                force = actor.GetForce(),
                predictDampTime = 0f
            });


            //GameObject gameObjectReal = LoadFromActorType(actor.GetActorType(), actor,true);
            //if (gameObjectReal == null) return;
            //gameObjectReal.AddComponent<MoveController>();
            //m_actorRealDataDic.Add(actorId, gameObjectReal);



            //GameObject gameObjectPre = LoadFromActorType(actor.GetActorType(), actor,true);
            //if (gameObjectPre == null) return;
            //gameObjectPre.AddComponent<MoveController>();
            //m_actorPredictDic.Add(actorId,gameObjectPre);
        }
        private void OnDestroyMessage(ulong actorId)
        {
            if (!IsOnReadyBattle) return;
            ActorBase tempactor = _plx.GetActorBase(actorId);
            if (tempactor != null)
            {
                GameObject gameObject = DestroyFromActorType(tempactor.GetActorType(), tempactor);
            }
            //Debug.Log("OnDestroyMessage:" + actorId);
            m_DestoryQue.Enqueue(actorId);
        }
        private GameObject DestroyFromActorType(int getActorType, ActorBase actor)
        {
            GameObject actorGameObject = null;
            if (actor.IsWeapon())
            {
                //Log.Info("武器爆炸生成");
                actorGameObject = BattleLoadingResource.LoadingResource(actor, getActorType, true);
            }
            else
            {
                //Log.Info("飞船爆炸生成");
                actorGameObject = BattleLoadingResource.LoadingResource(actor, getActorType, true);          
            }
            actorGameObject.transform.position = BattleHelper.Point2Vector3(position: _plx.GetActorBase(actor.GetActorID()).GetPosition());
            actorGameObject.transform.forward = BattleHelper.Point2Vector3(((IBaseComponentContainer)actor).GetPhysicalinternalBase().GetBody().GetForward());
            return actorGameObject;
        }
        /// <summary>
        /// 根据actorType生成
        /// </summary>
        /// <param name="getActorType"></param>
        /// <param name="actor" />
        /// <returns></returns>
        private GameObject LoadFromActorType(int getActorType, ActorBase actor,bool isPre = false)
        {
            GameObject actorGameObject = null;
            if (isPre) return null;
            if (actor.IsEnvir())
            {
                actorGameObject = BattleLoadingResource.LoadingResource(actor,getActorType, false);
                actorGameObject.transform.position = BattleHelper.Point2Vector3(position: _plx.GetActorBase(actor.GetActorID()).GetPosition());
                actorGameObject.transform.forward = BattleHelper.Point2Vector3(((IBaseComponentContainer)actor).GetPhysicalinternalBase().GetBody().GetForward());
            }
            else if (actor.IsShip())
            {
                Log.Info("飞船生成");
                actorGameObject = BattleLoadingResource.LoadingResource(actor, getActorType, false);
                //设置信息
                actorGameObject.transform.position = BattleHelper.Point2Vector3(position: _plx.GetActorBase(actor.GetActorID()).GetPosition());
                actorGameObject.transform.forward = BattleHelper.Point2Vector3(((IBaseComponentContainer)actor).GetPhysicalinternalBase().GetBody().GetForward());
                if (actor.IsPlayer())
                {
                    if (!isPre)
                    {
                        //actorGameObj.name =  actor.GetActorName();
                        actorGameObject.name = "Player(" + actor.GetActorName() + ")";
                        if (actor.GetActorName() == _plx.PlayerId)
                        {
                            //Debug.Log("找到玩家自身飞船，开启摄像机跟随");
                            if (m_cinemachineCameraController == null)
                            {
                                //Log.Info("m_cinemachineCameraController is Null");
                                m_cinemachineCameraController = GameObject.Find("Main Camera").GetComponent<CinemachineCameraController>();
                            }
                            m_cinemachineCameraController.BindCamera(actorGameObject.name);
                        }
                        actorGameObject.AddComponent<GPSController>();
                    }
                    else
                    {
                        actorGameObject.name = "Player(" + actor.GetActorName() + ")真实飞船";
                    }
                }
                else
                {
                    if (!isPre)
                    {
                        actorGameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>(BattleResourceDefine.Ship_02_A_Path);
                        actorGameObject.name = "Enemy(" + actor.GetActorID() + ")";
                    }
                    else
                    {
                        actorGameObject.name = "Enemy(" + actor.GetActorID() + ")真实飞船";
                    }
                }
                return actorGameObject;
            }
            //判断玩家、队友、敌人             
            else if (actor.IsWeapon())
            {
                //Log.Info("武器生成");
                actorGameObject = BattleLoadingResource.LoadingResource(actor, getActorType,false) ;
                WeaponActorBase weaponActorBase = actor as WeaponActorBase;
                if (weaponActorBase == null)
                {
                    Debug.Log("weaponActorBase is null");
                }
                ActorBase launchActor = level.GetActor(weaponActorBase.GetOwnerID());
                if (launchActor == null)
                {
                    Debug.Log("launchActor is null" + weaponActorBase.GetOwnerID());
                }

                var onwerActor = level.GetActor(weaponActorBase.GetOwnerID());
                Vector3 prePosition = BattleHelper.Point2Vector3(_plx.GetActorBase(onwerActor.GetActorID()).GetPosition());
                Vector3 preForward = BattleHelper.Point2Vector3(((IBaseComponentContainer)onwerActor).GetPhysicalinternalBase().GetBody()
                        .GetForward());
                Vector2 v;
                if (weaponActorBase.GetActorType() == ActorTypeBaseDefine.ContinuousLaserActor)
                {
                    v = ActorHelper.GetLaserShapeByShip(onwerActor.GetActorType());
                }

                actorGameObject.transform.position = prePosition;
                actorGameObject.transform.forward = preForward;
                actorGameObject.transform.localScale = new Vector3(5, 5, 5);
                return actorGameObject;
            }
            else
            {
                Debug.Log("Actor is not Palyer、Weapon、Ship or Environment");
            }
            return actorGameObject;
        }

        

        /// <summary>
        /// 位置预测函数
        /// 航位推测法
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="Force"></param>
        /// <param name="time"></param>
        /// <param name="invMass"></param>
        /// <param name="stepTime"></param>
        /// <param name="LinearDamping"></param>
        /// <returns></returns>
        public static Vector2 PredictPosition(ref Vector2 position,ref Vector2 velocity, Vector2 Force, float time, float invMass = 1/75f, float stepTime = 1 / 60f, float LinearDamping = 0.0f)
        {
            do
            {
                velocity += stepTime * (invMass * Force);//速度增加
                velocity *= 1.0f / (1.0f + stepTime * LinearDamping);//阻尼效应

                position +=  stepTime * velocity;//积分

                time -= stepTime;
            } while (time - stepTime > float.Epsilon);

            position += velocity * (stepTime - time);//补上积分

            return position;
        }
        public static Vector2 PredictForward(ref float forward,ref float velocity,float time, float stepTime = 1 / 60f, float AngularDamping = 0.0f)
        {
            do
            {

                forward *= 1.0f / (1.0f + stepTime * AngularDamping);//阻尼效应


                forward += stepTime * velocity;//积分
                time -= stepTime;
            } while (time - stepTime > float.Epsilon);

            forward += velocity * (stepTime - time);
            return new Vector2((float)-Math.Sin(forward), (float)Math.Cos(forward));
        }

    }
}
