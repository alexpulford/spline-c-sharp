using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace GraphTool
{
    class Window : GameWindow
    {
        public event EventHandler<FrameEventArgs> OnFrame;

        public event EventHandler<FrameEventArgs> OnUpdate;

        public event EventHandler<ResizeEventArgs> OnResized;

        public event EventHandler<TextInputEventArgs> OnKeyInput;

        public event EventHandler<MouseWheelEventArgs> OnWheelInput;

        public event EventHandler OnLoaded;

        public Window(string title) : base(new GameWindowSettings() { }, new NativeWindowSettings() { Title = title, Size = new Vector2i(1600, 900), APIVersion = new Version(4, 5) })
        { }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            OnUpdate?.Invoke(this, e);
            base.OnUpdateFrame(e);
        }
        protected override void OnLoad()
        {
            OnLoaded?.Invoke(this, null);
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Old OpenTk version, FPS limiting sucks. So we do it manually
            Thread.Sleep(Math.Clamp((int)(16 - e.Time),0,16));
            OnFrame?.Invoke(this, e);
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            OnResized?.Invoke(this, e);
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            OnKeyInput?.Invoke(this, e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            OnWheelInput?.Invoke(this, e);
        }
    }
}
