using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = System.Diagnostics.Debug;

namespace Box2DSharp.Inspection
{
    [ExecuteInEditMode]
    public class UnityDrawer : MonoBehaviour
    {
        private const string SceneCameraName = "SceneCamera";

        private const string MainCameraTag = "SpaceCamera";

        private static Material _lineMaterial;

        private readonly List<Color> _colors = new List<Color>();

        private readonly List<List<(Vector3 begin, Vector3 end)>> _lines = new List<List<(Vector3, Vector3)>>();

        private readonly List<(Vector3 Center, float Radius, Color color)> _points =
            new List<(Vector3 Center, float Radius, Color color)>();

        private StringBuilder _stringBuilder = new StringBuilder();
        
        public static UnityDrawer GetDrawer()
        {
            var drawLines = FindObjectOfType<UnityDrawer>();
            if (drawLines == default)
            {
                drawLines = GameObject.FindWithTag(MainCameraTag).AddComponent<UnityDrawer>();
            }

            return drawLines;
        }

        private void OnRenderObject()
        {
            if (Camera.current == default)
            {
                return;
            }

            if (!Camera.current.CompareTag(MainCameraTag) && Camera.current.name != SceneCameraName)
            {
                return;
            }

            CreateLineMaterial();
            _lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            //print("Line Count :  " + _lines.Count);
            for (var i = 0; i < _lines.Count; i++)
            {
                GL.Color(_colors[i]);
                foreach (var line in _lines[i])
                {
                    //GL.Vertex(line.begin);
                    //GL.Vertex(line.end);
                    GL.Vertex(new Vector3(line.begin.x, 0, line.begin.y));
                    GL.Vertex(new Vector3(line.end.x, 0, line.end.y));
                }
            }
            GL.End();
            GL.Begin(GL.QUADS);
            for (var i = 0; i < _points.Count; i++)
            {
                var point = _points[i];
                GL.Color(point.color);
                GL.Vertex(new Vector3(point.Center.x - point.Radius, 0, point.Center.y - point.Radius));
                GL.Vertex(new Vector3(point.Center.x - point.Radius, 0, point.Center.y + point.Radius));
                GL.Vertex(new Vector3(point.Center.x + point.Radius, 0, point.Center.y + point.Radius));
                GL.Vertex(new Vector3(point.Center.x + point.Radius, 0, point.Center.y - point.Radius));

            }

            GL.End();

            _lines.Clear();
            _colors.Clear();
            _points.Clear();
        }

        public void PostLines(List<(Vector3 begin, Vector3 end)> lines, Color color)
        {
            _lines.Add(lines);
            _colors.Add(color);
            //print("Begin" + _lines.Count);

        }

        public void PostPoint((Vector3 Center, float Radius, Color color) point)
        {
            _points.Add(point);
        }

        

        private void Update()
        {
           
        }

        private void LateUpdate()
        {
           
        }
        private static void CreateLineMaterial()
        {
            if (_lineMaterial)
            {
                return;
            }

            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};

            // Turn on alpha blending
            _lineMaterial.SetInt("_SrcBlend", (int) BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);

            // Turn backface culling off
            _lineMaterial.SetInt("_Cull", (int) CullMode.Off);

            // Turn off depth writes
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}