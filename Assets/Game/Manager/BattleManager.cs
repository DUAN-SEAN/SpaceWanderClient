using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Manager.BattleTask;
using Assets.Game.Manager.UITask;
using Crazy.ClientNet;
using Crazy.Main;
using UnityEngine;

namespace Assets.Game.Manager
{
    public class BattleManager: ManagerBase,ITickable
    {
        public BattleManager(GameManager gameManager):base(gameManager)
        {

        }

        #region ITickable

        /// <summary>
        /// Tick当前Task列表
        /// </summary>
        public void Tick()
        {
            if (m_currentPlayerCtx == null || m_currentPlayerCtx != GameManager.Instance.CurrentPlayerContext)
            {
                ReleaseSpacePlayerContext();
                m_currentPlayerCtx = GameManager.Instance.CurrentPlayerContext;
                BindSpacePlayerContext(m_currentPlayerCtx);
            }
            try
            {
                //Log.Info("我是tick1");
                if (IsOnCreateBattle)
                {
                    //Log.Info("我是tick2");
                    battleTask.Update();
                    m_uiInBattleTask.Update();
                }
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
            


        }

        #endregion

        #region IManagerContext
        public  override  void Initialize()
        {
            battleTask = new BattleMainTask(this, m_currentPlayerCtx);
            m_uiInBattleTask = new UIInBattleTask(this, m_currentPlayerCtx);
            m_cgBattleTask = new CGBattleTask();
            m_cgBattleTask.Init();
            battleTask.Init();
            base.Initialize();
            
        }
        public Component LoadUIPanelFromResource(string path)
        {
            var instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;

            instPanel?.transform.SetParent(CanvasTransform, false);

            return instPanel?.GetComponent<Component>();
        }

        /// <summary>
        /// 绑定玩家现场,注册回调事件
        /// </summary>
        public override void BindSpacePlayerContext(SpacePlayerContext spacePlayerContext)
        {
            spacePlayerContext.OnFinishBattleEvent += OnFinishBattle;
            spacePlayerContext.OnCreateBattleEvent += OnCreateBattle;
            spacePlayerContext.OnCloseBattleEvent += OnCloseBattle;
            spacePlayerContext.OnReadyBattleEvent += OnReadyBattle;
            base.BindSpacePlayerContext(spacePlayerContext);



        }

        private void OnReadyBattle()
        {
            battleTask.OnReadyBattle();
        }

        public override void ReleaseSpacePlayerContext()
        {
            if (m_currentPlayerCtx == null) return;
            m_currentPlayerCtx.OnFinishBattleEvent -= OnFinishBattle;
            m_currentPlayerCtx.OnCreateBattleEvent -= OnCreateBattle;
            m_currentPlayerCtx.OnCloseBattleEvent -= OnCloseBattle;
            m_currentPlayerCtx.OnReadyBattleEvent -= OnReadyBattle;
            base.ReleaseSpacePlayerContext();
        }


        #endregion

        #region PlayerContext回调事件
        //生成战斗的Task
        private void OnCreateBattle()
        {
            
            Log.Info("BattleManager 收到创建战斗的事件");

            Log.Info(m_currentPlayerCtx.GetMatchTeamInfo().Id+"   BattleId = "+m_currentPlayerCtx.m_CurrentLevel.GetBattleID());
            GameManager.Instance.LoadScenceFromTask("battle", () =>
            {
                
                battleTask.Start();
                battleTask.OnCreateBattle();
              
                UIInBattleTask.BindActorDic(battleTask.ActorDic);
                UIInBattleTask.Start();

                m_currentPlayerCtx.SendReadyBattleReq();
                battleTask.InitCamera();

            });
            //m_uiInBattleTask.Start();
            IsOnCreateBattle = true;
            
           

        }
        //关闭战斗的Task
        private void OnCloseBattle()
        {
            IsOnCreateBattle = false;
            //todo 暂时处理为跳场景 打主界面
            battleTask?.Stop();
            m_uiInBattleTask?.Stop();
            GameManager.Instance.LoadScenceFromTask("UILobby", () =>
            {
                GameManager.Instance.UiManager.TaskDic[typeof(UIMainMenuTask)]?.Start();
            });

        }

        private void OnFinishBattle(object obj)
        {
            IsOnCreateBattle = false;
            //todo 暂时处理为跳场景 到主界面
            Debug.Log("OnFinishBattle 1");

            battleTask?.Stop();
            Debug.Log("OnFinishBattle 2");
            m_uiInBattleTask?.Stop();
            Debug.Log("OnFinishBattle 3");

         
            GameManager.Instance.LoadScenceFromTask("UILobby", () =>
            {
               
                
                GameManager.Instance.UiManager.TaskDic[typeof(UIMainMenuTask)]?.Start();
                //battleTask?.Dispose();
                m_uiInBattleTask?.Dispose();
            });

        }

        #endregion

        #region Task
        private BattleMainTask battleTask;
        /// <summary>
        /// 战斗界面
        /// </summary>
        private UIInBattleTask m_uiInBattleTask;

        private CGBattleTask m_cgBattleTask;
        #endregion

        #region 属性
        public UIInBattleTask UIInBattleTask => m_uiInBattleTask;

        private Transform CanvasTransform
        {
            get
            {
                if (canvasTransform == null)
                {
                    canvasTransform = GameObject.Find("Canvas").transform;
                }
                return canvasTransform;
            }
        }


        #endregion

        #region 字段
        private bool IsOnCreateBattle = false;
        private Transform canvasTransform;

        #endregion


        



    }
}
