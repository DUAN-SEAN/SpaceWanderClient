using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlaneType
{
    public GameObject FH1, F1, F2, DA, DR, GC, C1;
}

public class WareHouse : MonoBehaviour
{
    public PlaneType PlaneType;
    public Transform Platform;
    public Transform PlaneTransform;
    private GameObject tempObject;
    private float speed = 5f;
    private int flag = 0;
    private Vector3 DownPlatform, UpPlatform, DownPlane, UpPlane;
    private GameObject PlaneObject;

    private GameObject PlaneObjectPre;

    private string str;
    //private string FH1 = "黄蜂飞船";
    //private string F1 = "战斗机A";
    //private string F2 = "战斗机B";
    //private string DA = "无人机";
    //private string DR = "歼灭船";
    //private string GC = "精英船A";
    //private string C1 = "精英船B";
    //string[] ShipType = { "黄蜂飞船", "战斗机A", "战斗机B", "无人机", "歼灭船", "精英船A", "精英船B" };

    // Start is called before the first frame update
    void Start()
    {
        tempObject = Instantiate(PlaneType.FH1, PlaneTransform.transform);
        DownPlatform = Platform.position - new Vector3(0, 7.3f, 0);
        UpPlatform = Platform.position;
        DownPlane = PlaneTransform.position - new Vector3(0, 7.3f, 0);
        UpPlane = PlaneTransform.position;

        PlaneObjectPre = PlaneType.FH1;
    }

    // Update is called once per frame
    void Update()
    {
        if (flag==0)
        {
            if (GameObject.Find("Canvas/ShipHousePanel(Clone)/TextContentGroup/ShipContentText") != null)
            {
                str = GameObject.Find("Canvas/ShipHousePanel(Clone)/TextContentGroup/ShipContentText").GetComponent<Text>().text;

                switch (str)
                {
                    case "黄蜂飞船":
                        PlaneObject = PlaneType.FH1;
                        if(PlaneObjectPre != PlaneObject)
                        {
                            flag = 1;
                        }
                        break;
                    case "战斗机A":
                        PlaneObject = PlaneType.F1;
                        if (PlaneObjectPre != PlaneObject)
                        {
                            flag = 1;
                        }
                        break;
                    case "战斗机B":
                        PlaneObject = PlaneType.F2;
                        if (PlaneObjectPre != PlaneObject)
                        {
                            flag = 1;
                        }
                        break;
                    case "无人机":
                        PlaneObject = PlaneType.DA;
                        if (PlaneObjectPre != PlaneObject)
                        {
                            flag = 1;
                        }
                        break;
                    case "歼灭船":
                        PlaneObject = PlaneType.DR;
                        if (PlaneObjectPre != PlaneObject)
                        {
                            flag = 1;
                        }
                        break;
                    //case "精英船A":
                    //    PlaneObject = PlaneType.GC;
                    //    if (PlaneObjectPre != PlaneObject)
                    //    flag = 1;
                    //    break;
                    //case "精英船B":
                    //    PlaneObject = PlaneType.C1;
                    //    if (PlaneObjectPre != PlaneObject)
                    //    flag = 1;
                    //    break;
                    default:
                        PlaneObject = PlaneType.FH1;
                        if (PlaneObjectPre != PlaneObject)
                            flag = 1;
                        break;

                        //if (Input.GetKey(KeyCode.Alpha1))
                        //{
                        //    PlaneObject = PlaneType.FH1;
                        //    flag = 1;
                        //}
                        //if (Input.GetKey(KeyCode.Alpha2))
                        //{
                        //    PlaneObject = PlaneType.F1;
                        //    flag = 1;
                        //}
                        //if (Input.GetKey(KeyCode.Alpha3))
                        //{
                        //    PlaneObject = PlaneType.F2;
                        //    flag = 1;
                        //}
                        //if (Input.GetKey(KeyCode.Alpha4))
                        //{
                        //    PlaneObject = PlaneType.DA;
                        //    flag = 1;
                        //}
                        //if (Input.GetKey(KeyCode.Alpha5))
                        //{
                        //    PlaneObject = PlaneType.DR;
                        //    flag = 1;
                        //}
                        
                    }
                PlaneObjectPre = PlaneObject;
                }
            }
        
        if (flag==1)
        {
            Platform.position = Vector3.MoveTowards(Platform.position, DownPlatform, speed * Time.deltaTime);
            tempObject.transform.position = Vector3.MoveTowards(tempObject.transform.position, DownPlane, speed * Time.deltaTime);
            if (Vector3.Distance(Platform.position,DownPlatform) <= 0.1f)
            {
                Destroy(tempObject);
                tempObject = Instantiate(PlaneObject, DownPlane, PlaneTransform.rotation);
                flag = 2;
            }
        }
        if(flag==2)
        {
            Platform.position = Vector3.MoveTowards(Platform.position, UpPlatform, speed * Time.deltaTime);
            tempObject.transform.position = Vector3.MoveTowards(tempObject.transform.position, UpPlane, speed * Time.deltaTime);
            if (Vector3.Distance(Platform.position, UpPlatform) <= 0.1f)
            {
                //Destroy(tempObject);
                //tempObject = Instantiate(PlaneType.TypeTwo, Plane.transform.position - new Vector3(0, -7.3f, 0), Plane.rotation);
                flag = 0;
            }
        }
        

    }
    void FixUpdate()
    {
       
    }
}
