using System;
using System.IO;
using System.Reflection;
using OpenTK.Mathematics;
namespace GraphTool
{
    class Util
    {
        public static float L1Distance(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
        public static float Round(float v, float i)
        {
            return (float)(MathHelper.Round(v / i) * i);
        }
        public static Vector2 Snap(Vector2 p, Vector2 i)
        {
            return new Vector2(Round(p.X, i.X), Round(p.Y, i.Y));
        }
        public static float Map(float v, Vector2 a, Vector2 b)
        {
            return (v - a.X) * ((b.Y - b.X) / (a.Y - a.X)) + b.X;
        }
        public static Vector2 Conv(System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static System.Numerics.Vector2 Conv(Vector2 v)
        {
            return new System.Numerics.Vector2(v.X, v.Y);
        }
        public static string ReadResource(string name)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphTool.Shaders." + name);
            string result;
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result.Trim(new char[] { '\uFEFF', '\u200B' });
        }

        public static float[] genSquare(Vector2 p, float z, Vector3 c)
        {
            float[] vertices = new float[] {
                p.X-z, p.Y-z, 0, c.X, c.Y, c.Z,
                p.X-z, p.Y+z, 0, c.X, c.Y, c.Z,

                p.X-z, p.Y+z, 0, c.X, c.Y, c.Z,
                p.X+z, p.Y+z, 0, c.X, c.Y, c.Z,

                p.X+z, p.Y+z, 0, c.X, c.Y, c.Z,
                p.X+z, p.Y-z, 0, c.X, c.Y, c.Z,

                p.X+z, p.Y-z, 0, c.X, c.Y, c.Z,
                p.X-z, p.Y-z, 0, c.X, c.Y, c.Z
            };
            return vertices;
        }

        public static float[] genLine(Vector2 a, Vector2 b, Vector3 aC, Vector3 bC)
        {
            float[] vertices = new float[] {
                a.X, a.Y, 0, aC.X, aC.Y, aC.Z,
                b.X, b.Y, 0, bC.X, bC.Y, bC.Z,
            };
            return vertices;
        }
    }
}