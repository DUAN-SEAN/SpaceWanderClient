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
    public class UISignUpTask:ITaskEventSystem
    {
        public UISignUpTask(UIManager um, SpacePlayerContext spc)
        {
            _plx = spc;
            _um = um;
        }
        private void BindEvent()
        {
            _signUpController.OnCancelButton += OnCancel;
            _signUpController.OnSignUpButton += OnSignUp;

            _plx.OnSignUpCallBack += OnSingUpAck;
        }
        
        private void ReleaseEvent()
        {
            _signUpController.OnCancelButton -= OnCancel;
            _signUpController.OnSignUpButton -= OnSignUp;

            _plx.OnSignUpCallBack -= OnSingUpAck;
        }
        public void Start()
        {
            if(_signupPanel == null)
                _signupPanel = _um.LoadUIPanelFromResource(UIResourceDefine.SignupPanelPath).gameObject;

            if (!GameObject.Find("MessageWindow(Clone)"))
            {
                if (_messageWindow == null)
                    _messageWindow = _um.LoadUIPanelFromResource(UIResourceDefine.MessageWindowPath).gameObject;
            }
            else
            {
                _messageWindow = GameObject.Find("MessageWindow(Clone)");
            }
            _messageWindowController = _messageWindow.GetComponent<MessageWindowController>();

            _accountText = _signupPanel.gameObject.transform.Find("Account/AccountInput").GetComponent<InputField>();

            _passwordText = _signupPanel.gameObject.transform.Find("Password/PasswordInput").GetComponent<InputField>();

            _passwordText2 = _signupPanel.gameObject.transform.Find("Password2/PasswordInput2").GetComponent<InputField>();

            _signUpController = _signupPanel.GetComponent<SignUpController>();
            _loginController = GameObject.Find("Canvas/LoginWindow(Clone)").GetComponent<LoginController>();
           
            
            BindEvent();
        }

        public void Update()
        {

        }

        public void Dispose()
        {

            ReleaseEvent();
            _loginController.SwitchBlocksRaycasts(true);
            GameObject.Destroy(_signupPanel);
        }

        #region PlayerContext回调

        private void OnSingUpAck(int obj)
        {
            if (obj == 1)
            {
                _um.TaskDic[typeof(UILoginTask)].Start();

                Dispose();

                _messageWindowController.Open("注册成功！");

            }
            else
            {
                _messageWindowController.Open("注册失败！");
            }
        }
        #endregion

        #region Unity
        private void OnSignUp()
        {
            string account = _accountText.text;
            string password = _passwordText.text;
            string password2 = _passwordText2.text;

            _plx.SignUp(account, password);
        }

        private void OnCancel()
        {
            this.Dispose();
        }
        #endregion


        private SpacePlayerContext _plx;
        private UIManager _um;
        
        private InputField _accountText;
        private InputField _passwordText;
        private InputField _passwordText2;
        
        private SignUpController _signUpController;
        private LoginController _loginController;
        private MessageWindowController _messageWindowController;

        private GameObject _messageWindow;
        private GameObject _signupPanel;
    }
}
