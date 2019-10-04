using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Assets.Game.Manager.UITask;
using Crazy.Common;
using Crazy.Main;
using GameServer.Configure;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Log = Crazy.ClientNet.Log;

namespace Assets.Game.Manager
{
    public interface ITickable
    {
        void Tick();
    }
    public interface IManagerContext
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();
        /// <summary>
        /// 绑定现场
        /// </summary>
        /// <param name="spacePlayerContext"></param>
        void BindSpacePlayerContext(SpacePlayerContext spacePlayerContext);
        /// <summary>
        /// 释放现场
        /// </summary>
        void ReleaseSpacePlayerContext();
        /// <summary>
        /// 现场属性
        /// </summary>
        SpacePlayerContext PlayerContext { get; }
    }
    public class GameManager:MonoBehaviour
    {

        void Awake()
        {

            if (Instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            Application.targetFrameRate = 30;
            Instance = this;
            _netWorkManager = new NetWorkManager(this);


            _uiManager = new UIManager(this);
            _battleManager = new BattleManager(this);
            _videoManager = new VideoManager(this);

            Initialize();
            DontDestroyOnLoad(this);
        }
        public void Start()
        {
            LoadScenceFromTask("UILobby", () =>
            {
                //加载场景后启动登录Task
                _uiManager.TaskDic[typeof(UILoginTask)].Start();

                //_videoManager.VoiceChatTask.Start();
            });

        }
        /// <summary>
        /// 初始化各个管理器
        /// </summary>
        private void Initialize()
        {
            InitConfig(configPath);

            _netWorkManager.Initialize(m_gameServerGlobalConfig);

            CreatePlayerContext();
            _uiManager.Initialize();
            _battleManager.Initialize();
            _videoManager.Initialize();
        }

        private GameServerGlobalConfig InitConfig(string path)
        {
            //GameServerGlobalConfig gameServerGlobalConfig = Util.Deserialize<GameServer.Configure.GameServerGlobalConfig>(path);
            path = Path.Combine(Application.streamingAssetsPath, "GameServerConfig.config");
            // 1 www加载 过时
            //{
            //    WWW www = new WWW(path);
            //    XmlSerializer xmlSerializer =
            //        new XmlSerializer(typeof(GameServerGlobalConfig), new XmlAttributeOverrides());
            //    GameServerGlobalConfig obj;
            //    using (Stream stream = new MemoryStream(www.bytes))
            //    {
            //        obj = xmlSerializer.Deserialize(stream) as GameServerGlobalConfig;
            //    }

            //    m_gameServerGlobalConfig = obj;
            //}

            // 2 UnityWebRequest加载 异步不可用
            //StartCoroutine(LoadStreamingAssets(path,
            //    () => { Log.Info($"Server Ip = {m_serverConfig.EndPortIP} Port = {m_serverConfig.EndPortPort}"); }));

            // 3 本地加载，安卓不可用
            //{
                //GameServerGlobalConfig gameServerGlobalConfig = Util.Deserialize<GameServer.Configure.GameServerGlobalConfig>(path);
                //m_gameServerGlobalConfig = gameServerGlobalConfig ;
            //}

            // 4 UnityWebRequest 同步加载 可用
            UnityWebRequest wr = new UnityWebRequest(path);
            DownloadHandlerBuffer bufferHandler = new DownloadHandlerBuffer();
            wr.downloadHandler = bufferHandler;
            var operation = wr.SendWebRequest();
            while (!operation.isDone)
            {
                Thread.Sleep(100);
            }
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(GameServerGlobalConfig), new XmlAttributeOverrides());
            GameServerGlobalConfig obj;
            using (Stream stream = new MemoryStream(bufferHandler.data))
            {
                obj = xmlSerializer.Deserialize(stream) as GameServerGlobalConfig;
            }

            m_gameServerGlobalConfig = obj;
            m_serverConfig = m_gameServerGlobalConfig.Global.Servers[0];
            Log.Info($"Server Ip = {m_serverConfig.EndPortIP} Port = {m_serverConfig.EndPortPort}");

            return m_gameServerGlobalConfig;
        }

        /// <summary>
        /// Tick各个Task管理器
        /// </summary>
        public void Update()
        {
            _uiManager?.Tick();
            _battleManager?.Tick();
            _netWorkManager?.Tick();
            _videoManager?.Tick();
        }
        /// <summary>
        /// 创建一个玩家现场
        /// </summary>
        public void CreatePlayerContext()
        {
            var client = _netWorkManager.CreateClient();
            SpacePlayerContext spacePlayerContext = new SpacePlayerContext(client,m_gameServerGlobalConfig);
            _currentSpacePlayerContext = spacePlayerContext;

            _uiManager.BindSpacePlayerContext(_currentSpacePlayerContext);
            _battleManager.BindSpacePlayerContext(_currentSpacePlayerContext);
            _netWorkManager.BindSpacePlayerContext(_currentSpacePlayerContext);
        }

        #region UnityObject
        [SerializeField]
        private string configPath;



        /// <summary>
        /// 加载场景，并在场景加载完成后执行回调
        /// </summary>
        /// <param name="scenceName">场景名</param>
        /// <param name="action">回调</param>
        public void LoadScenceFromTask(string scenceName,Action action = null,bool needLoading = false)
        {
            StartCoroutine(LoadScence(scenceName, action,needLoading));
        }
        /// <summary>
        /// 加载场景，并在场景加载完成后执行回调
        /// </summary>
        /// <param name="scenceIndex">场景Build索引</param>
        /// <param name="action"></param>
        public void LoadScenceFromTask(int scenceIndex, Action action = null,bool needLoading = false)
        {
            StartCoroutine(LoadScence(scenceIndex, action,needLoading));
        }
        /// <summary>
        /// 加载场景，执行回调
        /// 选择弹出加载界面
        /// </summary>
        /// <param name="scenceName">场景名称</param>
        /// <param name="action">回调函数</param>
        /// <param name="needLoading">需要加载界面</param>
        /// <returns></returns>
        public IEnumerator LoadScence(string scenceName, Action action = null, bool needLoading = false)
        {
            Debug.Log("开始异步加载场景");
            if(needLoading)
                (UiManager.TaskDic[typeof(UILoadingTask)] as UILoadingTask)?.Start();
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenceName);
            while (!asyncOperation.isDone)
            {
                //Output the current progress
                var text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
                if (needLoading)
                    (UiManager.TaskDic[typeof(UILoadingTask)] as UILoadingTask)?.SetLoadingText(asyncOperation.progress,text);
                // Check if the load has finished
                Debug.Log(text);
                yield return null;
            }
            yield return asyncOperation;
            Debug.Log("加载场景成功：" + scenceName);
            //if (needLoading)
            //    (UiManager.TaskDic[typeof(UILoadingTask)] as UILoadingTask)?.Dispose();
            action?.Invoke();
        }

        /// <summary>
        /// 加载场景，执行回调
        /// 选择弹出加载界面
        /// </summary>
        /// <param name="scenceIndex">场景索引</param>
        /// <param name="action">回调函数</param>
        /// <param name="needLoading">需要加载界面</param>
        /// <returns></returns>
        public IEnumerator LoadScence(int scenceIndex, Action action = null, bool needLoading = false)
        {
            Debug.Log("开始异步加载场景");
            if (needLoading)
                (UiManager.TaskDic[typeof(UILoadingTask)] as UILoadingTask)?.Start();
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenceIndex);
            while (!asyncOperation.isDone)
            {
                //Output the current progress
                var text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
                if (needLoading)
                    (UiManager.TaskDic[typeof(UILoadingTask)] as UILoadingTask)?.SetLoadingText(asyncOperation.progress, text);
                // Check if the load has finished
                Debug.Log(text);
                yield return null;
            }
            yield return asyncOperation;
            Debug.Log("加载场景成功：" + scenceIndex);
            //if (needLoading)
            //    (UiManager.TaskDic[typeof(UILoadingTask)] as UILoadingTask)?.Dispose();
            action?.Invoke();
        }
        /// <summary>
        /// 异步从StreamingAssets中加载数据
        /// 加载数据后执行回调
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IEnumerator LoadStreamingAssets(string url, Action<Byte[]> action = null)
        {
            UnityWebRequest wr = new UnityWebRequest(url);
            DownloadHandlerBuffer bufferHandler = new DownloadHandlerBuffer();
            wr.downloadHandler = bufferHandler;
            yield return wr.SendWebRequest();
            if (!wr.isNetworkError)
            {
                action?.Invoke(bufferHandler.data);
            }
            
        }
        #endregion

        #region 字段
        /// <summary>
        /// UI管理器
        /// </summary>
        private UIManager _uiManager;
        /// <summary>
        /// 战斗管理器
        /// </summary>
        private BattleManager _battleManager;
        /// <summary>
        /// 网络管理器
        /// </summary>
        private NetWorkManager _netWorkManager;
        /// <summary>
        /// 音频管理器
        /// </summary>
        private VideoManager _videoManager;
        /// <summary>
        /// 当前玩家现场
        /// </summary>
        private SpacePlayerContext _currentSpacePlayerContext;




        public static GameManager Instance;


        private ServerBaseGlobalConfigure m_Globalfigure;

        private Server m_serverConfig;

        private GameServerGlobalConfig m_gameServerGlobalConfig;


        #endregion

        #region 属性

        public GameServerGlobalConfig gameServerGlobalConfig => m_gameServerGlobalConfig;

        public SpacePlayerContext CurrentPlayerContext =>_currentSpacePlayerContext;
        public UIManager UiManager => _uiManager;

        public VideoManager VideoManager => _videoManager;
        public BattleManager BattleManager => _battleManager;

        #endregion

    }
}
