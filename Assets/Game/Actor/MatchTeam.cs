using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy.Main
{
    /// <summary>
    /// 匹配队伍的类型，用于保留一份队伍Id名单和队伍最大容量
    /// 后期匹配队伍可能需要状态机控制，但目前不使用状态机，简单。
    /// 匹配队伍目前的状态为线性态
    /// 注意：该类型的实例不是线程安全，出现异常现状优先检测它
    /// </summary>
    public class MatchTeam
    {
        public MatchTeam(UInt64 id, int maxCount = 4)
        {
            Id = id;
            State = MatchTeamState.OPEN;
            m_maxCount = maxCount;
        }

        public void Add(string playerId)
        {
            lock (Member)
            {
                Member.Add(playerId);
            }
        }

        public bool Remove(string playerId)
        {
            lock (Member)
            {
                if (Member.Remove(playerId))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 验证队伍是否满员
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            if (CurrentCount >= m_maxCount)
                return false;
            return true;
        }
        /// <summary>
        /// 判断玩家是否在队伍中
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public bool IsContain(string playerId)
        {
            lock (Member)
            {
                return Member.Contains(playerId);
            }

        }
        public string FindPlayer(string playerId)
        {
            lock (Member)
            {
                return Member.Contains(playerId) ? playerId : null;
            }

        }
        /// <summary>
        /// 获取队长的id
        /// </summary>
        /// <returns></returns>
        public string GetCaptainId()
        {
            lock (Member)
            {
                return Member.First();
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>获取队伍</returns>
        public List<string> GetMembers()
        {
            lock (Member)
            {
                return Member.ToList();
            }

        }
        public void Clear()
        {
            Member.Clear();
        }
        public UInt64 Id { get; protected set; }//表示队伍的唯一Id表示，不可被更改

        public int CurrentCount { get => Member.Count; }//当前队伍人数

        private int m_maxCount;//队伍最大限制人数

        /// <summary>
        /// 房间玩家列表
        /// </summary>
        private readonly HashSet<string> Member = new HashSet<string>();

        public MatchTeamState State { get; set; }
        public enum MatchTeamState
        {
            OPEN,//开放房间
            CLOSE,//关闭房间
            INBATTLE,//在战斗中
            Matching,//在匹配中
        }
    }

    
}
