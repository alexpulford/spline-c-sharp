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
    class Line : Geometry
    {
        private int vbo, vao, count;
        private Shader shader;

        float m, c;
        bool y;

        Vector3 color;

        public Line(float m, float c, Vector3 color, bool y = false)
        {
            this.m = m;
            this.c = c;
            this.color = color;
            this.y = y;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            shader = new Shader(Util.ReadResource("line_vert.glsl"), Util.ReadResource("line_frag.glsl"));
            shader.Use();

            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var colorLocation = shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

        }
        public override void Render(Box2 clip, Matrix4 proj)
        {
            float[] v;
            if (!y)
            {
                v = new float[]
                {
                    clip.Min.X, m*clip.Min.X + c, 1f,
                    color.X, color.Y, color.Z,
                    clip.Max.X, m*clip.Max.X + c, 1f,
                    color.X, color.Y, color.Z,
                };
            } else {
                v = new float[]
                {
                    m*clip.Min.Y + c, clip.Min.Y, 1f,
                    color.X, color.Y, color.Z,
                    m*clip.Max.Y + c, clip.Max.Y, 1f,
                    color.X, color.Y, color.Z,
                };
            }
            count = v.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), v, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(vao);
            shader.Use();

            shader.SetMatrix4("model", Matrix4.Identity);
            shader.SetMatrix4("view", Matrix4.Identity);
            shader.SetMatrix4("projection", proj);

            GL.LineWidth(2);

            GL.DrawArrays(PrimitiveType.Lines, 0, count / 6);
        }
    }
}
