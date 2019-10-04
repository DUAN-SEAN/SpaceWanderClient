using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TargetArrowController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject btn1;
    public GameObject btn2;
    void Start()
    {
        Debug.Log(btn1.transform.position);
        Debug.Log(btn2.transform.position);
    }

    private float z = 0;
    // Update is called once per frame
    void Update()
    {
        RotateByScreenPoint(btn2.transform.position,btn1.transform.position);
        //this.transform.rotation = Quaternion.Euler(new Vector3(this.transform.rotation.x, this.transform.rotation.y, z));
    }

    public void RotateByScreenPoint(Vector3 from, Vector3 to)
    {
        var dir = to - from;
        //this.transform.forward = dir;
        Debug.Log("from = "+from+":to = "+to);
        Debug.Log("dir = "+dir);
        Vector3 cos = Vector3.Cross(dir.normalized, Vector3.up);//叉乘求180度转折
        float angle = Vector3.Angle(dir.normalized, Vector3.up);//求出夹角，不分正负
        if (cos.z > 0)//叉积z大于0 角度变换，由于canvas是个镜像所以取z大于0
        {
            angle = -angle;
        }
        Debug.Log("Angle = "+angle);
        Debug.Log("Cross = "+cos);

        
        //this.transform.forward = new Vector3(this.transform.rotation.x, this.transform.rotation.y,cos.z);
        this.transform.rotation = Quaternion.Euler(new Vector3(this.transform.rotation.x, this.transform.rotation.y, angle));
        this.transform.position = from;
        //this.transform.position = 
    }



}
