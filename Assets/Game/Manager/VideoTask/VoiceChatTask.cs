using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Actor;
using Assets.Game.Manager.VideoTask.Controller;
using Crazy.Main;
using UnityEngine;
using Object = System.Object;

namespace Assets.Game.Manager.VideoTask
{
    public class VoiceChatTask : ITaskEventSystem<bool>
    {
        public VoiceChatTask(VideoManager videoManager)
        {
            _videoManager = videoManager;
        }
        /// <summary>
        /// 逻辑初始化
        /// </summary>
        public void Init()
        {
            _audioBlockDic = new Dictionary<string, Queue<AudioBlock>>();

        }

     
        public void Start(bool o)
        {
            if (_recordController == null || _speakController == null)
            {
                var voiceChat = UnityEngine.Object.Instantiate(Resources.Load(VideoResourceDefine.VoiceChatPath)) as GameObject;
                if (voiceChat == null) throw new Exception("VoiceChat Prefab Can't Build");
                _speakController = voiceChat?.GetComponent<SpeakController>();
                _recordController = voiceChat?.GetComponent<RecordController>();
            }

            if (currentContext != GameManager.Instance.CurrentPlayerContext)
            {
                currentContext = GameManager.Instance.CurrentPlayerContext;

            }
            currentContext.SpeakInTeamCallBack += OnSpeakInTeam;
            currentContext.OnAudioBattleEvent += OnSpeakInBattle;
            _audioBlockDic = new Dictionary<string, Queue<AudioBlock>>();
            _lobby = o;
            last = 0;
            intervalTime = 0f;
            _isWork = true;
            _recordController.StartRecord();
        }

        /// <summary>
        /// 逻辑开启
        /// </summary>
        public void Start()
        {
            if (_recordController == null || _speakController == null)
            {
                var voiceChat = GameObject.Instantiate(Resources.Load(VideoResourceDefine.VoiceChatPath)) as GameObject;
                _speakController = voiceChat.GetComponent<SpeakController>();
                _recordController = voiceChat.GetComponent<RecordController>();
            }
            if (currentContext != GameManager.Instance.CurrentPlayerContext)
            {
                currentContext = GameManager.Instance.CurrentPlayerContext;

            }
            currentContext.SpeakInTeamCallBack += OnSpeakInTeam;
            currentContext.OnAudioBattleEvent += OnSpeakInBattle;
            _audioBlockDic = new Dictionary<string, Queue<AudioBlock>>();
            last = 0;
            intervalTime = 0f;
            _lobby = true;
            _isWork = true;
            _recordController.StartRecord();


        }
        public void Stop()
        {
            if (currentContext != GameManager.Instance.CurrentPlayerContext)
            {
                currentContext = GameManager.Instance.CurrentPlayerContext;

            }
            currentContext.SpeakInTeamCallBack -= OnSpeakInTeam;
            currentContext.OnAudioBattleEvent -= OnSpeakInBattle;
            _audioBlockDic.Clear();
            last = 0;
            _isWork = false;
            _recordController.StopRecord();

        }

        public void Update()
        {
            if (!_isWork || _recordController == null || _speakController == null)
            {
                return;
            }

            TickRecord();

            TickPlayAudio();


        }

        private void TickPlayAudio()
        {

            List<float> dataList = new List<float>();
            foreach (var channleAudio in _audioBlockDic)
            {
                if (channleAudio.Value.Count < 10) continue;

                int i = 0;

                AudioBlock audioBlock = null;
                while (i < 10)
                {
                    audioBlock = channleAudio.Value.Dequeue();
                    foreach (var f in audioBlock.data)
                    {
                        dataList.Add(f);
                    }
                    i++;
                }

                _speakController.PlayRecord(channleAudio.Key, dataList.ToArray(), audioBlock.channels, audioBlock.frequency);
                dataList.Clear();
            }
        }

        private void TickRecord()
        {
            intervalTime += Time.deltaTime;
            if (intervalTime > 1f)
            {

                _recordController.CollectRecord(out var audioClip, out int pos);

                float[] data;
                if (pos < last)
                {
                    float[] data1 = new float[audioClip.samples - last];
                    audioClip.GetData(data1, last);
                    float[] data2 = new float[pos];
                    audioClip.GetData(data2, 0);
                    data = new float[data1.Length + data2.Length];
                    //拼接
                    Array.Copy(data1,0,data,0,data1.Length);
                    Array.Copy(data2,0,data, data1.Length, data2.Length);
                }
                else
                {
                    data = new float[pos - last];
                    audioClip.GetData(data, last);
                }

                int length = 0;
                float[] dataTwo = data;
                length = dataTwo.Length;


                if (length > audioSize)
                {
                    //Debug.Log("Length > 800 length = " +length);
                    List<AudioBlock> audioBlocks = new List<AudioBlock>();
                    float[] temp = new float[audioSize];
                    int readyedRead = 0;
                    while (length > audioSize)
                    {
                        int j = 0;
                        Array.Copy(dataTwo, readyedRead, temp, 0, audioSize);

                        AudioBlock audioBlock = new AudioBlock();
                        audioBlock.data = temp;
                        audioBlock.channels = audioClip.channels;
                        audioBlock.frequency = audioClip.frequency;
                        audioBlock.samples = dataTwo.Length;

                        SendAudio(audioBlock);

                        temp = new float[audioSize];
                        length -= audioSize;
                        readyedRead += audioSize;
                    }

                    if (length > 0)
                    {
                        Array.Copy(dataTwo, readyedRead, temp, 0, length);
                        AudioBlock audioBlock = new AudioBlock();
                        audioBlock.data = temp;
                        audioBlock.channels = audioClip.channels;
                        audioBlock.frequency = audioClip.frequency;
                        audioBlock.samples = dataTwo.Length;

                        SendAudio(audioBlock);
                    }
                }
                else
                {
                    AudioBlock audioBlock = new AudioBlock();
                    audioBlock.data = dataTwo;
                    audioBlock.channels = audioClip.channels;
                    audioBlock.frequency = audioClip.frequency;
                    audioBlock.samples = dataTwo.Length;

                    SendAudio(audioBlock);
                }


                last = pos;
                intervalTime = 0f;

            }
        }

        public void SendAudio(AudioBlock audioBlock)
        {

            if (_lobby)
                currentContext.SendAudioToTeam(audioBlock);
            else
            {
                currentContext.SendAudioToBattle(audioBlock);
            }
        }
        public void OnSpeakInTeam(string playerId, AudioBlock audioBlock)
        {

            if (!_audioBlockDic.ContainsKey(playerId)) _audioBlockDic.Add(playerId, new Queue<AudioBlock>());
            _audioBlockDic[playerId].Enqueue(audioBlock);

        }
        public void OnSpeakInBattle(string playerId, AudioBlock audioBlock)
        {
            if (!_audioBlockDic.ContainsKey(playerId)) _audioBlockDic.Add(playerId, new Queue<AudioBlock>());
            _audioBlockDic[playerId].Enqueue(audioBlock);
        }


        public void Dispose()
        {
            Stop();
        }
        /// <summary>
        /// 时间区间临时变量
        /// </summary>
        private float intervalTime = 0f;
        /// <summary>
        /// 读取音频取样临时变量
        /// </summary>
        private int last = 0;
        /// <summary>
        /// 是否是大厅中使用语音
        /// </summary>
        private bool _lobby;
        /// <summary>
        /// 音频包长度，float数组长度
        /// </summary>
        private int audioSize = 800;
        /// <summary>
        /// 是否允许Task工作
        /// </summary>
        private bool _isWork = false;
        /// <summary>
        /// 当前玩家现场
        /// </summary>
        private SpacePlayerContext currentContext = null;
        /// <summary>
        /// 音频包字典
        /// </summary>
        private Dictionary<string, Queue<AudioBlock>> _audioBlockDic;
        /// <summary>
        /// 音频管理器
        /// </summary>
        private VideoManager _videoManager;
        /// <summary>
        /// 扬声器控制器
        /// </summary>
        private SpeakController _speakController;
        /// <summary>
        /// 录音器控制器
        /// </summary>
        private RecordController _recordController;




    }
}
