using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Manager.UITask.Controller
{
    public class LoadingController:MonoBehaviour
    {
        public Slider m_Slider;
        public Text m_Text;


        public void SetLoadingPercentage(int displayProgress)
        {
            m_Slider.value = displayProgress * 0.01f;
            m_Text.text = displayProgress.ToString() + "%";
        }
        public void SetLoadingText(float displayProgress,string text)
        {
            m_Slider.value = displayProgress;
            m_Text.text = text;
        }
    }
    
}
