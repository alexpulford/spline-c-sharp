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
    class Polynomial : Geometry
    {
        private int vbo, vao, count;
        private Shader shader;

        float[] coeff;
        bool swap;

        Vector3 color;

        public Polynomial(float[] coeff, Vector3 color, bool swap = false)
        {
            this.coeff = coeff;
            this.color = color;
            this.swap = swap;

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

        private float evaluate(float x)
        {
            float o = 0;
            int degree = coeff.Length;

            for(int d = coeff.Length-1; d >= 0; d--)
            {
                o += coeff[d] * (float)Math.Pow(x, d);
            }
            return o;

        }
        public override void Render(Box2 clip, Matrix4 proj)
        {
            List<float> d = new List<float>();

            float x = clip.Min.X;
            float t = clip.Size.X/400f;

            while(x < clip.Max.X)
            {
                d.AddRange(new float[] {x, evaluate(x), 1f, color.X, color.Y, color.Z});
                x += t;
                d.AddRange(new float[] {x, evaluate(x), 1f, color.X, color.Y, color.Z});
            }
            float[] v = d.ToArray();
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
