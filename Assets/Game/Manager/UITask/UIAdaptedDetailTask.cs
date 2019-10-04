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
    public class UIAdaptedDetailTask : ITaskEventSystem<List<string>,string,int>
    {
        public UIAdaptedDetailTask(UIManager um, SpacePlayerContext plx)
        {
            _plx = plx;
            _um = um;
        }

        private void BindEvent()
        {
            _adaptedDetailController.OnCancelButton += OnCancel;
        }

        private void ReleaseEvent()
        {
            _adaptedDetailController.OnCancelButton -= OnCancel;
        }

        public void Start(List<string> list,string o,int count)
        {
            if (_adaptedDetailPanel == null)
                _adaptedDetailPanel = _um.LoadUIPanelFromResource(UIResourceDefine.AdaptedDetailPanelPath).gameObject;
            
            _adaptedDetailController = _adaptedDetailPanel.GetComponent<AdaptedDetailController>();

            _adaptedDetailController.Open(list,o,count);

            BindEvent();
        }

        public void Update()
        {

        }

        public void Dispose()
        {
            ReleaseEvent();
            _adaptedDetailController.Closed();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        private void OnCancel()
        {
            Dispose();
        }

        private SpacePlayerContext _plx;
        private UIManager _um;

        private AdaptedDetailController _adaptedDetailController;
        
        private GameObject _adaptedDetailPanel;
        
    }
}
