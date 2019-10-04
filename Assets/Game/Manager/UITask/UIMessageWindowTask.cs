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
    public class UIMessageWindowTask : ITaskEventSystem
    {
        public UIMessageWindowTask(UIManager um, SpacePlayerContext plx)
        {
            _um = um;
            _plx = plx;
        }
        
        public void Start()
        {
            _messageWindow = _um.LoadUIPanelFromResource(UIResourceDefine.MessageWindowPath).gameObject;
            
            _messageWindowController = _messageWindow.GetComponent<MessageWindowController>();
         
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

        private MessageWindowController _messageWindowController;
        
        private GameObject _messageWindow;
        
    }
}