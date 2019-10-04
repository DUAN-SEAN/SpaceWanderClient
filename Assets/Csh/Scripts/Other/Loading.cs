using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Loading : MonoBehaviour
{
    public Slider m_Slider;
    public Text m_Text;
   

    public void SetLoadingPercentage(int displayProgress)
    {
        m_Slider.value = displayProgress * 0.01f;
        m_Text.text = displayProgress.ToString() + "%";
    }
}
