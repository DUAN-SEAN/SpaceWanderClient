using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Common;

namespace Crazy.Main
{

    

    /// <summary>
    /// 飞船仓库逻辑块
    /// </summary>
    public interface ShipHouseLogicBlock
    {
        #region 客户端获取
        /// <summary>
        /// 获取当前的飞船配置
        /// </summary>
        /// <returns></returns>
        PlayerShipInfoDef GetCurrentShipInfo();

        #endregion

        #region 客户端请求

        ///<summary>
        /// 获取当前玩家驾驶的飞船信息
        /// </summary>
        void GetCurrentShipACK();
        /// <summary>
        /// 上传飞船配置
        /// </summary>
        /// <param name="shipInfoDefInfo">飞船信息</param>
        void UploadShipConfigurationACK(PlayerShipInfoDef shipInfoDefInfo);

        #endregion


    }
    /// <summary>
    /// 玩家现场的飞船仓库
    /// </summary>
    public partial class SpacePlayerContext:ShipHouseLogicBlock
    {
        #region 客户端获取

        public PlayerShipInfoDef GetCurrentShipInfo()
        {
            return _currentShipInfoDef;
        }

        #endregion

        #region 客户端请求



        /// <summary>
        /// 获取当前玩家驾驶的飞船信息
        /// </summary>
        public async void GetCurrentShipACK()
        {
            var response = await Call(new C2S_ShipInfoReq { PlayerId = PlayerId }) as S2C_ShipInfoAck;
            _currentShipInfoDef = new PlayerShipInfoDef
            {
                shipId = response.ShipId,
                shipName = response.ShipName,
                shipType = response.ShipType,
                weapon_a = response.WeaponA,
                weapon_b = response.WeaponB
            };
            //通知显式层显示
            PlayerShipInfoCallBack?.Invoke();

        }
        /// <summary>
        /// 上传飞机配置到服务器
        /// </summary>
        /// <param name="shipInfoDefInfo"></param>
        public async void UploadShipConfigurationACK(PlayerShipInfoDef shipInfoDefInfo)
        {
            var response = await Call(new C2S_UpLoadShipInfoReq
            {
                PlayerId = PlayerId,
                ShipId = shipInfoDefInfo.shipId,
                ShipType = shipInfoDefInfo.shipType,
                ShipName = shipInfoDefInfo.shipName,
                WeaponA = shipInfoDefInfo.weapon_a,
                WeaponB = shipInfoDefInfo.weapon_b
            }) as S2C_UpLoadShipInfoAck;

            //通知显式层显示
            UpLoadShipInfoCallBack?.Invoke((int)response.State);

        }

        #endregion

        #region 客户端注册事件
        /// <summary>
        ///  获取飞船配置信息回调
        /// </summary>
        public event Action PlayerShipInfoCallBack;

        /// <summary>
        /// 上传飞船配置回调
        /// 0 = Fail
        /// 1 = OK
        /// </summary>
        public event Action<int> UpLoadShipInfoCallBack;

        #endregion

        #region 字段
        /// <summary>
        /// 当前飞船配置
        /// </summary>
        private PlayerShipInfoDef _currentShipInfoDef;

        #endregion

    }
    /// <summary>
    /// 飞船定义
    /// </summary>
    public class PlayerShipInfoDef
    {
        /// <summary>
        /// 飞船Id
        /// </summary>
        public Int32 shipId;
        /// <summary>
        /// 飞船类型
        /// </summary>
        public Int32 shipType;
        /// <summary>
        /// 飞船名称
        /// </summary>
        public string shipName;
        /// <summary>
        /// 配置武器a
        /// </summary>
        public Int32 weapon_a;
        /// <summary>
        /// 配置武器b
        /// </summary>
        public Int32 weapon_b;
    }
}
