using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Manager.VideoTask;

namespace Assets.Game.Manager
{
    public class VideoManager:ManagerBase
    {
        public VideoManager(GameManager gm) : base(gm)
        {
            
        }
        public override void Initialize()
        {
            _voiceChatTask = new VoiceChatTask(this);
            _voiceChatTask.Init();
            base.Initialize();

        }

        public void Tick()
        {
            _voiceChatTask.Update();
        }

        public void StartSpeakInLobby()
        {
            _voiceChatTask.Start(true);
        }

        public void StartSpeakInBattle()
        {
            _voiceChatTask.Start(false);
        }
        public void Stop()
        {
            _voiceChatTask.Stop();
        }

        
        public void OnSwitchAudio()
        {
            if (!audioOpen)
            {
                StartSpeakInLobby();
                audioOpen = true;
            }
            else
            {
                Stop();
                audioOpen = false;
            }


        }

        private VoiceChatTask _voiceChatTask;

        private bool audioOpen = false;
        public VoiceChatTask VoiceChatTask => _voiceChatTask;
    }

    public class VideoResourceDefine
    {
        public const string VoiceChatPath = "VoiceChat";
    }
}
