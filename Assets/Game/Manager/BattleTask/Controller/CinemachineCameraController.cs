using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 创建一个虚拟摄像机
    /// </summary>
    public void CreateCinemachineVircualCamera()
    {
        gameObject.AddComponent<CinemachineBrain>();

        cvm = new GameObject("cvm").AddComponent<CinemachineVirtualCamera>();

        cvm.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

        cvm.m_Lens.OrthographicSize = 100f;
 
    }
    /// <summary>
    /// 绑定摄像头
    /// 默认绑定到player物体身上
    /// </summary>
    public void BindCamera(string playeName)
    {
        var p = GameObject.Find(playeName);
        cinemachineFramingTransposer = cvm.AddCinemachineComponent<CinemachineFramingTransposer>();

        ChangeCameraDistance(400f);

        cvm.m_Follow = p.transform;

        cvm.MoveToTopOfPrioritySubqueue();



        //Cinemachine Bpdy参数设置
        cinemachineFramingTransposer.m_ScreenX = 0.4952083f;
        cinemachineFramingTransposer.m_ScreenY = 0.4190741f;
        cinemachineFramingTransposer.m_DeadZoneWidth = 0.6f;
        cinemachineFramingTransposer.m_DeadZoneHeight = 0.53f;
        cinemachineFramingTransposer.m_DeadZoneDepth = 0f;
        cinemachineFramingTransposer.m_SoftZoneWidth = 0.6f;
        cinemachineFramingTransposer.m_SoftZoneHeight = 0.53f;
        cinemachineFramingTransposer.m_BiasY = 1.238f;



    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="distance"></param>
    public void ChangeCameraDistance(float distance)
    {
        cinemachineFramingTransposer.m_CameraDistance = distance;
        Material m = default;
        //m.mainTextureOffset=new Vector2(x,y);
    }

    private CinemachineVirtualCamera cvm;
    private CinemachineFramingTransposer cinemachineFramingTransposer;
}
