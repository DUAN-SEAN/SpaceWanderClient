using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Assets.Game;
using Assets.Game.Actor;
using Crazy.Common;
using GameActorLogic;
using Google.Protobuf;

namespace Crazy.Main
{
    /// <summary>
    /// 为战斗界面提供的逻辑块
    /// </summary>
    public interface IBattleUILogicBlockHandler
    {
        /// <summary>
        /// 获取关卡描述实体
        /// 描述为最大的战斗实体，包含了关卡所含的一切。
        /// ps:如果未提供充足的接口，可以直接从这里获取整个关卡信息
        /// </summary>
        /// <returns></returns>
        ClientLevel GetCurrentLevel();

        /// <summary>
        /// 获取actorId指定的飞船
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        ShipActorBase GetShipActorBase(ulong actorId);

        /// <summary>
        /// 获取整个队伍的actor
        /// </summary>
        /// <returns></returns>
        List<ActorBase> GetTeamActors();

        /// <summary>
        /// 获取玩家当前的飞船actor
        /// </summary>
        /// <returns></returns>
        ShipActorBase GetOnwerShipActor();
        /// <summary>
        /// 获取指定玩家的飞船
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        ShipActorBase GetPlayerShipActor(string playerId);
        /// <summary>
        /// 所有
        /// </summary>
        /// <returns></returns>
        List<ActorBase> GetAIActors();


        /// <summary>
        /// 获取当前任务
        /// 任务描述、进度
        /// </summary>
        /// <returns></returns>
        List<ITaskEvent> GetCurrentTask();
        /// <summary>
        /// 获得所有当前技能
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 获得自己的飞船技能
        /// </summary>
        /// <returns></returns>
        List<ISkillContainer> GetAllCurrentSkill();
        /// <summary>
        /// 获取关卡Id
        /// </summary>
        /// <returns></returns>
        int GetBarrierId();
        /// <summary>
        /// 获取战斗关卡中队伍玩家集合
        /// </summary>
        /// <returns></returns>
        List<string> GetBattlePlayers();

        #region BattleUIReq
        /// <summary>
        /// 发送战斗指令
        /// </summary>
        /// <param name="command"></param>
        void SendCommandReq(ICommand command);
        /// <summary>
        /// 发送战斗指令集
        /// </summary>
        /// <param name="commands"></param>
        void SendCommandReq(IList<ICommand> commands);
        /// <summary>
        /// 客户端关卡加载完毕后发送给服务器
        /// </summary>
        void SendReadyBattleReq();
        /// <summary>
        /// 发送退出战斗请求
        /// </summary>
        void SendExitBattleReq();

        void SendAudioToBattle(AudioBlock audioBlock);

        event Action<object> OnFinishBattleEvent;

        event Action<string, AudioBlock> OnAudioBattleEvent;



        #endregion
    }
    /// <summary>
    /// 为战斗场景提供的逻辑块
    /// </summary>
    public interface IBattleLogicBlockHandler
    {

        /// <summary>
        /// 获取关卡描述实体
        /// 描述为最大的战斗实体，包含了关卡所含的一切。
        /// ps:如果未提供充足的接口，可以直接从这里获取整个关卡信息
        /// </summary>
        /// <returns></returns>
        ClientLevel GetCurrentLevel();
        /// <summary>
        /// 获取actorId指定的飞船
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        ShipActorBase GetShipActorBase(ulong actorId);
        /// <summary>
        /// 获取玩家当前的飞船actor
        /// </summary>
        /// <returns></returns>
        ShipActorBase GetOnwerShipActor();
        /// <summary>
        /// 获取指定玩家的飞船
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        ShipActorBase GetPlayerShipActor(string playerId);

        /// <summary>
        /// 获取服务器逻辑频率
        /// </summary>
        /// <returns></returns>
        long GetServerLogicalInterval();
        /// <summary>
        /// 获取服务器延迟
        /// </summary>
        /// <returns></returns>
        long GetServerDelayTime();

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        DateTime GetServerTime();
        /// <summary>
        /// 发送资源确认准备
        /// </summary>
        void SendReadyBattleReq();
        /// <summary>
        /// 测试战斗网络延迟
        /// </summary>
        void CallDelayReq();

    }
    /// <summary>
    /// 描述战斗的玩家现场
    /// </summary>
    public partial class SpacePlayerContext:IBattleLogicBlockHandler,IBattleUILogicBlockHandler
    {

        


        #region Handler



        /// <summary>
        /// 生成一场战斗，从服务器获取数据，加载客户端数据层
        /// </summary>
        public void OnCreateBattle(ulong battleId, IList<string> players)
        {
            try
            {
                Crazy.ClientNet.Log.Info("OnCreateBattle");
                m_currentBattleId = battleId;
                m_CurrentLevel = new ClientLevel();
                m_CurrentLevel.InitConfig(m_serverGlobalConfig.BarrierConfigs[0], m_serverGlobalConfig.GameShipConfig.ToList(), m_serverGlobalConfig.GameSkillConfig);

                m_currentBattlePlayers = new List<string>();
                foreach (var playerId in players)
                {
                    m_currentBattlePlayers.Add(playerId);
                }

                //1 通知上层
                GetCurrentShipACK();//todo 暂时写这里
                OnCreateBattleEvent?.Invoke();
            }
            catch(Exception e)
            {
                Crazy.ClientNet.Log.Error(e);
            }

           

        }
        /// <summary>
        /// 退出战斗的应答消息，可能成功可能失败，可以收到战斗中其他玩家退出的请求状态
        /// </summary>
        /// <param name="state">1=OK:0=Fail</param>
        /// <param name="playerId"></param>
        public void OnExitBattle(int state,string playerId)
        {
            if(state == 1)
            {
                if (playerId == m_playerId)
                {
                    OnCloseBattle();
                }
                else
                {
                    //1 清理玩家在客户端实体的存在
                    m_currentMatchTeam.Remove(playerId);

                    OnExitBattleEvent?.Invoke(playerId);

                }
            }
            else
            {
                //退出失败 
            }
        }
        /// <summary>
        /// 关闭一场战斗，从服务器得到的关闭战斗
        /// </summary>
        public void OnCloseBattle()
        {
            m_currentBattlePlayers.Clear();
            m_currentBattlePlayers = null;
            m_CurrentLevel?.Dispose();

     
            //1 通知上层
            OnCloseBattleEvent?.Invoke();

            m_currentBattleId = default;
            m_CurrentLevel = null;

        }
        /// <summary>
        /// 从网络来了一个事件处理
        /// todo:吴悠维护
        /// </summary>
        /// <param name="e"></param>
        public void OnEventHandle(IEventMessage e)
        {
            //Crazy.ClientNet.Log.Info("收到战斗事件消息"+e.MessageId);
            
            if(e==null) Log.Info("e为null");
            switch (e.MessageId)
            {
                case EventMessageConstDefine.BattleEventNone:
                    break;
                case EventMessageConstDefine.InitEvent:
                    var init = (InitEventMessage) e;
                    //ClientNet.Log.Info("init message:"+init.point_x + " " + init.point_y + init.angle);
                    m_CurrentLevel.AddEventMessagesToHandlerForward(e);
                    break;
                case EventMessageConstDefine.UpdateEvent:
                    break;
                case EventMessageConstDefine.DestroyEvent:
                    m_CurrentLevel.AddEventMessagesToHandlerForward(e);
                    break;
            }
        }

        /// <summary>
        /// 服务器完成初始化且所有客户端均加载完毕
        /// </summary>
        public void OnReadyBattleAck()
        {
            try
            {
                m_CurrentLevel.Start();
                Crazy.ClientNet.Log.Info("OnReadyBattleAck::服务器三次握手，开启战斗");
                if (OnReadyBattleEvent == null) Crazy.ClientNet.Log.Info("OnReadyBattleAck::OnReadyBattleEvent is Null");
                //通知上层可以操纵
                OnReadyBattleEvent?.Invoke();
            }catch(Exception e)
            {
                Crazy.ClientNet.Log.Info(e.ToString());
            }
           
        }

        /// <summary>
        /// 获取血量同步飞船的血量值
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="Hp"></param>
        public void OnSyncHealthShield(ulong actorId, int Hp, int shield)
        {
            foreach (var a in m_CurrentLevel.GetAllActors())
            {
                if (a.GetActorID() == actorId)
                {
                    ShipActorBase shipActorBase = a as ShipActorBase;
                    if (shipActorBase != null)
                    {
                        //赋值
                        shipActorBase.SetHP(Hp);
                        shipActorBase.SetShieldNum(shield);
                    }
                }
            }
        }
   
        /// <summary>
        /// 同步Actor实体的物理状态
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="angleVelocity"></param>
        /// <param name="forceX"></param>
        /// <param name="forceY"></param>
        /// <param name="forwardAngle"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="velocityX"></param>
        /// <param name="velocityY"></param>
        /// <param name="torque"></param>
        public void OnSyncPhysicState(ulong actorId, double angleVelocity, double forceX, double forceY, double forwardAngle, double positionX, double positionY, double velocityX, double velocityY, double torque)
        {
            foreach (var a in m_CurrentLevel.GetAllActors())
            {
                if (a.GetActorID() == actorId)
                {
                    ActorBase actorBase = a as ActorBase;
                    if (actorBase != null)
                    {
                        //赋值
                        //actorBase.SetPhysicalValue(actorId,angleVelocity,forceX,forceY,forwardAngle,positionX,positionY,velocityX,velocityY,torque);
                    }
                }
            }
        }
        public void OnSyncPhysicState(ulong actorId, float angleVelocity, float forceX, float forceY, float forwardAngle, float positionX, float positionY, float positionPreX, float positionPreY, float velocityX, float velocityY, float torque)
        {
            foreach (var a in m_CurrentLevel.GetAllActors())
            {
                if (a.GetActorID() == actorId)
                {
                    ActorBase actorBase = a as ActorBase;
                    
                    if (actorBase != null)
                    {
                        //赋值
                        actorBase.SetPhysicalValue(actorId, angleVelocity, forceX, forceY, forwardAngle, positionX, positionY,positionPreX,positionPreY, velocityX, velocityY, torque);
                        //if (actorBase.IsWeapon())
                        //{
                        //    ClientNet.Log.Info(
                        //        actorBase.GetActorID()+"body id"+actorBase.GetBodyId() + "武器传送坐标朝向： 朝向：" + forwardAngle + " 坐标：" + positionX + " " + positionY);

                        //    ClientNet.Log.Info(
                        //        actorBase .GetBodyId()+ "武器坐标朝向： 朝向：" + actorBase.GetForward() + " 坐标：" + actorBase.GetPosition());
                        //}
                    }
                }
            }
        }
        /// <summary>
        /// 同步任务状态
        /// </summary>
        /// <param name="message"></param>
        public void OnSyncLevelTaskState(S2C_SyncLevelTaskBattleMessage message)
        {
           // Crazy.ClientNet.Log.Info("同步任务长度："+message.Tasks.Count.ToString());
            foreach (var messageTask in message.Tasks)
            {
                Dictionary<int,int> taskConditionDic = new Dictionary<int, int>();
                foreach (var mapField in messageTask.Conditions)
                {
                    taskConditionDic.Add(mapField.Key,mapField.Value);
                }
                m_CurrentLevel.SetTaskConditionAndState(messageTask.Id,messageTask.State, taskConditionDic);
            }
            message.Tasks.Clear();
        }

        /// <summary>
        /// 同步技能状态
        /// </summary>
        /// <param name="message"></param>
        public void OnSyncSkillState(S2C_SyncSkillStateBattleMessage message)
        {
            var actorId = message.ActorId;
            var shipActor = m_CurrentLevel.GetActor(actorId) as ShipActorBase;
            if (shipActor == null) return;
            foreach (var skillState in message.Skills)
            {
                shipActor.SetSkillCapNum(skillState.SkillType, skillState.Count);
                shipActor.SetSkillCd(skillState.SkillType, skillState.CD);
            }
            message.Skills.Clear();
        }

        public void OnSyncLevelState(S2C_SyncLevelStateBattleMessage message)
        {
            m_CurrentLevel.SetCurrentFrame(message.Frame);
            var time = message.Time;
            m_serverTime = new DateTime(time);

            m_CurrentLevel.SetDelta(message.IntervalTime);
            //ClientNet.Log.Info("Level DeltaTime = "+m_CurrentLevel.GetDelta());
            //m_ServerDelayTime = DateTime.Now.Ticks - time;
            m_ServerLogicalInterval = message.IntervalTime;
            //ClientNet.Log.Info("网络延时 = "+(DateTime.Now.Ticks-time)/10000+"ms");
            //ClientNet.Log.Info("服务器帧频间隔 = "+message.IntervalTime/10000+"ms");
        }
        /// <summary>
        /// 战斗结束
        /// </summary>
        /// <param name="messageBattleId"></param>
        /// <param name="messageResult"></param>
        public void OnFinishBattle(ulong messageBattleId, ByteString messageResult)
        {
            ClientNet.Log.Info("战斗结束 回调切换场景");
            m_currentBattlePlayers.Clear();
            m_currentBattlePlayers = null;
            //todo 调用回调事件 切换场景 打开关卡选择界面
            OnFinishBattleEvent?.Invoke(messageResult);

        }

        public void OnSpeakInBattle(S2C_SpeakToBattleAck message)
        {
            if (CurrentMatchTeamId != message.BattleId) return;
            using (MemoryStream ms = new MemoryStream(message.Data.ToByteArray()))
            {

                var audioBlock = m_bf.Deserialize(ms) as AudioBlock;

                OnAudioBattleEvent?.Invoke(message.LaunchPlayerId, audioBlock);
            }

        }

        #endregion

        #region 控制层Task关注的
        /// <summary>
        /// 获取当前战斗关卡实体
        /// 关卡实体内部包含了一系列生成的actor，上层task获取actor更新数据
        /// </summary>
        /// <returns></returns>
        public ClientLevel GetCurrentLevel()
        {
            return m_CurrentLevel;
        }

        public ShipActorBase GetShipActorBase(ulong actorId)
        {
            return m_CurrentLevel.GetActor(actorId) as ShipActorBase;
        }

        ShipActorBase IBattleUILogicBlockHandler.GetShipActorBase(ulong actorId)
        {
            return GetShipActorBase(actorId);
        }



        /// <summary>
        /// 获取actorId对应的飞船实体
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>

        public ShipActorBase GetOnwerShipActor()
        {
            ShipActorBase shipActorBase = m_CurrentLevel.GetPlayerActorByString(PlayerId) as ShipActorBase;
            return shipActorBase;
        }

        public ShipActorBase GetPlayerShipActor(string playerId)
        {
            ShipActorBase shipActorBase = m_CurrentLevel.GetPlayerActorByString(playerId) as ShipActorBase;
            return shipActorBase;
        }

        public List<ActorBase> GetAIActors()
        {
            return null;
        }

        /// <summary>
        /// 获取队伍所有飞船
        /// </summary>
        /// <returns></returns>
        public List<ActorBase> GetTeamActors()
        {
            return m_CurrentLevel.GetPlayerActors();
        }
        /// <summary>
        /// TODO:关卡提供接口
        /// </summary>
        /// <returns></returns>
        public ShipActorBase GetOnwerShipActor(string playerId)
        {
            ShipActorBase shipActorBase = m_CurrentLevel.GetPlayerActorByString(playerId) as ShipActorBase;
            return shipActorBase;
        }

        /// <summary>
        /// 获取当前任务
        /// TODO:关卡提供接口
        /// </summary>
        /// <returns></returns>
        public List<ITaskEvent> GetCurrentTask()
        {
            return m_CurrentLevel.GetAllTaskEvents();
        }
        /// <summary>
        /// 获取当前玩家技能状态
        /// </summary>
        /// <returns></returns>
        public List<ISkillContainer> GetAllCurrentSkill()
        {
            ShipActorBase shipActorBase = m_CurrentLevel.GetPlayerActorByString(PlayerId) as ShipActorBase;
            var skill = shipActorBase.GetSkills();
            return skill;
        }

        public int GetBarrierId()
        {
            return m_currentBarrierId;
        }

        public List<string> GetBattlePlayers()
        {
            return CurrentMatchTeam.GetMembers();
        }

        public long GetServerLogicalInterval()
        {
            return m_ServerLogicalInterval;
        }

        public long GetServerDelayTime()
        {
            return m_ServerDelayTime;
        }

        public DateTime GetServerTime()
        {
            return m_serverTime;
        }


        /// <summary>
        /// 获取ActorBase实体
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        public ActorBase GetActorBase(ulong actorId)
        {
            foreach (var a in m_CurrentLevel.GetAllActors())
            {
                if (a.GetActorID() == actorId)
                {
                    return a;
                }
            }

            return null;
        }
        #endregion

        #region IBattleUILogicBlockHandler
        /// <summary>
        /// 控制脚本收集指令后发送给服务器
        /// </summary>
        /// <param name="command"></param>
        public void SendCommandReq(ICommand command)
        {
            (command as Command).currenttime = DateTime.Now;
            using (MemoryStream ms = new MemoryStream())
            {

                m_bf.Serialize(ms, command);
                C2S_CommandBattleMessage commandMsg = new C2S_CommandBattleMessage { BattleId = m_currentBattleId, Command = ByteString.CopyFrom(ms.ToArray()) };
                Send(commandMsg);

            }
        }
        /// <summary>
        /// 发送一个指令集合给服务器
        /// </summary>
        /// <param name="commands"></param>
        public void SendCommandReq(IList<ICommand> commands)
        {
            foreach (var c in commands)
            {
                SendCommandReq(c);
            }
        }

        public void SendReadyBattleReq()
        {
            Send(new C2S_ReadyBattleBarrierReq {BattleId = m_currentBattleId, PlayerId = PlayerId});
        }

        public async void CallDelayReq()
        {
            long preTime = DateTime.Now.Ticks;
            var response = (S2C_DelayAck) await Call(new C2S_DelayReq());
            long intervalTime = DateTime.Now.Ticks - preTime;
            //ClientNet.Log.Info("延迟为:"+intervalTime/10000+"ms");
            //ClientNet.Log.Info("服务器时间为："+new DateTime(response.Time));
            m_ServerDelayTime = intervalTime;
        }

        /// <summary>
        /// 发送退出战斗请求
        /// </summary>
        public void SendExitBattleReq()
        {
            Send(new C2S_ExitBattleMessage {BattleId = CurrentBattleId, PlayerId = PlayerId});
        }

        public void SendAudioToBattle(AudioBlock audioBlock)
        {
            if (m_currentBattlePlayers == null || m_currentBattlePlayers.Count == 0) return;

            if (audioBlock == null) return;
            C2S_SpeakToBattleReq req = new C2S_SpeakToBattleReq();
            using (MemoryStream ms = new MemoryStream())
            {
                req.LaunchPlayerId = PlayerId;
                req.BattleId = CurrentBattleId;
                foreach (var member in CurrentMatchTeam.GetMembers())
                {
                    req.PlayerIds.Add(member);
                }
                m_bf.Serialize(ms, audioBlock);
                req.Data = ByteString.CopyFrom(ms.ToArray());
                Send(req);
            }

        }

        #endregion

        #region 属性

        public ulong CurrentBattleId
        {
            get => m_currentBattleId;
        }

        #endregion

        #region 事件

        /// <summary>
        /// BattleGameManager注册的事件
        /// </summary>
        public event Action OnCreateBattleEvent;

        /// <summary>
        /// BattleGameManager注册的事件
        /// </summary>
        public event Action OnCloseBattleEvent;

        /// <summary>
        /// 服务器发来最终确认 必须注册
        /// </summary>
        public event Action OnReadyBattleEvent;
        /// <summary>
        /// 服务器发来的有玩家退出战斗
        /// </summary>
        public event Action<string> OnExitBattleEvent;

        public event Action<object> OnFinishBattleEvent;

        /// <summary>
        /// 关卡生成Actor触发的事件、必须注册
        /// </summary>
        public event Action<ulong> OnInitActorMessageEvent
        {
            add
            {
                if (m_initActorContains.Contains(value.Target))
                {
                    return;
                }
                m_initActorContains.Add(value.Target);

                m_CurrentLevel.OnInitMessageHandler += value;
            }
            remove
            {
                m_initActorContains.Remove(value.Target);

                m_CurrentLevel.OnInitMessageHandler -= value;
            }
        }

        /// <summary>
        /// 关卡生成Actor触发的事件、必须注册
        /// </summary>
        public event Action<ulong> OnDestroyMessageEvent
        {
            add
            {
                if (m_DestroyActorContains.Contains(value.Target))
                {
                    return;
                }
                m_DestroyActorContains.Add(value.Target);

                m_CurrentLevel.OnDestroyMessageHandler += value;
            }
            remove
            {
                m_DestroyActorContains.Remove(value.Target);
                m_CurrentLevel.OnDestroyMessageHandler -= value;
            }
        }

        public event Action<string, AudioBlock> OnAudioBattleEvent;


        #endregion

        #region 字段
        private List<object> m_initActorContains = new List<object>();
        private List<object> m_DestroyActorContains = new List<object>();
        /// <summary>
        /// 当前关卡信息，包含了所有Actor
        /// </summary>
        public ClientLevel m_CurrentLevel;
        /// <summary>
        /// 序列化器
        /// </summary>
        public BinaryFormatter m_bf = new BinaryFormatter();
        /// <summary>
        /// 当前战斗Id
        /// </summary>
        public ulong m_currentBattleId;
        /// <summary>
        /// 当前战斗玩家集合
        /// </summary>
        private List<string> m_currentBattlePlayers;
        /// <summary>
        /// 服务器逻辑层频率
        /// </summary>
        private long m_ServerLogicalInterval;
        /// <summary>
        /// 服务器延迟
        /// </summary>
        private long m_ServerDelayTime;
        /// <summary>
        /// 服务器时间
        /// </summary>
        private DateTime m_serverTime;
        /// <summary>
        /// 当前关卡Id
        /// </summary>
        private int m_currentBarrierId;

        #endregion


       
    }
}
