//using UnityEngine;
//using CrazyEngine.Core;
//using System.Linq;
//using CrazyEngine.Common;

//public class BodyRenderer
//{

//    private Engine _engine;

//    private Vector3[] ScreenCorners = new Vector3[4];

//    public BodyRenderer(Engine engine)
//    {
//        _engine = engine;
//    }

//    void Start()
//    {
        
//    }

//    public void Render()
//    {
//        ScreenCorners[0] = new Vector3(Camera.main.orthographicSize * 16 / 9 + Camera.main.transform.localPosition.x, 0, Camera.main.orthographicSize + Camera.main.transform.localPosition.z);
//        ScreenCorners[2] = new Vector3(-Camera.main.orthographicSize * 16 / 9 + Camera.main.transform.localPosition.x, 0, -Camera.main.orthographicSize + Camera.main.transform.localPosition.z);
//        ScreenCorners[1] = new Vector3(Camera.main.orthographicSize * 16 / 9 + Camera.main.transform.localPosition.x, 0, -Camera.main.orthographicSize + Camera.main.transform.localPosition.z);
//        ScreenCorners[3] = new Vector3(-Camera.main.orthographicSize * 16 / 9 + Camera.main.transform.localPosition.x, 0, Camera.main.orthographicSize + Camera.main.transform.localPosition.z);
//        RenderBodies();
//        RenderCollisions();
//        RenderCamera();
//    }

//    /// <summary>
//    /// 绘制物体
//    /// </summary>
//    public void RenderBodies()
//    {
//        var bodies = _engine.World.AllBodies.Where(x => x.Visible).ToList();

//        var parts = bodies.SelectMany(x => x.EnumParts().Where(y => y.Visible)).ToList();

//        if (_engine.Debug)
//        {
//            foreach (var body in bodies)    //画物体边界
//            {
//                var bounds = body.Bounds.ToPoints();
//                DrawPolygon(bounds, Color.blue);
//            }

//            foreach (var part in parts)     //画物体Collider
//            {
//                var vertices = part.Vertices.ToPoints();
//                if (vertices.Length > 2)
//                    DrawPolygon(vertices, Color.green);
//                else
//                {
//                    MonoBehaviour.print(vertices[0]+ " " + vertices[1]);
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// 碰撞信息
//    /// </summary>
//    public void RenderCollisions()
//    {
//        if (!_engine.Debug) return;
//        var pairs = _engine.Pairs.PairList.Where(x => x.Active).ToList();
//        foreach (var pair in pairs)
//        {
//            var contacts = pair.ActiveContacts;

//            //当有碰撞时候 输出两者的标签 （测试用）
//            //if (pair.Collision.Collided) MonoBehaviour.print(pair.Collision.BodyA.Label + " " + pair.Collision.BodyB.Label);

//            if (contacts.Count > 0)
//            {
//                var normalPosX = contacts[0].Vertex.X;
//                var normalPosY = contacts[0].Vertex.Y;

//                if (contacts.Count == 2)
//                {
//                    normalPosX = (normalPosX + contacts[1].Vertex.X) / 2d;
//                    normalPosY = (normalPosY + contacts[1].Vertex.Y) / 2d;
//                }

//                var collision = pair.Collision;
//                if (collision.BodyB == collision.Supports.Vertexes[0].Body || collision.BodyA.Static)
//                {
//                    Point p1 = new Point((float)normalPosX - (float)collision.Normal.X * 8f, (float)normalPosY - (float)collision.Normal.Y * 8f);
//                    Point p2 = new Point((float)normalPosX, (float)normalPosY);
//                    DrawLine(p1, p2, Color.red);
//                }
//                else
//                {
//                    Point p1 = new Point((float)normalPosX + (float)collision.Normal.X * 8f, (float)normalPosY + (float)collision.Normal.Y * 8f);
//                    Point p2 = new Point((float)normalPosX, (float)normalPosY);
//                    DrawLine(p1, p2, Color.red);
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// 渲染镜头看到的大小
//    /// </summary>
//    private void RenderCamera()
//    {
//        Gizmos.color = Color.white;
//        Gizmos.DrawLine(ScreenCorners[0], ScreenCorners[1]);
//        Gizmos.DrawLine(ScreenCorners[1], ScreenCorners[2]);
//        Gizmos.DrawLine(ScreenCorners[2], ScreenCorners[3]);
//        Gizmos.DrawLine(ScreenCorners[3], ScreenCorners[0]);
//    }


//    public void DrawPolygon(Point[] points, Color color)
//    {
//        for(int i=0; i<points.Length; i++)
//        {
//            DrawLine(points[i], points[(i + 1) % points.Length], color);
//        }
//    }

//    public void DrawLine(Point p1, Point p2, Color color)
//    {
//        Gizmos.color = color;
//        Gizmos.DrawLine(new Vector3((float)p1.X, 0, (float)p1.Y), new Vector3((float)p2.X, 0, (float)p2.Y));
//    }

//}
