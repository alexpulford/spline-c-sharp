using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using ImGuiNET;

namespace GraphTool
{
    partial class Grid
    {
        private int vbo, vao, count;
        private Shader shader;

        private Graph g;

        private Matrix4 model;
        private Vector3 offset;
        private Vector3 scale;
        private Vector2 colors;

        public float major { get; private set; }
        public float minor { get; private set; }

        public Grid(Graph graph) {
            g = graph;


            g.geometry.Add(new Line(0, 0, new Vector3(1f, 0f, 0f)));
            g.geometry.Add(new Line(0, 0, new Vector3(1f, 0f, 0f), true));

            colors = Vector2.Zero;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            Update();

            shader = new Shader(Util.ReadResource("grid_vert.glsl"), Util.ReadResource("grid_frag.glsl"));
            shader.Use();

            InitGrid();

            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var colorLocation = shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }

        partial void Overlay(System.Drawing.Rectangle r);

        public void RenderOverlay(System.Drawing.Rectangle r)
        {
            Overlay(r);
        }
        public void Render()
        {
            GL.BindVertexArray(vao);

            shader.Use();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", Matrix4.Identity);
            shader.SetMatrix4("projection", Matrix4.CreateOrthographic(g.view.X * 2, g.view.Y * 2, -1f, 1f));
            shader.SetVector2("colors", colors);

            GL.LineWidth(1);
            GL.DrawArrays(PrimitiveType.Lines, 0, count/6);
        }

        private float sigmoid(float x)
        {
            if(x < 0.125)
            {
                return 4f*x;
            } 
            else if(x > 0.875)
            {
                return 4f * (x - 0.75f);
            }
            else
            {
                return 0.5f;
            }
        }

        public float map(float x, float a=11, float b=55)
        {
            return (x - a) / (b - a);
        }
        public void Update()
        {
            float logX = (float)Math.Log(g.rect.Size.X, 5);

            major = (float)Math.Pow(5, Math.Round(logX) - 1);
            minor = (float)Math.Pow(5, Math.Round(logX) - 2);

            int mX = (int)(g.rect.Size.X / minor);

            float majorColor = 0.2f;
            float minorColor = ((majorColor-0.055f) * (float)Math.Pow(sigmoid(1.0f - map(mX)), 2.0)) + 0.055f;

            colors = new Vector2(majorColor, minorColor);

            scale = new Vector3(major * 6, major * 6, 0);
            offset = new Vector3(major - (g.origin.X % major), major - (g.origin.Y % major), 0);
            model = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(offset);
        }
        public void InitGrid(int s = 5)
        {
            int maxMajor = (s * 2) + 2;
            int maxMinor = maxMajor * s;

            float[] v = new float[(2*maxMajor + 2*maxMinor) * 12];


            float majorD = 2f / maxMajor;
            float minorD = 2f / maxMinor;
            float d = -1f;

            int i = 0;

            for(int x = 0; x < maxMajor; x++)
            {
                v[(i + x) * 12 + 0] = d;
                v[(i + x) * 12 + 1] = -1;
                v[(i + x) * 12 + 2] = -0.5f;
                v[(i + x) * 12 + 3] = 1f;
                v[(i + x) * 12 + 4] = 1f;
                v[(i + x) * 12 + 5] = 1f;

                v[(i + x) * 12 + 6] = d;
                v[(i + x) * 12 + 7] = 1;
                v[(i + x) * 12 + 8] = -0.5f;
                v[(i + x) * 12 + 9] = 1f;
                v[(i + x) * 12 + 10] = 1f;
                v[(i + x) * 12 + 11] = 1f;

                d += majorD;
            }

            i += maxMajor;
            d = -1;

            for (int y = 0; y < maxMajor; y++)
            {
                v[(i + y) * 12 + 0] = -1;
                v[(i + y) * 12 + 1] = d;
                v[(i + y) * 12 + 2] = -0.5f;
                v[(i + y) * 12 + 3] = 1f;
                v[(i + y) * 12 + 4] = 1f;
                v[(i + y) * 12 + 5] = 1f;

                v[(i + y) * 12 + 6] = 1;
                v[(i + y) * 12 + 7] = d;
                v[(i + y) * 12 + 8] = -0.5f;
                v[(i + y) * 12 + 9] = 1f;
                v[(i + y) * 12 + 10] = 1f;
                v[(i + y) * 12 + 11] = 1f;

                d += majorD;
            }

            i += maxMajor;
            d = -1;

            for (int x = 0; x < maxMinor; x++)
            {
                v[(i + x) * 12 + 0] = d;
                v[(i + x) * 12 + 1] = -1;
                v[(i + x) * 12 + 2] = -1;
                v[(i + x) * 12 + 3] = 0f;
                v[(i + x) * 12 + 4] = 0f;
                v[(i + x) * 12 + 5] = 0f;

                v[(i + x) * 12 + 6] = d;
                v[(i + x) * 12 + 7] = 1;
                v[(i + x) * 12 + 8] = -1;
                v[(i + x) * 12 + 9] = 0f;
                v[(i + x) * 12 + 10] = 0f;
                v[(i + x) * 12 + 11] = 0f;

                d += minorD;
            }

            i += maxMinor;
            d = -1;

            for (int y = 0; y < maxMinor; y++)
            {
                v[(i + y) * 12 + 0] = -1;
                v[(i + y) * 12 + 1] = d;
                v[(i + y) * 12 + 2] = -1;
                v[(i + y) * 12 + 3] = 0f;
                v[(i + y) * 12 + 4] = 0f;
                v[(i + y) * 12 + 5] = 0f;

                v[(i + y) * 12 + 6] = 1;
                v[(i + y) * 12 + 7] = d;
                v[(i + y) * 12 + 8] = -1;
                v[(i + y) * 12 + 9] = 0f;
                v[(i + y) * 12 + 10] = 0f;
                v[(i + y) * 12 + 11] = 0f;

                d += minorD;
            }

            count = v.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), v, BufferUsageHint.StaticDraw);
        }
    }
}
