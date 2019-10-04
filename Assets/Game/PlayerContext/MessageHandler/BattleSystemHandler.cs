using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;
using Log = Crazy.ClientNet.Log;
using MongoDB.Bson;
using Crazy.ClientNet;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GameActorLogic;

namespace Crazy.Main
{
    /// <summary>
    /// old
    /// </summary>
    [MessageHandler]
    public class S2C_BodyInitBattleMessageHandler : AMHandler<S2C_BodyInitBattleMessage>
    {
        private static BinaryFormatter formatter = new BinaryFormatter();
        protected override void Run(ISession playerContext, S2C_BodyInitBattleMessage message)
        {
            
            var type = message.BodyType;
            
            
        }
    }
    /// <summary>
    /// 服务器发来开启战斗关卡Ack，上接匹配队伍成功
    /// </summary>
    [MessageHandler]
    public class S2C_CreateBarrierBattleMessageHandler:AMHandler<S2CM_CreateBattleBarrier>
    {
        /// <summary>
        /// 接受创建关卡战斗消息
        /// </summary>
        /// <param name="playerContext"></param>
        /// <param name="message"></param>
        protected override void Run(ISession playerContext, S2CM_CreateBattleBarrier message)
        {
            SpacePlayerContext plx = playerContext as SpacePlayerContext;
            if (plx == null) return;
            var battleId = message.BattleId;
            var battleInfo =  message.BattleInfo;
            var players = battleInfo.PlayerIds;
            plx.OnCreateBattle(battleId,players);
        }
    }
    /// <summary>
    /// 战斗事件，actor of eventMessage
    /// </summary>
    [MessageHandler]
    public class S2C_EventBattleMessageHandler :AMHandler<S2C_EventBattleMessage> {

        private static BinaryFormatter formatter = new BinaryFormatter();
        /// <summary>
        /// 接受服务器来的事件消息
        /// </summary>
        /// <param name="playerContext"></param>
        /// <param name="message"></param>
        protected override void Run(ISession playerContext, S2C_EventBattleMessage message)
        {
            SpacePlayerContext plx = playerContext as SpacePlayerContext;
            MemoryStream memoryStream = new MemoryStream(message.Event.ToByteArray());
            var eventMessage =  formatter.Deserialize(memoryStream) as IEventMessage;//反序列化一个eventMessage，然后分发给Actor的事件处理分发组件
            if(eventMessage!=null) plx.OnEventHandle(eventMessage);
            else
            {
                ClientNet.Log.Info("事件解析后为NULL");
            }
            memoryStream.Close();
            message.Event = null;

        }
    }
    /// <summary>
    /// 同步血量护盾
    /// </summary>
    [MessageHandler]
    public class S2C_SyncHpShieldStateBattleMessageHandler:AMHandler<S2C_SyncHpShieldStateBattleMessage>
    {
        /// <summary>
        /// 飞船HP和护盾值属性同步
        /// </summary>
        /// <param name="playerContext"></param>
        /// <param name="message"></param>
        protected override void Run(ISession playerContext, S2C_SyncHpShieldStateBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            if (ctx.CurrentBattleId == message.BattleId)
            {
                ctx.OnSyncHealthShield(message.ActorId,message.Hp,message.Shield);
            }
        }
    }
    /// <summary>
    /// 同步物理状态
    /// </summary>
    [MessageHandler]
    public class S2C_SyncPhysicsStateBattleMessageHandler:AMHandler<S2C_SyncPhysicsStateBattleMessage>
    {
        /// <summary>
        /// 物理状态组件同步
        /// </summary>
        /// <param name="playerContext"></param>
        /// <param name="message"></param>
        protected override void Run(ISession playerContext, S2C_SyncPhysicsStateBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            if(ctx.CurrentBattleId== message.BattleId)
            {
                ctx.OnSyncPhysicState(message.ActorId, message.AngleVelocity, message.ForceX, message.ForceY, message.ForwardAngle, message.PositionX, message.PositionY,message.PositionPrevX,message.PositionPrevY, message.VelocityX, message.VelocityY, message.Torque);
                //ctx.OnSyncPhysicState(message.ActorId,message.AngleVelocity,message.ForceX,message.ForceY,message.ForwardAngle,message.PositionX,message.PositionY,message.VelocityX,message.VelocityY,message.Torque);
            }
        }
    }
    /// <summary>
    /// 同步关卡任务进度
    /// </summary>
    [MessageHandler]
    public class S2C_SyncLevelTaskStateMessageHandler:AMHandler<S2C_SyncLevelTaskBattleMessage>
    {
        protected override void Run(ISession playerContext, S2C_SyncLevelTaskBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            if (ctx.CurrentBattleId == message.BattleId)
            {
                ctx.OnSyncLevelTaskState(message);
            }
        }
    }
    /// <summary>
    /// 同步玩家技能状态
    /// </summary>
    [MessageHandler]
    public class S2C_SyncSkillStateBattleMessageHandler:AMHandler<S2C_SyncSkillStateBattleMessage>
    {
        protected override void Run(ISession playerContext, S2C_SyncSkillStateBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            if (ctx.CurrentBattleId == message.BattleId)
            {
                ctx.OnSyncSkillState(message);
            }
        }
    }
    [MessageHandler]
    public class S2C_SyncLevelStateBattleMessageHandler:AMHandler<S2C_SyncLevelStateBattleMessage>
    {
        protected override void Run(ISession playerContext, S2C_SyncLevelStateBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            ctx.OnSyncLevelState(message);
        }
    }

    /// <summary>
    /// 战斗启动第三次握手，客户端可以开始处理战斗事件
    /// </summary>
    [MessageHandler]
    public class S2CM_ReadyBattleAckMessageHandler:AMHandler<S2CM_ReadyBattleBarrierAck>
    {
        protected override void Run(ISession playerContext, S2CM_ReadyBattleBarrierAck message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            ClientNet.Log.Info("S2CM_ReadyBattleAckMessageHandler");
            ctx.OnReadyBattleAck();
        }
    }

    [MessageHandler]
    public class S2CM_ExitBattleMessageHandler:AMHandler<S2C_ExitBattleMessage>
    {
        protected override void Run(ISession playerContext, S2C_ExitBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            ctx.OnExitBattle((int)message.State,message.PlayerId);
        }
    }
    [MessageHandler]
    public class S2CM_FinishBattleMessageHandler:AMHandler<S2CM_FinishBattleMessage>
    {
        protected override void Run(ISession playerContext, S2CM_FinishBattleMessage message)
        {
            SpacePlayerContext ctx = playerContext as SpacePlayerContext;
            ctx.OnFinishBattle(message.BattleId, message.Result);
        }
    }
}
