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
    public class UILevelChooseTask:ITaskEventSystem
    {
        public UILevelChooseTask(UIManager um, SpacePlayerContext spc)
        {
            _plx = spc;
            _um = um;
        }
        private void BindEvent()
        {
            _levelChooseController.OnCancelButton += OnCancel;
            _levelChooseController.OnOpenLevelDetailButton += OnOpenLevelDetailButton;
        }
        
        private void ReleaseEvent()
        {
            _levelChooseController.OnCancelButton -= OnCancel;
            _levelChooseController.OnOpenLevelDetailButton -= OnOpenLevelDetailButton;
        }

        public void Start()
        {
            if (_levelChoosePanel == null)
                _levelChoosePanel = _um.LoadUIPanelFromResource(UIResourceDefine.LevelChoosePanelPath).gameObject;
            
            _levelChooseController = _levelChoosePanel.GetComponent<LevelChooseController>();

            list = new List<string>();
            levelDetailList = new List<string>();

            OnGetLevelItem();

            _levelChooseController.Open();

            RefreshLevelChoose();
            
            BindEvent();
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            ReleaseEvent();

            _um.TaskDic[typeof(UIMainMenuTask)].Start();

            _levelChooseController.Closed();
        }
        public void Dispose(string State)
        {
            
            ReleaseEvent();
            
            _levelChooseController.Closed();
        }

        private void OnCancel()
        {
            this.Dispose();
        }

        private void RefreshLevelChoose()
        {
            ClearLevelChoose();
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.BarrierConfigs.Length; i++)
            {
                AddLevelItem();
            }

            SetItemValue();
            list.Clear();
        }

        private void SetItemValue()
        {
            buttons = _levelChooseController.GetButtons();
            for (int i = 0; i< buttons.Length; i++)
            {
                buttons[i].gameObject.transform.Find("Content/Level").GetComponent<Text>().text = list[2 * i];
                buttons[i].gameObject.transform.Find("Content/MemberCount").GetComponent<Text>().text = list[2 * i+1];
            }
        }

        private void AddLevelItem()
        {
            Item_parent = GameObject.Find("LevelChoosePanel(Clone)/BG_not_ui_Panel/Scroll View/Viewport/Content");
            _levelItem = _um.LoadUIPanelFromResource(UIResourceDefine.LevelItemPath).gameObject;
            _levelItemlevelChooseController = _levelItem.GetComponent<LevelChooseController>();
            _levelItemlevelChooseController.OnOpenLevelDetailButton += OnOpenLevelDetailButton;
            _levelItem.transform.parent = Item_parent.transform;
            _levelItem.transform.localScale = new Vector3(1f, 1f, 1f);
            GameObject.Find("LevelItem(Clone)").name = "LevelItem(Clone)+";
        }

        private void ClearLevelChoose()
        {
            while (GameObject.Find("LevelItem(Clone)+"))
            {
                GameObject.Find("LevelItem(Clone)+").GetComponent<LevelChooseController>().OnOpenLevelDetailButton -= OnOpenLevelDetailButton;

                _levelChooseController.OnDestroy();
            }
        }

        private void OnOpenLevelDetailButton()
        {
            OnGetLevelDetail(); 
            (( _um.TaskDic[typeof(UILevelDetailTask)])as UILevelDetailTask).Start(levelDetailList);
            levelDetailList.Clear();
        }

        public void OnGetLevelDetail()
        {
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.BarrierConfigs.Length; i++)
            {

                if (GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].Level.ToString().Equals(LevelItemController.BeChooseLevel)&& GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].MemberCount.ToString().Equals(LevelItemController.BeChooseMemberCount))
                {
                    levelDetailList.Add(GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].Name);
                    if (GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].TaskConfigs == null) levelDetailList.Add(null);
                    else levelDetailList.Add(GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].TaskConfigs[i].Description);
                    break;
                }
            }
        }

        public void OnGetLevelItem()
        {
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.BarrierConfigs.Length; i++)
            {
                GetLevelData(i);
            }
        }

        public void GetLevelData(int i)
        {
            list.Add(GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].Level.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.BarrierConfigs[i].MemberCount.ToString());
        }

        private GameObject _levelItem;
        private GameObject Item_parent;

        private Button[] buttons;

        public List<string> list;
        public List<string> levelDetailList;
        
        private SpacePlayerContext _plx;
        private UIManager _um;

        private LevelChooseController _levelChooseController;
        private LevelChooseController _levelItemlevelChooseController;
        private LevelItemController _levelItemController;

        private GameObject _levelChoosePanel;
    }
}
