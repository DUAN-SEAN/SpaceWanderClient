using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameActorLogic;

namespace Assets.Game
{
    public class ClientLevel:LevelActorBase
    {

        /// <summary>
        /// 只用tick事件处理组件
        /// </summary>
        public override void Update()
        {

            if (isStart == false)
            {
                return;
            }
           //Crazy.ClientNet.Log.Error("start config null" + _configComponent);

            _handlerComponent.Update();
        }
    }
}
