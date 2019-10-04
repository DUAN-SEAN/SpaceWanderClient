using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Assets.Game.Manager.BattleTask.Controller
{
    public class CGController:MonoBehaviour
    {
        private  Dictionary<string, PlayableBinding>  bindingDict ;
        private PlayableDirector qPlayableDirector;

        void Awake()
        {
            bindingDict = new Dictionary<string, PlayableBinding>();
            qPlayableDirector = GetComponent<PlayableDirector>();

            foreach (PlayableBinding pb in qPlayableDirector.playableAsset.outputs)
            {
                if (!bindingDict.ContainsKey(pb.streamName))
                {
                    bindingDict.Add(pb.streamName, pb);
                }
                Debug.Log(pb.streamName + " out Type = " + pb.outputTargetType);
            }
        }
        void Start()
        {
            
        }
        /// <summary>
        /// 向资产中绑定track片段操作的对象
        /// 例如TimeLine中名为Camera的片段需要添加Cinemachine Brain组件对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        public void BindTrackTargetObject(string key,Object o)
        {
            if (o == null) return;
            if(bindingDict.ContainsKey(key))
                qPlayableDirector.SetGenericBinding(bindingDict["Camera"].sourceObject, o);
        }

        public void Play()
        {
            qPlayableDirector?.Play();
        }


        public void Stop()
        {
            qPlayableDirector?.Stop();
            Destroy(this);
        }
    }
}
