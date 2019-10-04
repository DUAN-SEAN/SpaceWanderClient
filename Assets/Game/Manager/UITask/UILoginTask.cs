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
    public class UILoginTask:ITaskEventSystem
    {
        public UILoginTask(UIManager um,SpacePlayerContext plx)
        {
            _um = um;
            _plx = plx;
        }
        
        public void Start()
        {
            if (_loginPanel == null) 
                _loginPanel = _um.LoadUIPanelFromResource(UIResourceDefine.LoginWindowPath).gameObject;

            if (_messageWindow == null)
                _messageWindow = _um.LoadUIPanelFromResource(UIResourceDefine.MessageWindowPath).gameObject;

            _accountText = _loginPanel.gameObject.transform.Find("Account/AccountInput").GetComponent<InputField>();
            _passwordText = _loginPanel.gameObject.transform.Find("Password/PasswordInput").GetComponent<InputField>();

            _loginController = _loginPanel.GetComponent<LoginController>();
            _messageWindowController = _messageWindow.GetComponent<MessageWindowController>();

            _loginController.Open();

            BindEvent();
        }
        
        public void Update()
        {

        }

        public void Dispose()
        {
            ReleaseBindingEvent();
            _loginController.Closed();
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        private void BindEvent()
        {
            _loginController.OnLoginButton += OnLoginButton;
            _loginController.OnOpenRegisterButton += OnOpenSignUpButton;

            _plx.OnLoginCallBack += OnLoginPlxAck;
        }

        /// <summary>
        /// 释放绑定的事件
        /// </summary>
        private void ReleaseBindingEvent()
        {
            _loginController.OnLoginButton -= OnLoginButton;
            _loginController.OnOpenRegisterButton -= OnOpenSignUpButton;

            _plx.OnLoginCallBack -= OnLoginPlxAck;
        }

        #region PlayerContext回调
        /// <summary>
        /// 登录回调
        /// </summary>
        private void OnLoginPlxAck(int state)
        {
            if(state == 1)
            {
                //UI切换
                _um.TaskDic[typeof(UIMainMenuTask)].Start();

                _messageWindowController.Open("登录成功");
                
                Dispose();
                
            }
            else
            {
                _messageWindowController.Open("登录失败");
            }
        }
        
        #endregion

        #region Unity

        private void OnLoginButton()
        {
            string account = _accountText.text;
            string password = _passwordText.text;

            _plx.Login(account,password);
        }

        private void OnOpenSignUpButton()
        {
            _um.TaskDic[typeof(UISignUpTask)].Start();
            _loginController.SwitchBlocksRaycasts(false);
        }
        
        #endregion
        
        private SpacePlayerContext _plx;
        private UIManager _um;

        private InputField _accountText;
        private InputField _passwordText;

        private LoginController _loginController;
        private MessageWindowController _messageWindowController;

        private GameObject _loginPanel;
        private GameObject _messageWindow;
    }
}
