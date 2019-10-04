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
    public class UITwoButtonWindowTask : ITaskEventSystem
    {
        public UITwoButtonWindowTask(UIManager um, SpacePlayerContext plx)
        {
            _um = um;
            _plx = plx;
        }
        
        public void Start()
        {
            _twoButtonWindow = _um.LoadUIPanelFromResource(UIResourceDefine.TwoButtonWindowPath).gameObject;
            _twoButtonWindowController = _twoButtonWindow.GetComponent<TwoButtonWindowController>();
            _twoButtonWindowController.Open();
            BindEvent();
        }

        public void Update()
        {

        }

        public void Dispose()
        {
            ReleaseBindingEvent();
        }
        
        private void BindEvent()
        {
        }
        
        private void ReleaseBindingEvent()
        {
        }
        


        private SpacePlayerContext _plx;
        private UIManager _um;

        private TwoButtonWindowController _twoButtonWindowController;
        
        private GameObject _twoButtonWindow;

    }
}