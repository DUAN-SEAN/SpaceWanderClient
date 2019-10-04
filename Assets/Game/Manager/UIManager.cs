using System;
using System.Collections.Generic;
using Assets.Game.Manager.UITask;
using Crazy.Main;
using UnityEngine;

namespace Assets.Game.Manager
{
    public class UIManager : ManagerBase, ITickable
    {
        public UIManager(GameManager gameManager) : base(gameManager)
        {
            TaskDic = new Dictionary<Type, ITaskEventSystem>();
        }

        #region ITickable

        public void Tick()
        {
            if (m_currentPlayerCtx == null || m_currentPlayerCtx != GameManager.Instance.CurrentPlayerContext)
            {
                ReleaseSpacePlayerContext();
                m_currentPlayerCtx = GameManager.Instance.CurrentPlayerContext;
                BindSpacePlayerContext(m_currentPlayerCtx);
            }
            //Debug.Log(this.GetHashCode());
            //m_uiInBattleTask.Update();
            _uiMainMenuTask.Update();
        }

        #endregion

        #region IManagerContext
        public void Initialize()
        {
            _uiLoginTask = new UILoginTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiLoginTask.GetType(), _uiLoginTask);
            _uiSignUpTask = new UISignUpTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiSignUpTask.GetType(), _uiSignUpTask);
            _uiMainMenuTask = new UIMainMenuTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiMainMenuTask.GetType(), _uiMainMenuTask);
            _uiLevelChooseTask = new UILevelChooseTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiLevelChooseTask.GetType(), _uiLevelChooseTask);
            _uiLevelDetailTask = new UILevelDetailTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiLevelDetailTask.GetType(), _uiLevelDetailTask);
            _uiAdaptedDetailTask = new UIAdaptedDetailTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiAdaptedDetailTask.GetType(), _uiAdaptedDetailTask);
            _uiShipHouseTask = new UIShipHouseTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiShipHouseTask.GetType(), _uiShipHouseTask);
            _uiWorldChannelTask = new UIWorldChannelTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiWorldChannelTask.GetType(), _uiWorldChannelTask);
            _uiTwoButtonWindowTask = new UITwoButtonWindowTask(this, m_currentPlayerCtx);
            TaskDic.Add(_uiTwoButtonWindowTask.GetType(), _uiTwoButtonWindowTask);
            _uiLoadingTask = new UILoadingTask(this);
            TaskDic.Add(_uiLoadingTask.GetType(), _uiLoadingTask);
        }


        /// <summary>
        /// 绑定玩家现场, 注册回调事件
        /// </summary>
        public override void BindSpacePlayerContext(SpacePlayerContext spacePlayerContext)
        {
            spacePlayerContext.OnCreateBattleEvent += CreateBattleUI;
            spacePlayerContext.OnReadyBattleEvent += ReadyBattleUI;
            base.BindSpacePlayerContext(spacePlayerContext);
        }

        private void ReadyBattleUI()
        {
            //m_uiInBattleTask.setState(true);
        }

        public override void ReleaseSpacePlayerContext()
        {
            if (m_currentPlayerCtx == null)
            {
                return;
            }

            m_currentPlayerCtx.OnCreateBattleEvent -= CreateBattleUI;
            base.ReleaseSpacePlayerContext();
        }


        #endregion

        /// <summary>
        /// 从 Resource 加载 Panel
        /// </summary>
        /// <param name="path">载入的地址</param>
        /// <returns></returns>
        public Component LoadUIPanelFromResource(string path)
        {
            var instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;

            instPanel?.transform.SetParent(CanvasTransform, false);

            return instPanel?.GetComponent<Component>();
        }
        public void SwitchUIPanelWithTask(ITaskEventSystem task)
        {

        }

        #region Task


        private UILoginTask _uiLoginTask;
        private UISignUpTask _uiSignUpTask;
        private UIMainMenuTask _uiMainMenuTask;
        private UILevelChooseTask _uiLevelChooseTask;
        private UILevelDetailTask _uiLevelDetailTask;
        private UIAdaptedDetailTask _uiAdaptedDetailTask;
        private UIWorldChannelTask _uiWorldChannelTask;
        private UIShipHouseTask _uiShipHouseTask;
        private UITwoButtonWindowTask _uiTwoButtonWindowTask;

        private UILoadingTask _uiLoadingTask;

        //private UIInBattleTask m_uiInBattleTask;

        #endregion

        #region PlayerContext回调事件

        /// <summary>
        /// 加载战斗UI
        /// </summary>
        public void CreateBattleUI()
        {
            // m_uiInBattleTask.Start();

            // m_currentPlayerCtx.SendReadyBattleReq();
        }

        #endregion

        #region 属性

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

        private Transform canvasTransform;

        public Dictionary<Type, ITaskEventSystem> TaskDic;

        #endregion

    }

    public class UIResourceDefine
    {
        public const string LoginWindowPath = "UIPanel/LoginWindow";
        public const string MainMenuPath = "UIPanel/MainMenu";
        public const string SignupPanelPath = "UIPanel/SignupPanel";
        public const string MessageWindowPath = "UIPanel/MessageWindow";
        public const string LevelChoosePanelPath = "UIPanel/LevelChoosePanel";
        public const string LevelDetailPanelPath = "UIPanel/LevelDetailPanel";
        public const string InBattlePanelPath = "UIPanel/InBattlePanel";
        public const string AdaptedDetailPanelPath = "UIPanel/AdaptedDetailPanel";
        public const string ShipHousePanelPath = "UIPanel/ShipHousePanel";
        public const string LodingPanelPath = "UIPanel/LoadingPanel";


        public const string WorldChannelPath = "UIPanel/WorldChannel";
        public const string TwoButtonWindowPath = "UIPanel/TwoButtonWindow";
        public const string PlayerItemPath = "UIPanel/PlayerItem";
        public const string LevelItemPath = "UIPanel/LevelItem";
    }

}
