using System;
using Crazy.Main;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Assets.Game.Manager.BattleTask;
using GameActorLogic;
using GameServer.Configure;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Vector2 = System.Numerics.Vector2;

namespace Assets.Game.Manager.UITask
{
    public class UIInBattleTask : ITaskEventSystem
    {
        private class ArrowInfo
        {
            private bool _isHidden;
            private GameObject _arrowImage;

            public GameObject ArrowImage
            {
                get { return _arrowImage; }
                set { _arrowImage = value; }
            }

            public bool IsHidden
            {
                get { return _isHidden; }
                set { _isHidden = value; }
            }

            public ArrowInfo(GameObject image)
            {
                _arrowImage = image;
                _isHidden = true;
            }

            public void ShowArrow()
            {
                _arrowImage.GetComponent<Image>().enabled = true;
            }

            public void HideArrow()
            {
                _arrowImage.GetComponent<Image>().enabled = false;
            }
        }

        private bool m_state;
        private SpacePlayerContext m_ctx;
        private readonly BattleManager m_battleManager;
        private GameObject m_inBattlePanel;
        private InBattleController m_inBattleController;
        private Dictionary<ulong, GameObject> m_actorDic;

        private ETCJoystick m_joystick;
        private Slider m_sliderPlayerHp;
        private Text m_playerName;
        //private Text m_teammates;
        private GameObject m_teammates;
        private GameObject m_prefabTeammate;
        private Dictionary<string, GameObject> m_dictTeam;
        private Text m_tasks;

        private Transform m_arrowContainer;
        private GameObject m_prefabArrow;
        private Dictionary<ulong, ArrowInfo> m_dictArrows = new Dictionary<ulong, ArrowInfo>();

        private List<string> m_originalTeammates = new List<string>();
        private List<ActorBase> m_currentEnemies = new List<ActorBase>();

        private float m_screenWidth;
        private float m_screenHeight;
        private Camera m_camera;

        public UIInBattleTask(BattleManager manager, SpacePlayerContext ctx)
        {
            m_battleManager = manager;
            m_ctx = ctx;
            m_state = false;

            m_screenWidth = Screen.width;
            m_screenHeight = Screen.height;
        }

        public void Start()
        {
            if (m_inBattlePanel == null)
            {
                m_inBattlePanel = m_battleManager.LoadUIPanelFromResource
                    (UIResourceDefine.InBattlePanelPath).gameObject;
            }

            BindReferences();
            BindEvent();

            m_state = false;

            m_ctx = GameManager.Instance.CurrentPlayerContext;
            m_dictTeam = new Dictionary<string, GameObject>();
            m_dictArrows = new Dictionary<ulong, ArrowInfo>();
            m_originalTeammates = new List<string>();
            m_currentEnemies = new List<ActorBase>();
            m_screenWidth = Screen.width;
            m_screenHeight = Screen.height;
        }

        private void BindReferences()
        {
            m_inBattleController = m_inBattlePanel.GetComponent<InBattleController>();
            Transform controller = m_inBattleController.transform;

            m_playerName = controller.Find("TextPlayerName").gameObject.GetComponent<Text>();
            m_joystick = controller.Find("JoystickRudder").gameObject.GetComponent<ETCJoystick>();
            m_sliderPlayerHp = controller.Find("SliderHp").gameObject.GetComponent<Slider>();
            //m_teammates = controller.Find("TextTeam").gameObject.GetComponent<Text>();
            m_teammates = controller.Find("Teammates").gameObject;
            m_teammates.SetActive(false);
            m_prefabTeammate = Resources.Load<GameObject>("Teammate");
            m_dictTeam = new Dictionary<string, GameObject>();
            m_tasks = controller.Find("TextTasks").gameObject.GetComponent<Text>();

            m_arrowContainer = controller.Find("ArrowContainer");
            m_prefabArrow = (GameObject)Resources.Load("Arrow", typeof(GameObject));

            m_camera = GameObject.FindWithTag("SpaceCamera").GetComponent<Camera>();
        }

        public void Update()
        {
            if (!m_state)
            {
                return;
            }

            if (m_originalTeammates.Count == 0)
            {
                foreach (var member in m_ctx.CurrentMatchTeam.GetMembers())
                {
                    m_originalTeammates.Add(member);
                }
            }

            if (m_actorDic == null)
            {
                return;
            }

            UpdateUIElements();
            UpdateArrows();
            //UpdateMarks();
        }

        public void Dispose()
        {
            ReleaseEvent();
        }

        private void BindEvent()
        {
            m_inBattleController.UsingJoystrick += JoystickMoving;
            m_inBattleController.OnMainWeaponBtn += OnMainWeapon;
            m_inBattleController.UpMainWeaponBtn += UpMainWeapon;
            m_inBattleController.OnSubWeaponBtn += OnSubWeapon;
            m_inBattleController.UpMainWeaponBtn += UpSubWeapon;

            m_ctx.m_CurrentLevel.OnInitMessageHandler += AddToCurrentEnemies;
            m_ctx.m_CurrentLevel.OnDestroyMessageHandler += RemoveFromCurrentEnemies;

            //m_ctx.m_CurrentLevel.OnInitMessageHandler += LineAttachedToNewEnemy;
            m_ctx.m_CurrentLevel.OnInitMessageHandler += InstantiateArrowAndHide;
        }

        private void ReleaseEvent()
        {
            m_inBattleController.UsingJoystrick -= JoystickMoving;
            m_inBattleController.OnMainWeaponBtn -= OnMainWeapon;
            m_inBattleController.UpMainWeaponBtn -= UpMainWeapon;
            m_inBattleController.OnSubWeaponBtn -= OnSubWeapon;
            m_inBattleController.UpMainWeaponBtn -= UpSubWeapon;

            m_ctx.m_CurrentLevel.OnInitMessageHandler -= AddToCurrentEnemies;
            m_ctx.m_CurrentLevel.OnDestroyMessageHandler -= RemoveFromCurrentEnemies;

            //m_ctx.m_CurrentLevel.OnInitMessageHandler -= LineAttachedToNewEnemy;
            m_ctx.m_CurrentLevel.OnInitMessageHandler -= InstantiateArrowAndHide;
        }

        #region Controller Callbacks

        private void JoystickMoving()
        {
            var actor = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            if (actor == null) return;

            ulong actorid = actor.GetActorID();
            float x = -m_joystick.axisX.axisValue;
            float y = m_joystick.axisY.axisValue;

            var joystickCommand = new RemoteCommand(actorid, x, y);
            m_ctx.SendCommandReq(joystickCommand);
        }

        private void UpMainWeapon()
        {
            var actor = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            if (actor == null) return;

            ulong actorid = actor.GetActorID();
            int skillType = m_ctx.GetCurrentShipInfo().weapon_a;

            var mainWeaponCommand = new SkillCommand(actorid, skillType, 1);
            m_ctx.SendCommandReq(mainWeaponCommand);
        }

        private void UpSubWeapon()
        {
            var actor = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            if (actor == null) return;

            ulong actorid = actor.GetActorID();
            int skillType = m_ctx.GetCurrentShipInfo().weapon_b;

            var mainWeaponCommand = new SkillCommand(actorid, skillType, 1);
            m_ctx.SendCommandReq(mainWeaponCommand);
        }

        private void OnMainWeapon()
        {
            var actor = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            if (actor == null) return;

            ulong actorid = actor.GetActorID();
            int skillType = m_ctx.GetCurrentShipInfo().weapon_a;

            var mainWeaponCommand = new SkillCommand(actorid, skillType, 0);
            m_ctx.SendCommandReq(mainWeaponCommand);
        }

        private void OnSubWeapon()
        {
            var actor = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            if (actor == null) return;

            ulong actorid = m_ctx.GetPlayerShipActor(m_ctx.PlayerId).GetActorID();
            var weapon_b = m_ctx.GetCurrentShipInfo().weapon_b;

            var subWeaponCommand = new SkillCommand(actorid, weapon_b, 0);
            m_ctx.SendCommandReq(subWeaponCommand);
        }

        #endregion

        #region text processing

        /// <summary>
        /// 更新所有 UI 元素
        /// </summary>
        private void UpdateUIElements()
        {
            TryUpdatePlayerNameText();
            //UpdateTeammatesText();
            UpdateTeammatesUI();
            UpdateTasksText();
        }

        /// <summary>
        /// 只在需要的时候更新玩家名字
        /// </summary>
        private void TryUpdatePlayerNameText()
        {
            if (m_playerName.text == "Player Name")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<color=silver>");
                sb.Append(m_ctx.PlayerId);
                sb.Append("</color>");
                m_playerName.text = sb.ToString();
            }
        }

        /// <summary>
        /// 更新队员文本
        /// </summary>
        //private void UpdateTeammatesText()
        //{
        //    if (m_originalTeammates.Count == 1)
        //    {
        //        m_teammates.text = "";
        //        return;
        //    }

        //    var sb = new StringBuilder();
        //    sb.Append("<size=25>Team member(s): </size>");
        //    sb.Append(Environment.NewLine);
        //    foreach (string member in m_originalTeammates)
        //    {
        //        if (member == m_ctx.PlayerId)
        //        {
        //            continue;
        //        }

        //        if (!m_ctx.CurrentMatchTeam.IsContain(member))
        //        {
        //            sb.Append("<color=grey>");
        //            sb.Append(member);
        //            sb.Append("</color>");
        //            sb.Append(Environment.NewLine);
        //        }
        //        else
        //        {
        //            sb.Append(member);
        //            sb.Append(Environment.NewLine);
        //        }
        //    }

        //    if (sb.Length > 0)
        //        sb.Length -= Environment.NewLine.Length;

        //    m_teammates.text = sb.ToString();
        //}

        /// <summary>
        /// 更新队员信息 UI
        /// </summary>
        private void UpdateTeammatesUI()
        {
            if (m_originalTeammates.Count == 1)
            {
                return;
            }

            if (m_dictTeam == null)
            {
                InitTeamUI();
            }

            for (int memberIndex = 0; memberIndex < m_dictTeam.Count; memberIndex++)
            {
                string member = m_originalTeammates[memberIndex];
                if (!m_ctx.CurrentMatchTeam.IsContain(member))
                {
                    m_dictTeam[member].transform.Find("SliderHp").gameObject.SetActive(false);
                }
                else
                {
                    GameObject hp = m_dictTeam[member].transform.Find("SliderHp").gameObject;
                    Slider sliderHp = hp.GetComponent<Slider>();
                    //sliderHp.value = 
                }
            }
        }

        /// <summary>
        /// 生成与成员（除玩家之外）的姓名和血量 UI
        /// </summary>
        private void InitTeamUI()
        {
            m_teammates.SetActive(true);
            List<string> myMates = new List<string>();

            foreach (string teammate in m_originalTeammates)
            {
                if (teammate == m_ctx.PlayerId)
                {
                    continue;
                }
                myMates.Add(teammate);
            }

            for (int memberIndex = 0; memberIndex < m_originalTeammates.Count - 1; memberIndex++)
            {
                GameObject teammate = GameObject.Instantiate(m_prefabTeammate);
                teammate.transform.parent = m_teammates.transform;
                m_dictTeam[myMates[0]] = teammate;
            }
        }

        /// <summary>
        /// 更新玩家的血条显示
        /// </summary>
        private void UpdatePlayerHp()
        {
            ActorBase playerActor = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            m_sliderPlayerHp.value = 0.5f;
        }

        /// <summary>
        /// 更新任务文本
        /// </summary>
        private void UpdateTasksText()
        {
            List<ITaskEvent> currentTasks = m_ctx.GetCurrentTask();
            StringBuilder sb = new StringBuilder();

            foreach (var task in currentTasks)
            {
                if (task.GetTaskConditionTypeDefine() == 1003)
                {
                    switch (task.GetTaskState())
                    {
                        case TaskEventState.Finished:
                            sb.Append("<color=grey>");
                            sb.Append(task.GetTaskDescription());
                            sb.Append(": ");
                            sb.Append(task.GetCurrentValue());
                            sb.Append(" / ");
                            sb.Append(task.GetTargetValue());
                            sb.Append("</color>");
                            sb.Append(Environment.NewLine);
                            break;
                        case TaskEventState.UnFinished:
                            sb.Append("<color=white>");
                            sb.Append(task.GetTaskDescription());
                            sb.Append(": ");
                            sb.Append(task.GetCurrentValue());
                            sb.Append(" / ");
                            sb.Append(task.GetTargetValue());
                            sb.Append("</color>");
                            sb.Append(Environment.NewLine);
                            break;
                    }
                }
            }

            if (sb.Length > 0)
                sb.Length -= Environment.NewLine.Length;

            m_tasks.text = sb.ToString();
        }

        #endregion

        #region 雷达标记(image)

        /// <summary>
        /// 在新敌机出现的时候加入字典
        /// </summary>
        /// <param name="actorId"></param>
        private void InstantiateArrowAndHide(ulong actorId)
        {
            if (m_actorDic == null) return;

            ActorBase newborn = m_ctx.GetActorBase(actorId);

            if (newborn.IsShip() && !newborn.IsPlayer())
            {
                GameObject arrow = GameObject.Instantiate(m_prefabArrow);
                ArrowInfo ai = new ArrowInfo(arrow);

                ai.ArrowImage.transform.SetParent(m_arrowContainer);
                Vector3 parentPosition = ai.ArrowImage.transform.parent.position;
                ai.ArrowImage.transform.position = m_camera.WorldToScreenPoint(parentPosition);
                ai.HideArrow();
                ai.IsHidden = true;
                m_dictArrows[actorId] = ai;
            }
        }

        /// <summary>
        /// 更新箭头们的位置
        /// </summary>
        private void UpdateArrows()
        {
            foreach (var enemy in m_currentEnemies)
            {
                if (CanBeSeen(enemy))
                {
                    //m_dictArrows[enemy.GetActorID()].ArrowImage.transform.position =
                    //    m_actorDic[enemy.GetActorID()].transform.position;
                    m_dictArrows[enemy.GetActorID()].HideArrow();
                    m_dictArrows[enemy.GetActorID()].IsHidden = true;
                }
                else
                {
                    SetAnArrow(enemy.GetActorID());
                }
            }
        }

        /// <summary>
        /// 屏幕外的船显示
        /// </summary>
        /// <param name="actorId">敌机 id</param>
        private void SetAnArrow(ulong actorId)
        {
        
            Vector3 realPos = GetIntersectionP(GetRealEnemyScreenPos(actorId), GetRealPlayerScreenPos());
            realPos += (GetRealPlayerScreenPos() - GetRealEnemyScreenPos(actorId)).normalized * 50f;
            Quaternion realRotation = GetRealRotation(actorId);

            Transform arrowTrans = m_dictArrows[actorId].ArrowImage.transform;
            switch (m_dictArrows[actorId].IsHidden)
            {
                case true:
                    arrowTrans.position = realPos;
                    arrowTrans.rotation = realRotation;
                    break;
                case false:
                    //arrowTrans.position =Vector3.Lerp(arrowTrans.position, realPos, 0.5f);
                    //arrowTrans.rotation =Quaternion.Slerp(arrowTrans.rotation, realRotation, 0.5f);
                    arrowTrans.rotation = realRotation;
                    arrowTrans.position = realPos;
                    break;
            }

            m_dictArrows[actorId].ShowArrow();
            m_dictArrows[actorId].IsHidden = false;
        }

        /// <summary>
        /// 返回目前箭头的 rotation
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        private Quaternion GetCurrentRotation(ulong actorId)
        {
            ArrowInfo ai = m_dictArrows[actorId];
            GameObject arrow = ai.ArrowImage;

            return arrow.transform.rotation;
        }

        /// <summary>
        /// 返回目前箭头的 position
        /// </summary>
        /// <param name="actorId">敌机的 id</param>
        /// <returns>当前箭头的屏幕 position</returns>
        private Vector3 GetCurrentScreenPos(ulong actorId)
        {
            ArrowInfo ai = m_dictArrows[actorId];
            GameObject arrow = ai.ArrowImage;

            Vector3 arrowCurrentWorldPos = arrow.transform.position;
            return m_camera.WorldToScreenPoint(arrowCurrentWorldPos);
        }

        /// <summary>
        /// 获得某敌机和玩家之间的真实距离
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        private float GetRealDistance(ulong actorId)
        {
            Vector3 playerRealScreenPos = GetRealPlayerScreenPos();
            Vector3 enemyRealScreenPos = GetRealEnemyScreenPos(actorId);

            return (playerRealScreenPos - enemyRealScreenPos).magnitude;
        }

        /// <summary>
        /// 返回某箭头实时的旋转
        /// </summary>
        /// <param name="actorId">敌机的 id</param>
        /// <returns>某箭头实时的屏幕位置</returns>
        private Quaternion GetRealRotation(ulong actorId)
        {
            Vector3 playerScreenPos = GetRealPlayerScreenPos();
            Vector3 enemyScreenPos = GetRealEnemyScreenPos(actorId);

            Vector3 direction = enemyScreenPos - playerScreenPos;

            Vector3 x = Vector3.Cross(direction, Vector3.up);
            float angle = Vector3.Angle(direction, Vector3.up);

            if (x.z > 0)
            {
                angle = -angle;
            }

            return Quaternion.Euler(new Vector3(0, 0, angle));
        }

        /// <summary>
        /// 返回实时的玩家位置
        /// </summary>
        /// <returns>实时的玩家的屏幕位置</returns>
        private Vector3 GetRealPlayerScreenPos()
        {
            ActorBase playerActorBase = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);
            return Point2Screen(playerActorBase.GetPosition());
        }

        /// <summary>
        /// 返回实时的某 actor 位置
        /// </summary>
        /// <param name="actorId">敌机 id</param>
        /// <returns>实时的敌机的屏幕位置</returns>
        private Vector3 GetRealEnemyScreenPos(ulong actorId)
        {
            ActorBase enemyActorBase = m_ctx.m_CurrentLevel.GetActor(actorId);
            return Point2Screen(enemyActorBase.GetPosition());
        }

        #endregion

        #region 雷达标记(line renderer)

        /// <summary>
        /// 在新生敌机上加上 lr 组件并关闭
        /// </summary>
        /// <param name="actorId"></param>
        private void LineAttachedToNewEnemy(ulong actorId)
        {
            if (m_actorDic == null) return;

            ActorBase newborn = m_ctx.GetActorBase(actorId);

            if (newborn.IsShip() && !newborn.IsPlayer())
            {
                GameObject enemy = m_actorDic[actorId];
                enemy.AddComponent<LineRenderer>();
                LineRenderer lr = enemy.GetComponent<LineRenderer>();
                lr.enabled = false;
            }
        }

        /// <summary>
        /// 判断是敌机后，加入当前敌人列表
        /// </summary>
        private void AddToCurrentEnemies(ulong actorId)
        {
            ActorBase newborn = m_ctx.GetActorBase(actorId);

            if (newborn.IsShip() && !newborn.IsPlayer())
            {
                m_currentEnemies.Add(newborn);
            }
        }

        /// <summary>
        /// 判断是敌机后，从当前敌人列表去除
        /// </summary>
        /// <param name="actorId"></param>
        private void RemoveFromCurrentEnemies(ulong actorId)
        {
            ActorBase justDied = m_ctx.GetActorBase(actorId);

            if (justDied.IsShip() && !justDied.IsPlayer())
            {
                m_currentEnemies.Remove(justDied);
            }
        }

        /// <summary>
        /// 更新画面上的雷达
        /// </summary>
        private void UpdateMarks()
        {
            foreach (var enemy in m_currentEnemies)
            {
                LineRenderer lr = m_actorDic[enemy.GetActorID()].GetComponent<LineRenderer>();
                if (lr == null) return;
                lr.enabled = false;

                if (CanBeSeen(enemy))
                {
                    lr.enabled = false;
                    return;
                }

                DrawOneLine(enemy.GetActorID());
            }
        }

        /// <summary>
        /// 某 actor 是否在屏幕范围内
        /// </summary>
        /// <param name="actor">某 actor</param>
        /// <returns>可见返回 true</returns>
        private bool CanBeSeen(ActorBase actor)
        {
            Vector3 worldPos = BattleHelper.Point2Vector3(actor.GetPosition());
            Vector3 viewPos = m_camera.WorldToViewportPoint(worldPos);

            if (viewPos.x >= 0f && viewPos.x <= 1 &&
                viewPos.y >= 0f && viewPos.y <= 1)
            {
                return true;
            }
            return false;
        }
        private bool CanBeSeen(Vector3 position)
        {
            Vector3 viewPos = m_camera.ScreenToViewportPoint(position);

            if (viewPos.x >= 0f && viewPos.x <= 1 &&
                viewPos.y >= 0f && viewPos.y <= 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 画指定 actorId 的飞机的指示线
        /// </summary>
        /// <param name="actorId"></param>
        private void DrawOneLine(ulong actorId)
        {
            LineRenderer lr = m_actorDic[actorId].GetComponent<LineRenderer>();
            if (lr == null) return;
            lr.enabled = true;

            ActorBase theEnemy = m_ctx.GetActorBase(actorId);
            if (theEnemy == null) return;

            ActorBase playerActorBase = m_ctx.GetPlayerShipActor(m_ctx.PlayerId);

            Vector3 enemyScreenPos = Point2Screen(theEnemy.GetPosition());
            Vector3 playerScreenPos = Point2Screen(playerActorBase.GetPosition());

            Vector3 p = GetIntersectionP(enemyScreenPos, playerScreenPos);
            Vector3 direction = (playerScreenPos - enemyScreenPos).normalized;
            float distance = (playerScreenPos - enemyScreenPos).magnitude;
            float multiplier = Mathf.Clamp(distance / 10f, 100f, 250);

            Vector3 p1 = m_camera.ScreenToWorldPoint(p);
            Vector3 p2 = m_camera.ScreenToWorldPoint(p + direction * multiplier);

            lr.SetPositions(new[] { p1, p2 });
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.startWidth = 2f;
            lr.endWidth = 0.5f;
        }

        /// <summary>
        /// 从 point 坐标转到屏幕 Point
        /// </summary>
        /// <param name="pointPos"></param>
        /// <returns></returns>
        private Vector3 Point2Screen(Vector2 pointPos)
        {
            
            Vector3 worldPos = BattleHelper.Point2Vector3(pointPos);
            return m_camera.WorldToScreenPoint(worldPos);
        }

        /// <summary>
        /// 求真正的交点
        /// </summary>
        /// <param name="screenPosOfTheActor">敌机的屏幕坐标</param>
        /// <param name="screenPosOfThePlayer">玩家飞机的屏幕坐标</param>
        /// <param name="direction">表示方位</param>
        /// <returns></returns>
        private Vector3 GetIntersectionP(Vector3 screenPosOfTheActor, Vector3 screenPosOfThePlayer)
        {
            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(m_screenWidth, 0, 0);
            Vector3 p3 = new Vector3(0, m_screenHeight, 0);
            Vector3 p4 = new Vector3(m_screenWidth, m_screenHeight, 0);

            var node1 = GetIntersection(p1, p2, screenPosOfTheActor, screenPosOfThePlayer);
            if (node1 != Vector3.zero)
            {
                node1.z = screenPosOfTheActor.z;
                return node1;
            }

            var node2 = GetIntersection(p1, p3, screenPosOfTheActor, screenPosOfThePlayer);
            if (node2 != Vector3.zero)
            {
                node2.z = screenPosOfTheActor.z;
                return node2;
            }

            var node3 = GetIntersection(p2, p4, screenPosOfTheActor, screenPosOfThePlayer);
            if (node3 != Vector3.zero)
            {
                node3.z = screenPosOfTheActor.z;
                return node3;
            }

            var node4 = GetIntersection(p3, p4, screenPosOfTheActor, screenPosOfThePlayer);
            if (node4 != Vector3.zero)
            {
                node4.z = screenPosOfTheActor.z;
                return node4;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// 求两个线段的交点
        /// </summary>
        /// <param name="line1Start">line1Start</param>
        /// <param name="line1End">line1End</param>
        /// <param name="line2Start">line2Start</param>
        /// <param name="line2End">line2End</param>
        /// <returns></returns>
        private Vector3 GetIntersection(
            Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
        {

            
            Vector3 intersection = new Vector3();

            float line1A, line1B, line1C, line2A, line2B, line2C, d;
            line1A = line1Start.y - line1End.y;
            line2A = line2Start.y - line2End.y;
            line1B = line1End.x - line1Start.x;
            line2B = line2End.x - line2Start.x;
            line1C = line1End.y * line1Start.x - line1Start.y * line1End.x;
            line2C = line2End.y * line2Start.x - line2Start.y * line2End.x;
            d = line1A * line2B - line2A * line1B;

            intersection.x = (line1B * line2C - line2B * line1C) / d;
            intersection.y = (line1C * line2A - line2C * line1A) / d;

            if (intersection.x > Math.Max(line1Start.x, line1End.x) ||
                intersection.x > Math.Max(line2Start.x, line2End.x) ||
                intersection.y > Math.Max(line1Start.y, line1End.y) ||
                intersection.y > Math.Max(line2Start.y, line2End.y) ||
                intersection.x < Math.Min(line1Start.x, line1End.x) ||
                intersection.x < Math.Min(line2Start.x, line2End.x) ||
                intersection.y < Math.Min(line1Start.y, line1End.y) ||
                intersection.y < Math.Min(line2Start.y, line2End.y))
            {
                return Vector3.zero;
            }
            
            return intersection;
        }

        #endregion

        #region tools
        /// <summary>
        /// 尽量不要用
        /// </summary>
        /// <returns></returns>
        private GameServerGlobalConfig GetConfig()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "GameServerConfig.config");
            {

                WWW www = new WWW(path);

                XmlSerializer xmlSerializer =
                    new XmlSerializer(typeof(GameServerGlobalConfig), new XmlAttributeOverrides());
                GameServerGlobalConfig obj;
                using (Stream stream = new MemoryStream(www.bytes))
                {
                    obj = xmlSerializer.Deserialize(stream) as GameServerGlobalConfig;
                }

                return obj;
            }
        }

        public void BindActorDic(Dictionary<ulong, GameObject> dict)
        {
            m_actorDic = dict;
        }
        public void setState(bool b)
        {
            m_state = b;
        }
        public void Stop()
        {
            m_state = false;
            ReleaseEvent();
        }

        #endregion
    }
}
