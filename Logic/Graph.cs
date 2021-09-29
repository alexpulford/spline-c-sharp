using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GraphTool
{
    class Graph : Thing
    {
        public Vector2 origin { get; private set; }
        public Vector2 view { get; private set; }
        public Box2 rect { get; private set; }
        public Vector2i window { get; private set; }
        public Vector2i UI { get; private set; }
        public Box2 screen { get; private set; }
        public bool drag { get; private set; }
        public float c { get; private set; }
        public float c2 { get; private set; }

        public Geometry selected { get; private set; }

        public Vector2 mouseStart;
        public Vector2 mousePos;

        private float ratio;

        public Grid grid;

        public List<Geometry> geometry;
        public Graph(int x, int y) {
            origin = new Vector2(0, 0);
            geometry = new List<Geometry>();
            grid = new Grid(this);
            drag = false;
            Resize(x, y);
            //geometry.Add(new Polynomial(new float[] { 0, 0, 1 }, new Vector3(1f, 1f, 0f)));
            //geometry.Add(new Polynomial(new float[] { 8, -6, 3, 1 }, new Vector3(1f, 0f, 1f)));
            geometry.Add(new BSpline(new Vector3(1f, 0.5f, 0f), this));

           
        }

        public override void Init()
        {

        }

        private Box2 CalcRect()
        {
            c = 10 * (view.X / window.X);
            c2 = 1.2f * c;
            return new Box2(origin - view, origin + view);
        }

        private Vector2 MapMouse(Vector2 pos)
        {
            return new Vector2(Util.Map(pos.X, new Vector2(UI.X, window.X), new Vector2(rect.Min.X, rect.Max.X)),
                               Util.Map(pos.Y, new Vector2(window.Y, 0), new Vector2(rect.Min.Y, rect.Max.Y)));
        }
        public override void Input(InputState old, InputState now)
        {
            if (now.mouse.ScrollDelta.Y != 0)
            {
                view *= 1.0f + (0.01f * now.mouse.ScrollDelta.Y);
                rect = CalcRect();
            }

            if (now.keys.IsKeyDown(Keys.W))
            {
                origin = new Vector2(origin.X, origin.Y + 0.1f); rect = CalcRect();
            }
            if (now.keys.IsKeyDown(Keys.A))
            {
                origin = new Vector2(origin.X - 0.1f, origin.Y); rect = CalcRect();
            }
            if (now.keys.IsKeyDown(Keys.S))
            {
                origin = new Vector2(origin.X, origin.Y - 0.1f); rect = CalcRect();
            }
            if (now.keys.IsKeyDown(Keys.D))
            {
                origin = new Vector2(origin.X + 0.1f, origin.Y); rect = CalcRect();
            }
            mousePos = MapMouse(now.mouse.Position);

            if (rect.Contains(mousePos))
            {
                if (now.mouse.IsButtonDown(MouseButton.Left) && !now.mouse.WasButtonDown(MouseButton.Left))
                {
                    mouseStart = MapMouse(now.mouse.Position);
                    drag = true;
                    foreach (Geometry g in geometry)
                    {
                        if (g.Click(mousePos, drag))
                        {
                            selected = g;
                            break;
                        }
                    }
                }
                if (!now.mouse.IsButtonDown(MouseButton.Left) && now.mouse.WasButtonDown(MouseButton.Left))
                {
                    mouseStart = new Vector2(0, 0);
                    drag = false;
                    foreach (Geometry g in geometry)
                    {
                        if (g.Click(mousePos, drag)) break;
                    }
                }
            }
        }
        public override void Render(Camera camera)
        {
            
            Matrix4 m = Matrix4.CreateOrthographicOffCenter(rect.Min.X, rect.Max.X, rect.Min.Y, rect.Max.Y, -1f, 1f);
            System.Drawing.Rectangle r = new System.Drawing.Rectangle(UI.X, 0, (int)window.X - UI.X, (int)window.Y);
            
            grid.Update();
            grid.RenderOverlay(r);
            
            GL.Viewport(r);
            
            grid.Render();
            GL.DepthFunc(DepthFunction.Always);
            foreach (Geometry g in geometry)
            {
                if (g.Drag(mousePos, mouseStart)) break;
            }

            foreach (Geometry g in geometry)
            {
                g.Render(rect, m);
            }
            GL.DepthFunc(DepthFunction.Less);
            GL.Viewport(0, 0, (int)window.X, (int)window.Y);
        }

        public override void Resize(int x, int y)
        {
            UI = new Vector2i(Math.Clamp((int)(x * 0.2), 100, 500), y);
            window = new Vector2i(x, y);
            ratio = (float)(window.X - UI.X) / (float)(UI.Y);
            view = new Vector2(10, 10 * (1f / ratio));
            rect = CalcRect();
        }

        public override void Update()
        {


        }

        public override void Destroy()
        {

        }


    }
}
