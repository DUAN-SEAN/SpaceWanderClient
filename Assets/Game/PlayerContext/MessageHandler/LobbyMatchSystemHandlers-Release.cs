using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Log = Crazy.ClientNet.Log;
using MongoDB.Bson;
using UnityEngine;

namespace Crazy.Main.Release
{

    /// <summary>
    /// 创建队伍
    /// </summary>
    [MessageHandler]
    public class S2C_CreateMatchTeamCompleteMessageHandler : AMHandler<S2C_CreateMatchTeamComplete>
    {
     
        protected override void Run(ISession playerContext, S2C_CreateMatchTeamComplete message)
        {
            Log.Info("收到一条创建队伍成功");
            try
            {
                SpacePlayerContext pctx = playerContext as SpacePlayerContext;

                pctx.OnCreateMatchTeam(message);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
       

        }
    }

    /// <summary>
    /// 退出队伍,可以是收到自己或者其他人退出
    /// </summary>
    [MessageHandler]
    public class S2CM_ExitMatchTeamCompleteMessageHandler : AMHandler<S2CM_ExitMatchTeamComplete>
    {
        protected override void Run(ISession playerContext, S2CM_ExitMatchTeamComplete message)
        {
            SpacePlayerContext pctx = playerContext as SpacePlayerContext;

            pctx.OnExitMatchTeam(message);

           
        }
    }
    /// <summary>
    /// 加入队伍成功 可以是自己也可以是别人
    /// </summary>
    [MessageHandler]
    public class S2CM_JoinMatchTeamCompleteMessageHandler : AMHandler<S2CM_JoinMatchTeamComplete>
    {
        protected override void Run(ISession playerContext, S2CM_JoinMatchTeamComplete message)
        {
            SpacePlayerContext pctx = playerContext as SpacePlayerContext;

            pctx.OnJoinMatchTeam(message);
           
        }
    }
    /// <summary>
    /// 更新队伍信息
    /// </summary>
    [MessageHandler]
    public class S2C_UpdateMatchTeamInfoMessageHandler : AMHandler<S2C_UpdateMatchTeamInfo>
    {
        /// <summary>
        /// 刷新队伍信息
        /// </summary>
        /// <param name="playerContext"></param>
        /// <param name="message"></param>
        protected override void Run(ISession playerContext, S2C_UpdateMatchTeamInfo message)
        {
            SpacePlayerContext pctx = playerContext as SpacePlayerContext;

            pctx.OnUpdateMatchTeam(message);

           
        }
    }
    /// <summary>
    /// 加入匹配队列成功，队伍里的所有人都会收到这个请求
    /// </summary>
    [MessageHandler]
    public class S2C_JoinMatchQueueCompleteMessageHandler : AMHandler<S2CM_JoinMatchQueueComplete>
    {
        protected override void Run(ISession playerContext, S2CM_JoinMatchQueueComplete message)
        {
            SpacePlayerContext pctx = playerContext as SpacePlayerContext;
            pctx.OnJoinMatchQueue(message);
          
        }
    }
    /// <summary>
    /// 退出匹配队列
    /// </summary>
    [MessageHandler]
    public class S2CM_ExitMatchQueueMessageHandler : AMHandler<S2CM_ExitMatchQueue>
    {
        protected override void Run(ISession playerContext, S2CM_ExitMatchQueue message)
        {
            SpacePlayerContext pctx = playerContext as SpacePlayerContext;
            pctx.OnExitMatchQueue(message);


        }
    }
    [MessageHandler]
    public class S2C_InvitedMatchTeamMessageHandler:AMHandler<S2C_InvitePlayerMatchTeam>
    {
        protected override void Run(ISession playerContext, S2C_InvitePlayerMatchTeam message)
        {
            SpacePlayerContext spacePlayerContext = playerContext as SpacePlayerContext;
            spacePlayerContext.OnInvited(message);
        }
    }

    [MessageHandler]
    public class S2C_SpeakTeamMessageHandler:AMHandler<S2C_SpeakToTeamAck>
    {
        protected override void Run(ISession playerContext, S2C_SpeakToTeamAck message)
        {
            SpacePlayerContext spacePlayerContext = playerContext as SpacePlayerContext;
            spacePlayerContext.OnSpeakInTeam(message);
        }
    }
}
