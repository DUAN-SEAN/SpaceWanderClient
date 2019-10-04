using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
using Color = Box2DSharp.Common.Color;

namespace Box2DSharp.External
{
    public class BodyRender 
    {
        public IDrawer Drawer;

        public BodyRender(IDrawer drawer)
        {
            this.Drawer = drawer;
            Drawer.Flags = DrawFlag.DrawShape;
        }

        public void DrawBody(IEnumerable<Body> BodyList)
        {
            if (Drawer == null)
            {
                return;
            }
            var inactiveColor = Color.FromArgb(128, 128, 77);
            var staticBodyColor = Color.FromArgb(127, 230, 127);
            var kinematicBodyColor = Color.FromArgb(127, 127, 230);
            var sleepColor = Color.FromArgb(153, 153, 153);
            var lastColor = Color.FromArgb(230, 179, 179);
            var flags = Drawer.Flags;

            if (flags.HasFlag(DrawFlag.DrawShape))
            {
                foreach (var b in BodyList)
                {
                    var xf = b.GetTransform();
                    var isActive = b.IsActive;
                    var isAwake = b.IsAwake;
                    foreach (var f in b.FixtureList)
                    {
                        if (isActive == false)
                        {
                            DrawShape(f, xf, inactiveColor);
                        }
                        else if (b.BodyType == BodyType.StaticBody)
                        {
                            DrawShape(f, xf, staticBodyColor);
                        }
                        else if (b.BodyType == BodyType.KinematicBody)
                        {
                            DrawShape(f, xf, kinematicBodyColor);
                        }
                        else if (isAwake == false)
                        {
                            DrawShape(f, xf, sleepColor);
                        }
                        else
                        {
                            DrawShape(f, xf, lastColor);
                        }
                    }
                }
            }

        }

        private void DrawShape(Fixture fixture, in Common.Transform xf, in Common.Color color)
        {
            switch (fixture.Shape)
            {
                case CircleShape circle:
                    {
                        var center = MathUtils.Mul(xf, circle.Position);
                        var radius = circle.Radius;
                        var axis = MathUtils.Mul(xf.Rotation, new Vector2(1.0f, 0.0f));

                        Drawer.DrawSolidCircle(center, radius, axis, color);
                    }
                    break;

                case EdgeShape edge:
                    {
                        var v1 = MathUtils.Mul(xf, edge.Vertex1);
                        var v2 = MathUtils.Mul(xf, edge.Vertex2);
                        Drawer.DrawSegment(v1, v2, color);
                    }
                    break;

                case ChainShape chain:
                    {
                        var count = chain.Count;
                        var vertices = chain.Vertices;

                        var ghostColor = Color.FromArgb(
                            color.A,
                            (int)(0.75f * color.R),
                            (int)(0.75f * color.G),
                            (int)(0.75f * color.B));

                        var v1 = MathUtils.Mul(xf, vertices[0]);
                        Drawer.DrawPoint(v1, 4.0f, color);

                        if (chain.HasPrevVertex)
                        {
                            var vp = MathUtils.Mul(xf, chain.PrevVertex);
                            Drawer.DrawSegment(vp, v1, ghostColor);
                            Drawer.DrawCircle(vp, 0.1f, ghostColor);
                        }

                        for (var i = 1; i < count; ++i)
                        {
                            var v2 = MathUtils.Mul(xf, vertices[i]);
                            Drawer.DrawSegment(v1, v2, color);
                            Drawer.DrawPoint(v2, 4.0f, color);
                            v1 = v2;
                        }

                        if (chain.HasNextVertex)
                        {
                            var vn = MathUtils.Mul(xf, chain.NextVertex);
                            Drawer.DrawSegment(v1, vn, ghostColor);
                            Drawer.DrawCircle(vn, 0.1f, ghostColor);
                        }
                    }
                    break;

                case PolygonShape poly:
                    {
                        var vertexCount = poly.Count;
                        Debug.Assert(vertexCount <= Settings.MaxPolygonVertices);
                        var vertices = new Vector2[vertexCount];

                        for (var i = 0; i < vertexCount; ++i)
                        {
                            vertices[i] = MathUtils.Mul(xf, poly.Vertices[i]);
                        }

                        Drawer.DrawSolidPolygon(vertices, vertexCount, color);
                    }
                    break;
            }
        }

    }
}
