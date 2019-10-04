using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Manager.BattleTask
{
    static class BattleHelper
    {
        internal static UnityEngine.Vector3 Point2Vector3(Vector2 position)
        {
            return new UnityEngine.Vector3(position.X, 0, position.Y);
        }

        //internal static Vector3 Double2Vector3(double forward)
        //{
        //    return ;
        //}
    }
}
