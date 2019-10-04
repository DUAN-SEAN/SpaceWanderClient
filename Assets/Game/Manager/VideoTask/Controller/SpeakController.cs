using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Game.Manager.VideoTask.Controller
{
    public class SpeakController:MonoBehaviour
    {

        private AudioSource _audioSource;
        void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayRecord(string playerId,float[] data,int channels = 2,int frequency = 8000)
        {
           
            var audioClip =  AudioClip.Create("语音:"+ playerId, data.Length, channels, frequency, false);
            
            audioClip.SetData(data, 0);
            AudioSource.PlayClipAtPoint(audioClip,new Vector3(0,0,-100));
            //_audioSource.clip = audioClip;
            //_audioSource.Play();
        }


        


    }
}
