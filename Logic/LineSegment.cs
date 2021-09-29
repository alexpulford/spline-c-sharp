using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace GraphTool
{
    class LineSegment : Geometry
    {
        private int vbo, vao, count;
        private Shader shader;
        public Vector2 a;
        public Vector2 b;
        private PointState aState;
        private PointState bState;

        private Graph g;
        public LineSegment(Vector2 a, Vector2 b, Graph g)
        {
            this.a = a;
            this.b = b;
            aState = PointState.Idle;
            bState = PointState.Idle;
            this.g = g;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            shader = new Shader(Util.ReadResource("line_vert.glsl"), Util.ReadResource("line_frag.glsl"));
            shader.Use();

            Generate();

            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var colorLocation = shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

        }
        public void Generate()
        {
            Vector3 aColor = Geometry.colors[(int)aState];
            Vector3 bColor = Geometry.colors[(int)bState];
            float[] v = new float[]
            {
                a.X, a.Y, 1f,
                aColor.X, aColor.Y, aColor.Z,
                b.X, b.Y, 1f,
                bColor.X, bColor.Y, bColor.Z,
            };

            v = v.Concat(Util.genSquare(a, g.c, aColor)).Concat(Util.genSquare(b, g.c, bColor)).ToArray();

            count = v.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), v, BufferUsageHint.StaticDraw);
        }
        public override void Render(Box2 clip, Matrix4 proj)
        {
            
            GL.BindVertexArray(vao);
            shader.Use();

            shader.SetMatrix4("model", Matrix4.Identity);
            shader.SetMatrix4("view", Matrix4.Identity);
            shader.SetMatrix4("projection", proj);

            GL.LineWidth(2);

            GL.DrawArrays(PrimitiveType.Lines, 0, count / 6);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, point_vbo);
            //GL.DrawArrays(PrimitiveType.Lines, 0, point_count / 6);
        }

        public override bool Click(Vector2 pos, bool down)
        {
            bool changed = false;
            if(down && Util.L1Distance(pos, a) < g.c2)
            {
                aState = PointState.Drag;
                changed = true;
            }
            else if (down && Util.L1Distance(pos, b) < g.c2)
            {
                bState = PointState.Drag;
                changed = true;
            } else if (!down)
            {
                aState = bState = PointState.Idle;
            }
            Generate();
            return changed;
        }
        public override bool Drag(Vector2 pos, Vector2 start)
        {
            bool changed = false;
            bool aD = Vector2.Distance(pos, a) < g.c2;
            bool bD = Vector2.Distance(pos, b) < g.c2;

            if (aState != PointState.Drag)
            {
                aState = aD ? PointState.Hover : PointState.Idle;
            }

            if (bState != PointState.Drag)
            {
                bState = bD ? PointState.Hover : PointState.Idle;
            }

            if (aState == PointState.Drag)
            {
                a = Util.Snap(pos, new Vector2(g.grid.minor, g.grid.minor));
                changed = true;
            }
            if (bState == PointState.Drag)
            {
                b = Util.Snap(pos, new Vector2(g.grid.minor, g.grid.minor));
                changed = true;
            }
            Generate();
            return changed;
        }

        public override void Destroy()
        {
            
        }
    }
}
