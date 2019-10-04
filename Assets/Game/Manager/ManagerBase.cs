using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crazy.Main;

namespace Assets.Game.Manager
{
    /// <summary>
    /// 管理基类
    /// </summary>
    public abstract class ManagerBase:IManagerContext
    {
        /// <summary>
        /// 要求不能被实例化
        /// </summary>
        /// <param name="gm"></param>
        protected ManagerBase(GameManager gm)
        {
            m_gameManager = gm;
        }
        public virtual void Initialize()
        {
        }

        public virtual void BindSpacePlayerContext(SpacePlayerContext spacePlayerContext)
        {
            m_currentPlayerCtx = spacePlayerContext;
        }

        public virtual void ReleaseSpacePlayerContext()
        {
            m_currentPlayerCtx = null;
        }
        /// <summary>
        /// 当前玩家现场
        /// </summary>
        protected SpacePlayerContext m_currentPlayerCtx;
        /// <summary>
        /// 当前游戏主管理
        /// </summary>
        protected GameManager m_gameManager;
        /// <summary>
        /// 玩家现场Get属性
        /// </summary>
        public SpacePlayerContext PlayerContext
        {
            get => m_currentPlayerCtx;
        }
    }
}
