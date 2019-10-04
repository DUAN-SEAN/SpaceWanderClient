using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.ClientNet;
using Crazy.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Manager.UITask
{
    public class UILevelDetailTask: ITaskEventSystem<List<string>>
    {
        public UILevelDetailTask(UIManager um, SpacePlayerContext plx)
        {
            _plx = plx;
            _um = um;
        }

        public void Start(List<string> str)
        {
            if (_levelDetailPanel == null)
                _levelDetailPanel = _um.LoadUIPanelFromResource(UIResourceDefine.LevelDetailPanelPath).gameObject;

            _levelDetailController = _levelDetailPanel.GetComponent<LevelDetailController>();
            _levelChooseController = GameObject.Find("LevelChoosePanel(Clone)").GetComponent<LevelChooseController>();

            _levelDetailController.Open(str);

            BindEvent();
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            _levelDetailController.Closed();
            ReleaseEvent();
        }

        private void BindEvent()
        {
            _levelDetailController.OnJoinMatchQueueEventButton += OnJoinMatchQueueButton;

            _plx.JoinMatchQueueCallBack += OnJoinMatchQueueplxAck;
        }

        private void ReleaseEvent()
        {
            _levelDetailController.OnJoinMatchQueueEventButton -= OnJoinMatchQueueButton;

            _plx.JoinMatchQueueCallBack -= OnJoinMatchQueueplxAck;
        }
      

        #region Unity

        private void OnJoinMatchQueueButton()
        {
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.BarrierConfigs.Length; i++)
            {

                if (GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].Name.Equals(_levelDetailController.LevelName))
                {
                    LevelBarrierId = GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].Id;
                    break;
                }
            }

            _plx.JoinMatchQueueReq(LevelBarrierId);
        }

        #endregion

        #region PlayerContext
        /// <summary>
        /// 1 失败 0 成功
        /// </summary>
        /// <param name="state"></param>
        private void OnJoinMatchQueueplxAck(int state)
        {
            if(state == 1)
            {
                Log.Info("加入匹配队列失败");
            }
            else
            {
                Log.Info("加入匹配队列成功");
                _um.TaskDic[typeof(UIMainMenuTask)].Start();
                (_um.TaskDic[typeof(UILevelChooseTask)] as UILevelChooseTask).Dispose("Matching");
                this.Dispose();
            }
        }


        #endregion
        public void Start()
        {
            throw new NotImplementedException();
        }


        private int LevelBarrierId;
        private SpacePlayerContext _plx;
        private UIManager _um;

        private LevelDetailController _levelDetailController;
        private LevelChooseController _levelChooseController;

        private GameObject _levelDetailPanel;
    }
}
