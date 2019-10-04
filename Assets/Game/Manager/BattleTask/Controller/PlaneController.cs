using GameActorLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DSharp.External;
using UnityEngine;

namespace Assets.Game.Manager.BattleTask.Controller
{
    public class PlaneController : MonoBehaviour
    {
        private ActorBase ship;
        void Start()
        {

        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, BattleHelper.Point2Vector3(ship.GetPosition()), Time.deltaTime * 100f);
            transform.forward = Vector3.MoveTowards(transform.forward, BattleHelper.Point2Vector3(((IBaseComponentContainer)ship).GetPhysicalinternalBase().GetBody().GetForward()), Time.deltaTime * 30f);
        }
        public void Move(ActorBase ship)
        {
            this.ship = ship;
        }


    }
}
