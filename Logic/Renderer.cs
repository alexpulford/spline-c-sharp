using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

using ImGuiNET;

namespace GraphTool
{
    class Renderer
    {
        private Window window;
        private GUI.ImGuiController controller;
        private InputState old;
        private Camera camera;

        private bool rendering = true;

        private int current = 0;
        private int count = 2;
        private bool locked = false;
        public Renderer(Window w)
        {
            window = w;
            window.OnLoaded += Load;
            window.OnFrame += Render;
            window.OnUpdate += Input;
            window.OnResized += Resize;
            window.OnKeyInput += KeyInput;
            window.OnWheelInput += WheelInput;
        }

        public List<Thing> things = new List<Thing>();
        private void Load(object sender, object e)
        {
            GL.ClearColor(0.055f, 0.055f, 0.055f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            foreach (Thing t in things)
            {
                t.Init();
            }

            Vector2i z = window.ClientSize;

            controller = new GUI.ImGuiController(z.X, z.Y);

            camera = new Camera(new Vector3(0.0f, 0.0f, 3f), z);
        }

        private void Resize(object sender, ResizeEventArgs e)
        {
            Vector2i z = window.ClientRectangle.Size;
            camera.UpdateSize(z);
            controller.WindowResized(z.X, z.Y);

            foreach (Thing t in things)
            {
                t.Resize(z.X, z.Y);
            }

            rendering = z.X * z.Y != 0;

        }
        private void Render(object sender, FrameEventArgs e)
        {
            controller.Update(window, (float)e.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (rendering)
            {
                foreach (Thing t in things)
                {
                    t.Render(camera);
                }
                current++;
                if (count == current)
                {
                    current = 0;
                    foreach (Thing t in things)
                    {
                        t.Update();
                    }
                }
            }
            controller.Render();
        }

        private void Input(object sender, FrameEventArgs e)
        {
            float time = (float)e.Time;

            MouseState mouse = window.MouseState;
            
            if (old != null && locked)
            {
                float deltaX = mouse.Position.X - mouse.PreviousPosition.X;
                float deltaY = mouse.Position.Y - mouse.PreviousPosition.Y;

                camera.Yaw += deltaX * 0.2f;
                camera.Pitch -= deltaY * 0.2f;
            }

            KeyboardState keys = window.KeyboardState;

            if (old == null)
            {
                old = new InputState(keys, mouse, time);
            }

            if (keys.IsKeyDown(Keys.Escape))
            {
                window.Close();
            }

            if (keys.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * 1.5f * time;
            }

            if (keys.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * 1.5f * time;
            }
            if (keys.IsKeyDown(Keys.A))
            {
                camera.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * 1.5f * time;
            }
            if (keys.IsKeyDown(Keys.D))
            {
                camera.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * 1.5f * time;
            }

            if(keys.IsKeyPressed(Keys.LeftControl))
            {
                locked = !locked;
                window.CursorVisible = !window.CursorVisible;
            }
            InputState now = new InputState(keys, mouse, time);

            foreach (Thing t in things)
            {
                t.Input(old, now);
            }

            old = now;
        }

        private void KeyInput(object sender, TextInputEventArgs e)
        {
            controller.PressChar((char)e.Unicode);
        }

        private void WheelInput(object sender, MouseWheelEventArgs e)
        {
            controller.MouseScroll(e.Offset);
        }

        public void Add(Thing thing)
        {
            things.Add(thing);
        }
    }
}
