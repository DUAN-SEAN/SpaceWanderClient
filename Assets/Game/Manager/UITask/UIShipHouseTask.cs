using Crazy.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Manager.UITask
{
    public class UIShipHouseTask : ITaskEventSystem<string>
    {
        public UIShipHouseTask(UIManager um, SpacePlayerContext spc)
        {
            _plx = spc;
            _um = um;
        }

        private void BindEvent()
        {
            _shipHouseController.OnChangeShipButton += OnOpenChangeShipButton;
            _shipHouseController.OnChangeWeapon1Button += OnOpenChangeWeapon1Button;
            _shipHouseController.OnChangeWeapon2Button += OnOpenChangeWeapon2Button;
            _shipHouseController.OnSaveChangesButton += OnSaveChanges;
            _shipHouseController.OnCancelButton += OnCancel;

            _plx.PlayerShipInfoCallBack += OnGetCurrentShipACK;
            _plx.UpLoadShipInfoCallBack += OnUploadShipConfigurationACK;
        }
        
        private void ReleaseEvent()
        {
            _shipHouseController.OnChangeShipButton -= OnOpenChangeShipButton;
            _shipHouseController.OnChangeWeapon1Button -= OnOpenChangeWeapon1Button;
            _shipHouseController.OnChangeWeapon2Button -= OnOpenChangeWeapon2Button;
            _shipHouseController.OnSaveChangesButton -= OnSaveChanges;
            _shipHouseController.OnCancelButton -= OnCancel;

            _plx.PlayerShipInfoCallBack -= OnGetCurrentShipACK;
            _plx.UpLoadShipInfoCallBack -= OnUploadShipConfigurationACK;

        }
        public void Start()
        {
            if (_shipHousePanel == null)
                _shipHousePanel = _um.LoadUIPanelFromResource(UIResourceDefine.ShipHousePanelPath).gameObject;
            if (_messageWindow == null)
                _messageWindow = _um.LoadUIPanelFromResource(UIResourceDefine.MessageWindowPath).gameObject;
            
            _messageWindowController = _messageWindow.GetComponent<MessageWindowController>();

            _shipHouseController = _shipHousePanel.GetComponent<ShipHouseController>();

            _shipHouseController.Open();

            list = new List<string>();
            _playerShipInfoDef = new PlayerShipInfoDef();
            BindEvent();

            _plx.GetCurrentShipACK();
        }

        public void Update()
        {

        }

        public void Dispose()
        {
            _um.TaskDic[typeof(UIMainMenuTask)].Start();

            ReleaseEvent();

            _shipHouseController.Closed();
        }
        
        private void OnCancel()
        {
            Dispose();

        }

        public void OnGetCurrentShipACK()
        {
            _shipHouseController.UpdatePlayerShip(_plx.GetCurrentShipInfo().shipName, WeaponTranslate(_plx.GetCurrentShipInfo().weapon_a), WeaponTranslate(_plx.GetCurrentShipInfo().weapon_b));
        }
        
        public void OnUploadShipConfigurationACK(int i)
        {
            _messageWindowController.Open("保存配置成功!");
        }

        public void OnSaveChanges()
        {
            _playerShipInfoDef.shipId = ShipTranslate(_shipHouseController.gameObject.transform.Find("TextContentGroup/ShipContentText").GetComponent<Text>().text);
            _playerShipInfoDef.shipName = _shipHouseController.gameObject.transform.Find("TextContentGroup/ShipContentText").GetComponent<Text>().text;
            _playerShipInfoDef.shipType = ShipTranslate(_shipHouseController.gameObject.transform.Find("TextContentGroup/ShipContentText").GetComponent<Text>().text);
            _playerShipInfoDef.weapon_a = WeaponTranslate(_shipHouseController.gameObject.transform.Find("TextContentGroup/Weapon1ContentText").GetComponent<Text>().text);
            _playerShipInfoDef.weapon_b = WeaponTranslate(_shipHouseController.gameObject.transform.Find("TextContentGroup/Weapon2ContentText").GetComponent<Text>().text);

            if (_playerShipInfoDef.weapon_a.Equals(_playerShipInfoDef.weapon_b))
            {
                _messageWindowController.Open("保存配置失败，请选择两种不同武器！");
            }
            else
            {
                _plx.UploadShipConfigurationACK(_playerShipInfoDef);
            }
        }

        public void OnOpenChangeShipButton()
        {
            Debug.Log("ShipContent:" + _shipHouseController.transform.Find("TextContentGroup/ShipContentText").GetComponent<Text>().text);
            ShipHouseController._whichBeChosen = "Ship";
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.GameShipConfig.Length; i++)
            {
                GetShipData(i);
            }
            (_um.TaskDic[typeof(UIAdaptedDetailTask)] as UIAdaptedDetailTask).Start(list,"ShipType", GameManager.Instance.gameServerGlobalConfig.GameShipConfig.Length);

            list.Clear();
        }

        public void OnOpenChangeWeapon1Button()
        {
            Debug.Log("ShipContent:" + _shipHouseController.transform.Find("TextContentGroup/Weapon1ContentText").GetComponent<Text>().text);
            ShipHouseController._whichBeChosen = "Weapon1";

            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig.Length; i++)
            {
                GetWeaponData(i);
            }
            (_um.TaskDic[typeof(UIAdaptedDetailTask)] as UIAdaptedDetailTask).Start(list, "SkillType", GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig.Length);

            list.Clear();
        }

        public void OnOpenChangeWeapon2Button()
        {
            Debug.Log("ShipContent:" + _shipHouseController.transform.Find("TextContentGroup/Weapon2ContentText").GetComponent<Text>().text);
            ShipHouseController._whichBeChosen = "Weapon2";

            for (int i=0;i< GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig.Length; i++)
            {
                GetWeaponData(i);
            }
            (_um.TaskDic[typeof(UIAdaptedDetailTask)] as UIAdaptedDetailTask).Start(list, "SkillType", GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig.Length);

            list.Clear();
        }


        public void GetWeaponData(int i)
        {
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].SkillType.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].SkillName.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].CD.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].MaxCount.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].DamageValue.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].DamageDistance.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].DamageRange.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].AttackInterval.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].PhsicsValue.ToString());
        }

        public void GetShipData(int i)
        {
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShipType.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShipName.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].Mass.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].MaxHp.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].MaxShield.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShieldRecoverySpeed.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].AccelerationSpeed.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].MaxAccelerationSpeed.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].MaxSpeed.ToString());
            list.Add(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].MaxTurnSpeed.ToString());
        }

        public string ShipTranslate(int ShipType)
        {
            for(int i=0;i< GameManager.Instance.gameServerGlobalConfig.GameShipConfig.Length; i++)
            {
                if(GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShipType == ShipType)
                {
                    return GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShipName;
                   
                }
            }
            return null;
        }

        public int ShipTranslate(string ShipName)
        {
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.GameShipConfig.Length; i++)
            {
                if (GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShipName == ShipName)
                {
                    return GameManager.Instance.gameServerGlobalConfig.GameShipConfig[i].ShipType;
                    
                }
            }
            return 0;
        }

        public string WeaponTranslate(int SkillType)
        {
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig.Length; i++)
            {
                if (GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].SkillType == SkillType)
                {
                    return GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].SkillName;
                    
                }
            }
            return null;
        }

        public int  WeaponTranslate(string SkillName)
        {
            for (int i = 0; i < GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig.Length; i++)
            {
                if (GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].SkillName == SkillName)
                {
                    return GameManager.Instance.gameServerGlobalConfig.GameSkillConfig.DamageSkillConfig[i].SkillType;
                }
            }
            return 0;
        }

        public void Start(string o)
        {
            throw new NotImplementedException();
        }

        public PlayerShipInfoDef _playerShipInfoDef;

        public List<string> list;

        private SpacePlayerContext _plx;
        private UIManager _um;

        private ShipHouseController _shipHouseController;
        private MessageWindowController _messageWindowController;

        private GameObject _shipHousePanel;
        private GameObject _messageWindow;

    }
}
