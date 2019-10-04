using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Game.Manager.BattleTask.Controller;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using Object = System.Object;

namespace Assets.Game.Manager.BattleTask
{
    public class CGBattleTask:ITaskEventSystem
    {
        public void Init()
        {
            m_CGDic = new Dictionary<string, CGController>();
        }

        public void Start()
        { 
           
        }
        /// <summary>
        /// 播放CG动画
        /// 关卡1开场动画
        /// </summary>
        public void PlayCG_1()
        {
            Camera cameraMain = GameObject.FindWithTag("SpaceCamera").GetComponent<Camera>();
            var cgName = CGResourceDefine.CG_1Path;
            Debug.Log(cgName);
            CGController controller = FindorBuildCgController(cgName);
            if (controller == null) return;

            var brain = cameraMain.GetComponent<CinemachineBrain>();
            cameraMain.orthographic = false;//切换摄像头

            if (brain == null) Debug.Log("Brain == null");
            controller.BindTrackTargetObject("Camera",brain);
            controller.Play();
        }
        public void StopCG_1()
        {
            Camera cameraMain = GameObject.FindWithTag("SpaceCamera").GetComponent<Camera>();
            var cgName = CGResourceDefine.CG_1Path;
            CGController controller = FindorBuildCgController(cgName);
            controller.Stop();
            cameraMain.orthographic = true;
            m_CGDic.Remove(cgName);
        }
        public void Play(string cgName)
        {
            CGController controller = FindorBuildCgController(cgName);
            //进行播放逻辑
            controller.Play();
        }

        private CGController FindorBuildCgController(string cgName,Vector3 pos = default)
        {
            if (pos == default)
                pos = new Vector3(-200f, -1000f, 0);
            CGController controller;
            if (!m_CGDic.TryGetValue(cgName, out controller))
            {
                GameObject cg = UnityEngine.Object.Instantiate(Resources.Load(cgName)) as GameObject;
                cg.transform.position = pos;
                controller = cg.GetComponent<CGController>();
                m_CGDic.Add(cgName, controller);
            }

            return controller;
        }
        public void Update()
        {
        }

        public void Dispose()
        {
        }


        private Dictionary<string, CGController> m_CGDic;

     
    }

    public class CGResourceDefine
    {
        //CG路径
        public const string CG_1Path = "CG/CG-1";
    }
}
