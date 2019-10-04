using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Manager.VideoTask.Controller
{
    public class RecordController : MonoBehaviour
    {


        private string _deviceName;//录音设备名称

        private int _recordInterval;//录音时长

        private int _frequency = 44100;//采样频率 电话：8000 CD：44100 默认为CD的采样频率

        private AudioClip _audioClip;

        void Awake()
        {
            foreach (var device in Microphone.devices)
            {
                _deviceName = device;


                Microphone.GetDeviceCaps(device, out var minFreq, out var maxFreq);
                Debug.Log("Name: " + device);
                Debug.Log("minFreq = " + minFreq);
                Debug.Log("maxFreq = " + maxFreq);

            }

            
            _deviceName = Microphone.devices[0];
            _recordInterval = 3599;
            _frequency = 8000;
        }

        void Start()
        {


            //StartRecord();
        }
        private float interval = 0;
        void Update()
        {
            interval += Time.deltaTime;
            if (interval > 1f)
            {
                //Debug.Log("音频采样点 = " + Microphone.GetPosition(_deviceName));


                interval = 0;
            }

        }
        public void StartRecord()
        {
            if (_deviceName == null || _deviceName == default)
            {
                throw new Exception("未找到麦克风设备");
            }

            _audioClip = Microphone.Start(_deviceName, true, _recordInterval, _frequency);
            Debug.Log("录音通道 = "+_audioClip.channels+" frequency = "+_audioClip.frequency+" samples = "+_audioClip.samples);
        }


        public void StopRecord()
        {
            Microphone.End(_deviceName);
        }
        public void CollectRecord(out AudioClip audioClip, out int position)
        {
            audioClip = _audioClip;
            position = Microphone.GetPosition(_deviceName);
        }

    }
}
