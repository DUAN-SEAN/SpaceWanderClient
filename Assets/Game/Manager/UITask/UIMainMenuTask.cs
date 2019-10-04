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
    public class UIMainMenuTask : ITaskEventSystem
    {
        public UIMainMenuTask(UIManager um, SpacePlayerContext plx)
        {
            _um = um;
            _plx = plx;
        }

        public void Start()
        {
            if (_mainMenu == null)
                _mainMenu = _um.LoadUIPanelFromResource(UIResourceDefine.MainMenuPath).gameObject;

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
            
            _mainMenuController = _mainMenu.GetComponent<MainMenuController>();
            
            _mainMenuController.Open();

            gbStack = new Stack<GameObject>();

            MatchTeam matchTeam = _plx.GetMatchTeamInfo();
            
            if (matchTeam == null)
            {
                _mainMenuController.RefreshTeamGroup(0, null , 0);
            }
            else
            {
                _mainMenuController.RefreshTeamGroup(matchTeam.GetMembers().Count, matchTeam.GetMembers(),matchTeam.Id);
            }

            //_plx.GetOnlinePlayerInfoReq();
            _mainMenuController.OnSwitchAudioButton += GameManager.Instance.VideoManager.OnSwitchAudio;



            BindEvent();
        }
        
            
        public void Update()
        {
            if (Time.frameCount % 20 == 0)
            {
                if (_plx.PlayerId != null)
                {
                    _plx.GetMatchTeamInfoReq();
                    _plx.GetOnlinePlayerInfoReq();
                }
            }

            if (Time.frameCount % 1 == 0)
            {
                if (OnlinePlayerInfoTupleList != null)
                {
                    foreach (Tuple<string, int> temp in OnlinePlayerInfoTupleList)
                    {
                        if (temp.Item1.Equals(_plx.PlayerId))
                        {
                            playerState = temp.Item2;
                        }
                    }
                }

                if (playerState == 3)
                {
                    if (_mainMenuController != null)
                        _mainMenuController.SwitchButton1ToButton2();
                }
                else
                {
                    if (_mainMenuController != null)
                        _mainMenuController.SwitchButton2ToButton1();
                }
            }
        }

        public void Dispose()
        {
            _mainMenuController.Closed();
            ReleaseBindingEvent();
        }

        /// <summary>
        /// 创建邀请界面
        /// </summary>
        private void BeInvitedWindow(string inviterId)
        {
            _twoButtonWindow = _um.LoadUIPanelFromResource(UIResourceDefine.TwoButtonWindowPath).gameObject;

            gbStack.Push(_twoButtonWindow);
            _twoButtonWindowController = _twoButtonWindow.GetComponent<TwoButtonWindowController>();
            _twoButtonWindowController.LeftButton += OnLeftButton;
            _twoButtonWindowController.RightButton += OnRightButton;
            string content = inviterId + " 邀请您加入队伍！";
            string leftBtn = "拒绝";
            string rightBtn = "接受";
            _twoButtonWindowController.OnChangeContent(content, leftBtn, rightBtn);
            SwitchMainMenuBlocksRaycasts();
        }
        /// <summary>
        /// 切换是否可点击
        /// </summary>
        private void SwitchMainMenuBlocksRaycasts()
        {
            if (gbStack.Count != 0)
            {
                _mainMenuController.SwitchBlocksRaycasts(false);
            }
            else
            {
                _mainMenuController.SwitchBlocksRaycasts(true);
            }
        }

        /// <summary>
         /// 绑定事件
         /// </summary>
        private void BindEvent()
        {
            _plx.CreateTeamCallBack += OnCreateTeamplxAck;
            _plx.UpdateMatchTeamCallBack += UpdateMatchTeamplxAck;
            _plx.UpdateOnlinePlayerInfo += OnGetOnlinePlayerAck;
            _plx.InvitedTeamCallBack += OnInvitedAck;
            
            _mainMenuController.OnLevelChooseButton += OnOpenLevelChooseButton;
            _mainMenuController.OnWorldChannelButton += OnOpenWorldChannelButton;
            _mainMenuController.OnShipHouseButton += OnOpenShipHouseButton;
            _mainMenuController.OnCreateTeamButton += OnCreateTeamButton;
            _mainMenuController.OnLeaveTeamButton += OnLeaveTeamButton;
            _mainMenuController.OnQuitMatchingButton += OnQuitMatchingButton;
            _mainMenuController.OnLogoutButton += OnLogout;

            _mainMenuController.OnStillBuildingButton += OnStillBuilding;
        }
        /// <summary>
        /// 释放绑定的事件
        /// </summary>
        private void ReleaseBindingEvent()
        {
            _plx.CreateTeamCallBack -= OnCreateTeamplxAck;
            _plx.UpdateMatchTeamCallBack -= UpdateMatchTeamplxAck;
            _plx.UpdateOnlinePlayerInfo -= OnGetOnlinePlayerAck;
            _plx.InvitedTeamCallBack -= OnInvitedAck;
            
            _mainMenuController.OnLevelChooseButton -= OnOpenLevelChooseButton;
            _mainMenuController.OnWorldChannelButton -= OnOpenWorldChannelButton;
            _mainMenuController.OnShipHouseButton -= OnOpenShipHouseButton;
            _mainMenuController.OnCreateTeamButton -= OnCreateTeamButton;
            _mainMenuController.OnLeaveTeamButton -= OnLeaveTeamButton;
            _mainMenuController.OnQuitMatchingButton -= OnQuitMatchingButton;
            _mainMenuController.OnLogoutButton -= OnLogout;

            _mainMenuController.OnStillBuildingButton -= OnStillBuilding;

        }

        #region Unity

        /// <summary>
        /// 创建队伍
        /// </summary>
        private void OnCreateTeamButton()
        {
            _plx.CreateMatchTeamReq();
        }
        /// <summary>
        /// 退出队伍
        /// </summary>
        private void OnLeaveTeamButton()
        {
            _plx.ExitMatchTeamReq();
        }
        /// <summary>
        /// 退出账户
        /// </summary>
        private void OnLogout()
        {
            //todo

        }
        /// <summary>
        /// 待开发
        /// </summary>
        private void OnStillBuilding()
        {
            _messageWindowController.Open("待开发");
        }
        /// <summary>
        /// 进入选择关卡
        /// </summary>
        private void OnOpenLevelChooseButton()
        {
            Dispose();
            _um.TaskDic[typeof(UILevelChooseTask)].Start();
        }
        /// <summary>
        /// 进入世界频道
        /// </summary>
        private void OnOpenWorldChannelButton()
        {
            _plx.GetOnlinePlayerInfoReq();

            (_um.TaskDic[typeof(UIWorldChannelTask)] as UIWorldChannelTask).Start(OnlinePlayerInfoTupleList);
        }
        /// <summary>
        /// 进入飞船改造
        /// </summary>
        private void OnOpenShipHouseButton()
        {
            Dispose();
            _um.TaskDic[typeof(UIShipHouseTask)].Start();
        }
        /// <summary>
        /// 取消匹配
        /// </summary>
        private void OnQuitMatchingButton()
        {
            _plx.ExitMatchQueueReq();
            _mainMenuController.SwitchButton2ToButton1();
        }
        /// <summary>
        /// 双按钮窗体左按钮
        /// </summary>
        private void OnLeftButton()
        {
            CloseTwoButtonWindow();
        }
        /// <summary>
        /// 双按钮窗体右按钮
        /// </summary>
        private void OnRightButton()
        {
            _plx.JoinMatchTeamReq(InviterTeamId);

            CloseTwoButtonWindow();
        }
        /// <summary>
        /// 关闭窗口按钮
        /// </summary>
        private void CloseTwoButtonWindow()
        {
            _twoButtonWindowController.LeftButton -= OnLeftButton;
            _twoButtonWindowController.RightButton -= OnRightButton;
            _twoButtonWindowController.Closed(gbStack.Peek());
            gbStack.Pop();
            SwitchMainMenuBlocksRaycasts();
        }
        /// <summary>
        /// 获取在线玩家信息
        /// </summary>
        public List<Tuple<string, int>> GetOnlinePlayerInfo()
        {
            return OnlinePlayerInfoTupleList;
        }
        #endregion

        #region playerContext 回调

        private void OnInvitedAck(ulong teamId,string playerId)
        {
            InviterTeamId = teamId;
            BeInvitedWindow(playerId);
        }
        
        private void OnGetOnlinePlayerAck(List<Tuple<string, int>> strList)
        {
            OnlinePlayerInfoTupleList = strList;
        }

        private void OnCreateTeamplxAck(int obj)
        {
            if(obj == 0)
            {
                Log.Info("OnCreateTeamplxAck::创建队伍回调 队伍Id = " + _plx.GetMatchTeamInfo().Id);
                MatchTeam matchTeam = _plx.GetMatchTeamInfo();
                _mainMenuController.RefreshTeamGroup(matchTeam.GetMembers().Count, matchTeam.GetMembers(),matchTeam.Id);

                _messageWindowController.Open("创建队伍成功");
            }
            else
            {
                Log.Info("OnCreateTeamplxAck::创建队伍失败");
                _messageWindowController.Open("创建队伍失败，您已拥有队伍");
            }
        }
        
        private void UpdateMatchTeamplxAck()
        {
            MatchTeam matchTeam = _plx.GetMatchTeamInfo();

            if (matchTeam == null)
            {
                _mainMenuController.RefreshTeamGroup(0, null , 0);
            }
            else
            {
                _mainMenuController.RefreshTeamGroup(matchTeam.GetMembers().Count, matchTeam.GetMembers(), matchTeam.Id);
            }

        }

        #endregion

        private int playerState;
        private List<Tuple<string,int>> OnlinePlayerInfoTupleList;
        private Stack<GameObject> gbStack;
        private ulong InviterTeamId;

        private SpacePlayerContext _plx;
        private UIManager _um;

        private MainMenuController _mainMenuController;
        private MessageWindowController _messageWindowController;
        private TwoButtonWindowController _twoButtonWindowController;
        
        private GameObject _mainMenu;
        private GameObject _messageWindow;
        private GameObject _twoButtonWindow;


    }
}