using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Manager.BattleTask.Controller
{
    public class GPSController:MonoBehaviour
    {
        private List<Vector3> _targets;
        /// <summary>
        /// 导航脚本
        /// </summary>
        /// <param name="targets"></param>
        public void CheckPosition(List<Vector3> targets)
        {
            _targets = targets;
        }
        private void OnDrawGizmos()
        {
            if (_targets == null) return;
            Gizmos.color = Color.red;
            foreach (var pos in _targets)
            {
                Gizmos.DrawLine(transform.position,pos);
            }
         
            //Gizmos.DrawRay(Camera.main.ScreenToWorldPoint(start), Vector3.left*10);

        }
    }
}
