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
    enum PointState
    {
        Idle = 0,
        Hover = 1,
        Drag = 2
    };

    class Geometry
    {
        static public Vector3[] colors = new Vector3[]{
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 0)
        };
        public virtual void Render(Box2 clip, Matrix4 proj) { }
        public virtual void Destroy() { }
        public virtual bool Drag(Vector2 pos, Vector2 delta) { return false; }
        public virtual bool Click(Vector2 pos, bool down) { return false; }
    }
}
