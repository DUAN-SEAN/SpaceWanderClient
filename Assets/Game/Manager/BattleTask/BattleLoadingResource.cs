using GameActorLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Assets.Game.Manager.BattleTask
{

    public static class BattleLoadingResource
    {
        private static GameObject gameObject;
        public static GameObject LoadingResource(ActorBase actor,int ActorType,bool isDestroy)
        {
            if(actor.IsEnvir())
            {
                if(!isDestroy)
                switch (ActorType)
                {
                    case ActorTypeBaseDefine.Meteorite_S:
                        gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Asteroid_01_Path)) as GameObject;
                        break;
                    case ActorTypeBaseDefine.Meteorite_M:
                        gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Asteroid_02_Path)) as GameObject;
                        break;
                    case ActorTypeBaseDefine.Meteorite_L:
                        gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Asteroid_03_Path)) as GameObject;
                        break;
                    case ActorTypeBaseDefine.BlackHole:
                        gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Blackhole_Path)) as GameObject;
                        break;
                    default:
                        Debug.Log("无法加载环境资源");
                        break;
                }
                
            }
            else if(actor.IsShip())
            {
                if(!isDestroy)
                    switch (ActorType)
                    {
                        case ActorTypeBaseDefine.ShipActorNone:
                            break;
                        case ActorTypeBaseDefine.PlayerShipActor:
                            break;
                        case ActorTypeBaseDefine.WaspShipActorA:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Ship_Fighter_Heavy_01_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.FighterShipActorA:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Ship_Fighter_01_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.FighterShipActorB:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Ship_Fighter_02_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.DroneShipActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Veh_Drone_Attach_01_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.AnnihilationShipActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Veh_Drone_Repair_01_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.EliteShipActorA:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Ship_Galactic_Carrier_01_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.EliteShipActorB:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Ship_Cruiser_01_Path)) as GameObject;
                            break;
                        default:
                            Debug.LogError("无法加载飞机资源");
                            break;
                    }
                else
                    gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.explosion_player_Path)) as GameObject;
            }
            else if(actor.IsWeapon())
            {
                if(!isDestroy)
                    switch (ActorType)
                    {
                        case ActorTypeBaseDefine.MachineGunActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Bolt_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.AntiAircraftGunActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Sologun_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TorpedoActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Missile_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TrackingMissileActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Guided_Missile_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.ContinuousLaserActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Light_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.PowerLaserActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Laser_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TimeBombActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Timing_Mine_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TriggerBombActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Trigger_Mine_Path)) as GameObject;
                            break;
                        default:
                            Debug.LogError("无法加载武器资源");
                            break;
                    }
                else
                    switch (ActorType)
                    {

                        case ActorTypeBaseDefine.MachineGunActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.AntiAircraftGunActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TorpedoActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TrackingMissileActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.ContinuousLaserActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.PowerLaserActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TimeBombActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        case ActorTypeBaseDefine.TriggerBombActor:
                            gameObject = Object.Instantiate(Resources.Load(BattleResourceDefine.Explosion_001_Path)) as GameObject;
                            break;
                        default:
                            Debug.LogError("无法加载武器爆炸效果资源");
                            break;
                    }

            }
            else
            {
                Debug.Log("找不到类型对应的资源");

            }
            
            

            return gameObject;
        }
    }
    
}
