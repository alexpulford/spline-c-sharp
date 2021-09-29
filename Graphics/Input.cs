using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GraphTool
{
    class InputState
    {
        public KeyboardState keys;
        public MouseState mouse;
        public float time;
        public InputState(KeyboardState inKeys, MouseState inMouse, float inTime)
        {
            keys = inKeys;
            mouse = inMouse;
            time = inTime;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(keys, mouse);
        }
    }
}