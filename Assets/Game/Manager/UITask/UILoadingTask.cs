using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Manager.UITask.Controller;
using UnityEngine;

namespace Assets.Game.Manager.UITask
{
    public class UILoadingTask:ITaskEventSystem
    {
        public UILoadingTask(UIManager um)
        {
            _um = um;
        }
        public void Init()
        {
            
        }

        public void Start()
        {
            if (_loadingPanel == null)
                _loadingPanel = _um.LoadUIPanelFromResource(UIResourceDefine.LodingPanelPath).gameObject;

            
            _loadingController = _loadingPanel.GetComponent<LoadingController>();
            _loadingPanel.SetActive(true);
        }

        public void Update()
        {
            
        }

        public void Dispose()
        {
            _loadingPanel.SetActive(false);
        }
        public void SetLoadingText(float displayProgress, string text)
        {
            _loadingController.SetLoadingText(displayProgress,text);
        }
        private UIManager _um;
        private GameObject _loadingPanel;
        private LoadingController _loadingController;
    }
}
