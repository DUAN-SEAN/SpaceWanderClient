using Crazy.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Actor;
using Google.Protobuf;
using MongoDB.Bson;
using Log = Crazy.ClientNet.Log;

namespace Crazy.Main
{
    /// <summary>
    /// 玩家现场的匹配逻辑
    /// </summary>
    public interface IMatchLogicBlockHandler
    {
        #region 客户端请求
        /// <summary>
        /// 邀请玩家加入队伍
        /// </summary>
        /// <param name="Id">玩家Id</param>
        void InvitePlayerInTeam(string Id);
        /// <summary>
        /// 创建匹配队伍
        /// </summary>
        void CreateMatchTeamReq();
        /// <summary>
        /// 加入队伍
        /// </summary>
        void JoinMatchTeamReq(ulong teamId);
        /// <summary>
        /// 退出匹配队伍
        /// </summary>
        void ExitMatchTeamReq();
        /// <summary>
        /// 获取匹配队伍信息
        /// </summary>
        void GetMatchTeamInfoReq();
        /// <summary>
        /// 加入匹配队列
        /// </summary>
        void JoinMatchQueueReq(int barrierId);
        /// <summary>
        /// 退出匹配队列
        /// </summary>
        void ExitMatchQueueReq();
        /// <summary>
        /// 邀请加入队伍
        /// </summary>
        void InviteMatchTeamReq(string aimPlayerId);
        #endregion

        #region 客户端数据获取

        MatchTeam GetMatchTeamInfo();

        /// <summary>
        /// UI 创建队伍回调
        /// </summary>
        event Action<int> CreateTeamCallBack;
        /// <summary>
        /// UI 更新队伍回调
        /// </summary>
        event Action UpdateMatchTeamCallBack;
        /// <summary>
        /// UI 加入匹配队列回调
        /// </summary>
        event Action<int> JoinMatchQueueCallBack;
        /// <summary>
        /// UI 退出匹配队列回调
        /// </summary>
        event Action ExitMatchQueueCallBack;
        /// <summary>
        /// 被邀请回调 2019-9-4
        /// </summary>
        event Action<ulong, string> InvitedTeamCallBack;
        #endregion
    }

    public interface ILobbyLogicBlockHandler
    {
        /// <summary>
        /// 获取在线玩家信息
        /// </summary>
        void GetOnlinePlayerInfoReq();
        /// <summary>
        /// 向队伍里的玩家发送语音
        /// </summary>
        void SendAudioToTeam(AudioBlock audioBlock);
        /// <summary>
        /// 更新在线玩家信息回调
        /// </summary>
        event Action<List<Tuple<string, int>>> UpdateOnlinePlayerInfo;

        event Action<string,AudioBlock> SpeakInTeamCallBack;
    }


    public partial class SpacePlayerContext : IMatchLogicBlockHandler, ILobbyLogicBlockHandler
    {
        #region 客户端请求
        /// <summary>
        /// 邀请玩家加入
        /// </summary>
        /// <param name="Id"></param>
        public void InvitePlayerInTeam(string Id)
        {
            Send(new C2S_InvitePlayerMatchTeam
                {AimPlayerId = Id, LaunchPlayerId = PlayerId, MatchTeamId = CurrentMatchTeamId});
        }

        /// <summary>
        /// 创建匹配队伍
        /// </summary>
        public void CreateMatchTeamReq()
        {
            Send(new C2S_CreateMatchTeam { PlayerId = PlayerId });
        }
        /// <summary>
        /// 加入队伍
        /// </summary>
        public void JoinMatchTeamReq(ulong teamId)
        {
            Send(new C2S_JoinMatchTeam { LaunchPlayerId = PlayerId, MatchTeamId = teamId });
        }
        /// <summary>
        /// 退出匹配队伍
        /// </summary>
        public void ExitMatchTeamReq()
        {
            if (m_currentMatchTeam == null)
            {
                Crazy.ClientNet.Log.Info("没有进入队伍");
                return;
            }
            Send(message: new C2S_ExitMatchTeam { LaunchPlayerId = PlayerId, MatchTeamId = m_currentMatchTeam.Id });
        }
        /// <summary>
        /// 获得匹配队伍的信息
        /// </summary>
        public void GetMatchTeamInfoReq()
        {
            if (m_currentMatchTeam == null)
            {
                Crazy.ClientNet.Log.Info("没有进入队伍");
                return;
            }
            Send(message: new C2S_GetMatchTeamInfo { LaunchPlayerId = PlayerId, MatchTeamId = m_currentMatchTeam.Id });
        }
        /// <summary>
        /// 加入匹配队列
        /// </summary>
        public async void JoinMatchQueueReq(int barrierId)
        {
            
            if (m_currentMatchTeam == null)
            {
                Crazy.ClientNet.Log.Info("没有进入队伍");
                CreateMatchTeamReq();
                await Task.Delay(1000);
            }
            Log.Info("请求匹配关卡 barrierId = "+barrierId);
            Send(message: new C2S_JoinMatchQueue { LaunchPlayerId = PlayerId, MatchTeamId = m_currentMatchTeam.Id, BarrierId = barrierId });
        }
        /// <summary>
        /// 退出匹配队列
        /// </summary>
        public void ExitMatchQueueReq()
        {
            Crazy.ClientNet.Log.Info("发起退出匹配队列");
            Send(new C2S_ExitMatchQueue { LaunchPlayerId = PlayerId, MatchTeamId = m_currentMatchTeam.Id });


        }
        /// <summary>
        /// 邀请加入队伍
        /// </summary>
        public void InviteMatchTeamReq(string aimPlayerId)
        {
            Send(new C2S_InvitePlayerMatchTeam { AimPlayerId = aimPlayerId, LaunchPlayerId = PlayerId, MatchTeamId = m_currentMatchTeam.Id });
        }
        /// <summary>
        /// 获取在线玩家信息
        /// </summary>
        public async void GetOnlinePlayerInfoReq()
        {
            var response = (S2C_UpdateOnlinePlayerList)await Call(new C2S_UpdateOnlinePlayerList {LaunchPlayerId = PlayerId});
            List<Tuple<string, int>> playerInfos  = new List<Tuple<string, int>>();
            foreach (var responseOnlinePlayer in response.OnlinePlayers)
            {
                playerInfos.Add(new Tuple<string, int>(responseOnlinePlayer.PlayerId,responseOnlinePlayer.State));
            }
            // = new Tuple<string, int>(response.OnlinePlayers.Count);
            UpdateOnlinePlayerInfo?.Invoke(playerInfos);
        }
        /// <summary>
        /// 向队伍发送一条语音
        /// </summary>
        /// <param name="audioBlock"></param>
        public void SendAudioToTeam(AudioBlock audioBlock)
        {
            if (CurrentMatchTeam == null) return;
            if (audioBlock == null) return;
            C2S_SpeakToTeamReq req = new C2S_SpeakToTeamReq();
            using (MemoryStream ms = new MemoryStream())
            {
                req.LaunchPlayerId = PlayerId;
                req.MatchTeamId = CurrentMatchTeamId;
                foreach (var member in CurrentMatchTeam.GetMembers())
                {
                    ClientNet.Log.Info("SendAudioToTeam playerId " +member);
                    req.PlayerIds.Add(member);
                }
                m_bf.Serialize(ms, audioBlock);
                req.Data = ByteString.CopyFrom(ms.ToArray());
                
                Send(req);
            }
        }

        /// <summary>
        /// 获取在线玩家列表
        /// </summary>
        public void OnGetOnlinePlayerMessage()
        {
            Send(new C2S_UpdateOnlinePlayerList { LaunchPlayerId = PlayerId });
        }
        //释放队伍
        public void CloseMatchTeam()
        {
            m_currentMatchTeam = null;
        }
        /// <summary>
        /// 玩家现场队伍重新绑定一个队伍
        /// </summary>
        /// <param name="matchTeam"></param>
        public void CreateMatchTeam(MatchTeam matchTeam)
        {
            m_currentMatchTeam = matchTeam;
        }

        #endregion

        #region 客户端获取
        /// <summary>
        /// 客户端获取当前的队伍信息
        /// </summary>
        /// <returns></returns>
        public MatchTeam GetMatchTeamInfo()
        {
            return m_currentMatchTeam;
        }

        #endregion

        #region Handler
        public void OnCreateMatchTeam(S2C_CreateMatchTeamComplete message)
        {
            ClientNet.Log.Info(message.ToJson());
            switch (message.State)
            {
                case S2C_CreateMatchTeamComplete.Types.State.Complete:
                    ClientNet.Log.Info($"创建队伍成功 MatchId = {message.MatchTeamId}");
                    var team = new MatchTeam(message.MatchTeamId);//默认为4
                    CreateMatchTeam(team);
                    team.Add(PlayerId);
                    break;
                case S2C_CreateMatchTeamComplete.Types.State.HaveTeam:
                    ClientNet.Log.Info($"创建队伍失败 已经在队伍中了 MatchId = {message.MatchTeamId}");
                    break;
                case S2C_CreateMatchTeamComplete.Types.State.SystemError:
                    Log.Info($"创建队伍成功 队伍系统错误不允许");
                    break;
                default: break;
            }
            //UI响应
            CreateTeamCallBack?.Invoke((int)message.State);
        }

        public void OnExitMatchTeam(S2CM_ExitMatchTeamComplete message)
        {
            switch (message.State)
            {
                case S2CM_ExitMatchTeamComplete.Types.State.Ok:

                    if (message.MatchTeamId == CurrentMatchTeamId)
                    {
                        if (message.LaunchPlayerId == PlayerId)//是自己
                        {
                            CloseMatchTeam();//关闭匹配队伍
                            //return;
                        }
                        else if (CurrentMatchTeam.IsContain(message.LaunchPlayerId))//是别人
                        {
                            CurrentMatchTeam.Remove(message.LaunchPlayerId);
                        }
                    }
                    break;
                case S2CM_ExitMatchTeamComplete.Types.State.Fail://无论如何 服务器最终都保证发起的玩家都不在服务器队伍中
                    if (CurrentMatchTeam == null) return;
                    else
                    {
                        CloseMatchTeam();
                    }
                    break;
                default: break;
            }
            UpdateMatchTeamCallBack?.Invoke();
        }

        public void OnJoinMatchTeam(S2CM_JoinMatchTeamComplete message)
        {
            switch (message.State)
            {
                case S2CM_JoinMatchTeamComplete.Types.State.Complete:
                    if (CurrentMatchTeam == null)//表示自己加入 因为房间为空
                    {
                        if (PlayerId == message.LaunchPlayerId)//如果是自己发起的 就自己创建一个队伍
                        {
                            var team = new MatchTeam(message.MatchTeamId);
                            CreateMatchTeam(team);
                            CurrentMatchTeam.Add(message.LaunchPlayerId);
                            Log.Info($"进入房间成功 MatchId = {message.MatchTeamId}");
                            //TODO:还要请求服务器获取队伍里其他人的信息
                            GetMatchTeamInfoReq();
                        }
                        else
                        {
                            Log.Info("虽然是我发起的进入房间的，但还是失败了");
                            //错误消息
                        }
                    }
                    else if (message.MatchTeamId == CurrentMatchTeam.Id)//表示是收到其他玩家进入队伍的消息
                    {
                        if (PlayerId != message.LaunchPlayerId)
                        {
                            //如果请求的队伍是本队伍 则加入此玩家到本队伍
                            CurrentMatchTeam.Add(message.LaunchPlayerId);
                            Log.Info($"添加{message.LaunchPlayerId} 进入本队伍");

                        }

                    }
                    else//表示是玩家加入的队伍
                    {

                    }
                    break;
                case S2CM_JoinMatchTeamComplete.Types.State.SystemError:
                    Log.Info("服务器错误");
                    break;
                case S2CM_JoinMatchTeamComplete.Types.State.HaveTeam:
                    Log.Info("我已经在队伍了");
                    break;
                default:
                    break;
            }
            UpdateMatchTeamCallBack?.Invoke();
        }

        public void OnUpdateMatchTeam(S2C_UpdateMatchTeamInfo message)
        {
            MatchTeam matchTeam = CurrentMatchTeam;
            if (matchTeam == null) return;
            lock (matchTeam)
            {
                if (matchTeam.Id == message.MatchTeamId)
                {
                    //1 清空一边集合
                    matchTeam.Clear();
                    foreach (var id in message.TeamInfo.PlayerIds)
                    {
                        matchTeam.Add(id);
                    }
                    //2TODO 重新添加 刷新队伍信息，显示在UI界面上

                }
            }
            UpdateMatchTeamCallBack?.Invoke();
            
        }

        public void OnJoinMatchQueue(S2CM_JoinMatchQueueComplete message)
        {
            switch (message.State)
            {
                case S2CM_JoinMatchQueueComplete.Types.State.Ok:
                    MatchTeam matchTeam = CurrentMatchTeam;
                    if (matchTeam == null) return;
                    if (matchTeam.Id == message.MatchTeamId)
                    {
                        Log.Info("加入匹配队列成功\n进入的关卡Id = " + message.BarrierId);
                        matchTeam.State = MatchTeam.MatchTeamState.Matching;
                    }

                    break;
                case S2CM_JoinMatchQueueComplete.Types.State.Fail:
                    break;
                default:
                    break;
            }
            JoinMatchQueueCallBack?.Invoke((int)message.State);
        }

        public void OnExitMatchQueue(S2CM_ExitMatchQueue message)
        {

            if (CurrentMatchTeam == null)
                return;
            if (CurrentMatchTeam.Id == message.MatchTeamId)//表示需要退出队伍
            {
                Log.Info("退出匹配队列成功\n" + (message.State == S2CM_ExitMatchQueue.Types.State.Client ? "客户发起" : "服务器发起的"));
                CurrentMatchTeam.State = MatchTeam.MatchTeamState.OPEN;
            }
            ExitMatchQueueCallBack?.Invoke();
        }

        public void OnInvited(S2C_InvitePlayerMatchTeam message)
        {
            var teamId = message.MatchTeamId;
            var playerId = message.LaunchPlayerId;
            if (teamId != default&&playerId!=null)
            {
                ClientNet.Log.Info("被邀请回调");
                InvitedTeamCallBack?.Invoke(teamId,playerId);
            }

        }

        public void OnSpeakInTeam(S2C_SpeakToTeamAck message)
        {
            if (CurrentMatchTeamId != message.MatchTeamId)
            {
                ClientNet.Log.Info("不是本队伍的语音 返回");
                return;
            }
            using (MemoryStream ms = new MemoryStream(message.Data.ToByteArray()))
            {

                var audioBlock = m_bf.Deserialize(ms) as AudioBlock;
                
                SpeakInTeamCallBack?.Invoke(message.LaunchPlayerId,audioBlock);
            }

        }
        #endregion

        #region 客户端事件
        /// <summary>
        /// UI 创建队伍回调
        /// </summary>
        public event Action<int> CreateTeamCallBack;
        /// <summary>
        /// UI 更新队伍回调
        /// </summary>
        public event Action UpdateMatchTeamCallBack;
        /// <summary>
        /// UI 加入匹配队列回调
        /// </summary>
        public event Action<int> JoinMatchQueueCallBack;
        /// <summary>
        /// UI 退出匹配队列回调
        /// </summary>
        public event Action ExitMatchQueueCallBack;
        /// <summary>
        /// 被邀请回调 2019-9-4
        /// </summary>
        public event Action<ulong,string> InvitedTeamCallBack;
        /// <summary>
        /// 更新在线玩家信息回调
        /// </summary>
        public event Action<List<Tuple<string, int>>> UpdateOnlinePlayerInfo;

        /// <summary>
        /// 队伍语音回调
        /// </summary>
        public event Action<string, AudioBlock> SpeakInTeamCallBack;

        #endregion

        #region 字段

        /// <summary>
        /// 当前队伍的Id
        /// </summary>
        private MatchTeam m_currentMatchTeam;

        #endregion

        #region 属性

        public ulong CurrentMatchTeamId { get => m_currentMatchTeam.Id; }
        public MatchTeam CurrentMatchTeam { get => m_currentMatchTeam; }

        #endregion


       
    }


}
