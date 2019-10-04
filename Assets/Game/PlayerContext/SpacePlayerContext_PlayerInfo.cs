using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.ClientNet;
using Crazy.Common;
using Crazy.Main;
using GameServer.Configure;
using Log = Crazy.ClientNet.Log;

namespace Crazy.Main
{
    public partial class SpacePlayerContext:PlayerContextBase
    {
       

        public SpacePlayerContext(Client client, GameServerGlobalConfig mGameServerGlobalConfig) : base(client)
        {
            m_serverGlobalConfig = mGameServerGlobalConfig;
        }

        public override void OnMessage(KeyValuePair<int, object> keyValuePair)
        {
            base.OnMessage(keyValuePair);
            


        }
        /// <summary>
        /// 玩家登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public async void Login(string account, string password)
        {
            Log.Info(account+":"+password);
            var response = (S2C_LoginMessage)await Call(new C2S_LoginMessage() { Account = account, Password = password });
            if (response.State == S2C_LoginMessage.Types.State.Ok)
            {
                m_playerId = response.PlayerGameId;
                
            }

            if(OnLoginCallBack==null) Log.Info("回调为NULL");

            OnLoginCallBack?.Invoke((int)response.State);
           


        }
        /// <summary>
        /// 玩家注册
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public async void SignUp(string account, string password)
        {
            var response = (S2C_RegisterMessage)await Call(new C2S_RegisterMessage { Account = account, Password = password });

            //S2C_RegisterMessage.Types.State.
            OnSignUpCallBack?.Invoke((int)response.State);
        }


        #region 客户端事件

        public event Action<int> OnLoginCallBack;

        public event Action<int> OnSignUpCallBack;


        #endregion

        #region 字段
        private GameServerGlobalConfig m_serverGlobalConfig;
        #endregion


    }
}
