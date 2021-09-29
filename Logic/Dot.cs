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
    class Dot : Geometry
    {
        private int vbo, vao, count;
        private Shader shader;

        Vector2 pos;
        Vector3 color;
        PointState state;
        Graph g;
        public Dot(float x, float y, Vector3 color, Graph g)
        {
            this.pos = new Vector2(x, y);
            this.color = color;
            this.g = g;
            state = PointState.Idle;

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
        private void Generate()
        {
            float[] v = Util.genSquare(pos, g.c, Geometry.colors[(int)state]);
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
        }

        public override bool Click(Vector2 p, bool down)
        {
            bool changed = false;
            if (down && Vector2.Distance(p, pos) < g.c2)
            {
                state = PointState.Drag;
                changed = true;
            }
            else if (!down)
            {
                state = PointState.Idle;
            }
            Generate();
            return changed;
        }
        public override bool Drag(Vector2 p, Vector2 start)
        {
            bool changed = false;
            bool d = Vector2.Distance(p, pos) < g.c2;

            if (state != PointState.Drag)
            {
                state = d ? PointState.Hover : PointState.Idle;
            }
            if (state == PointState.Drag)
            {
                pos = Util.Snap(p, new Vector2(g.grid.minor, g.grid.minor));
                changed = true;
            }
            
            Generate();
            return changed;
        }
    }
}
