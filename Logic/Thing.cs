using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTool
{
    abstract class Thing
    {
        public abstract void Init();
        public abstract void Update();
        public abstract void Input(InputState old, InputState now);
        public abstract void Render(Camera camera);

        public abstract void Resize(int x, int y);
        public abstract void Destroy();
    }
}