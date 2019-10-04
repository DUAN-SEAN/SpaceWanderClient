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
    public class UIWorldChannelTask : ITaskEventSystem<List<Tuple<string, int>>>
    {
        public UIWorldChannelTask(UIManager um, SpacePlayerContext plx)
        {
            _um = um;
            _plx = plx;
        }
        
        public void Start(List<Tuple<string, int>> OnlinePlayerInfoTupleList)
        {
            if (_worldChannel == null)
                _worldChannel = _um.LoadUIPanelFromResource(UIResourceDefine.WorldChannelPath).gameObject;

            _worldChannelController = _worldChannel.GetComponent<WorldChannelController>();

            if (_messageWindow == null)
                _messageWindow = _um.LoadUIPanelFromResource(UIResourceDefine.MessageWindowPath).gameObject;

            _messageWindowController = _messageWindow.GetComponent<MessageWindowController>();


            OnlinePlayerInfoStringList = new List<string>();

            foreach (Tuple<string, int> temp in OnlinePlayerInfoTupleList)
            {
                OnlinePlayerInfoStringList.Add(temp.Item1);
            }

            RefreshWorldChannel(OnlinePlayerInfoStringList);

            BindEvent();
        }

        public void Update()
        {

        }

        public void Dispose()
        {
            ReleaseBindingEvent();
        }
        
        private void RefreshWorldChannel(List<string> strList)
        {
            ClearWorldChannel();
            for(int i = 0; i < strList.Count; i++)
            {
                AddPlayerItem();
                _playerItem.gameObject.transform.Find("Content/PlayerIdContent").GetComponent<Text>().text = strList[i];
            }
        }

        private void AddPlayerItem()
        {
            Item_parent = GameObject.Find("WorldChannel(Clone)/WorldChannelPanel/Scroll View/Viewport/Content");
            _playerItem = _um.LoadUIPanelFromResource(UIResourceDefine.PlayerItemPath).gameObject;
            _playerItemWorldChannelController = _playerItem.GetComponent<WorldChannelController>();
            _playerItemWorldChannelController.OnInviteButton += OnInvite;
            _playerItem.transform.parent = Item_parent.transform;
            _playerItem.transform.localScale = new Vector3(1f, 1f, 1f);
            GameObject.Find("PlayerItem(Clone)").name = "PlayerItem(Clone)+";
        }

        private void ClearWorldChannel()
        {
            while (GameObject.Find("PlayerItem(Clone)+"))
            {
                GameObject.Find("PlayerItem(Clone)+").GetComponent<WorldChannelController>().OnInviteButton -= OnInvite;

                _worldChannelController.OnDestroy();
            }
        }

        public void OnSearchPlayer()
        {

            List<string> strList = new List<string>();
            List<string> searchList = new List<string>();
            strList = OnlinePlayerInfoStringList;
            if (_worldChannelController.WeatherContainsPlayer(strList)!= -1)
            {
                searchList.Add(strList[_worldChannelController.WeatherContainsPlayer(strList)]);
                
                RefreshWorldChannel(searchList);
            }
            else
            {
                _messageWindowController.Open("查无此人");
            }
        }
        public void OnCancel()
        {
            ClearWorldChannel();
            _worldChannelController.Closed();
            Dispose();
        }

        public void OnInvite()
        {
            MatchTeam matchTeam = _plx.GetMatchTeamInfo();
            if (matchTeam != null)
            {
                if (_plx.PlayerId!= PlayerItemController.inviteePlayerName)
                {
                    int playerState = -1;
                    
                    foreach (Tuple<string, int> temp in uIMainMenuTask.GetOnlinePlayerInfo())
                    {
                        if (temp.Item1.Equals(PlayerItemController.inviteePlayerName))
                        {
                            playerState = temp.Item2;
                        }
                        else
                        {
                            playerState = -2;
                            _messageWindowController.Open("意外出错");
                        }
                    }

                    if (playerState==0)
                    {
                        _messageWindowController.Open("发出邀请给：" + PlayerItemController.inviteePlayerName);
                        _plx.InviteMatchTeamReq(PlayerItemController.inviteePlayerName);

                    }
                    else
                    {
                        switch (playerState)
                        {
                            case -2:
                                _messageWindowController.Open("邀请出现意外错误");
                                break;
                            case -1:
                                _messageWindowController.Open("邀请出现意外错误");
                                break;
                            case 1:
                                _messageWindowController.Open("该用户已拥有队伍");
                                break;
                            case 2:
                                _messageWindowController.Open("该用户正在战斗中");
                                break;
                            case 3:
                                _messageWindowController.Open("该用户正在匹配中");
                                break;
                            default:
                                _messageWindowController.Open("邀请出现意外错误");
                                break;
                        }
                    }
                }
                else
                {
                    _messageWindowController.Open("无法邀请自己加入队伍");
                }
            }
            else
            {
                _messageWindowController.Open("你需要先创建队伍");

            }
        }

        private void BindEvent()
        {

            _worldChannelController.OnCancelButton += OnCancel;
            _worldChannelController.OnSearchPlayerButton += OnSearchPlayer;
        }
        
        private void ReleaseBindingEvent()
        {
            _worldChannelController.OnCancelButton -= OnCancel;
            _worldChannelController.OnSearchPlayerButton -= OnSearchPlayer;

        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        private SpacePlayerContext _plx;
        private UIManager _um;

        private UIMainMenuTask uIMainMenuTask;
        private WorldChannelController _worldChannelController;
        private WorldChannelController _playerItemWorldChannelController;
        private MessageWindowController _messageWindowController;

        private GameObject Item_parent;

        private List<string> OnlinePlayerInfoStringList;
        private GameObject _worldChannel;
        private GameObject _playerItem;
        private GameObject _messageWindow;

    }
}