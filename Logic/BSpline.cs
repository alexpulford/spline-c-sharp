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
    enum BSplineType
    {
        open,
        clamped,
        closed,
        none,
    }
    class BSpline : Geometry
    {


        private int vbo, vao, count;
        private Shader shader;

        public List<float> U;
        public List<Vector2> P;
        public int m;
        public int n;
        public int d = 3;
        public float[] domain;
        public BSplineType type;

        public Vector3 color;
        private float c;
        private Graph g;
        private const int precision = 1000;
        private List<PointState> sP;
        public bool edit;

        public BSpline(Vector3 color, Graph g)
        {
            this.g = g;
            this.c = g.c;
            this.color = color;
            edit = false;

            U = new List<float>();
            P = new List<Vector2>(new Vector2[] {
                new Vector2(-1,-1),
                new Vector2(-1,1),
                new Vector2(1,1),
                new Vector2(1,-1),
            });

            change_type(BSplineType.clamped);

            sP = Enumerable.Repeat(PointState.Idle, P.Count).ToList();

            Init();
        }

        public void add_control(Vector2 pos)
        {
            P.Add(pos);
            sP.Add(PointState.Hover);
            calculate_knot();
        }
        public void remove_control(int index)
        {
            P.RemoveAt(index);
            sP.RemoveAt(index);
            calculate_knot();
        }
        public void change_type(BSplineType new_type)
        {
            type = new_type;
            calculate_knot();
        }

        private void calculate_knot()
        {
            U.Clear();
            switch (type)
            {
                case BSplineType.clamped:
                    n = P.Count - 1;
                    int l = n - d;
                    U = Enumerable.Repeat(0f, d + 1).Concat(Enumerable.Range(1, l).Select(i => i / (float)(l+1))).Concat(Enumerable.Repeat(1f, d + 1)).ToList();
                    domain = new float[] {0f, 1f};
                    break;
                case BSplineType.closed:
                    n = P.Count + 2;
                    m = n + d + 1;
                    U = Enumerable.Range(0, m + 1).Select(i => i / (float)(m)).ToList();
                    domain = new float[] { U[d], U[m - d] };
                    break;
            }
            
            m = U.Count - 1;
            n = m - d - 1;

            if (m != n + d + 1)
            {
                Console.WriteLine("oh no");
            }
        }

        private float N(int i, int p, float x)
        {
            if (p == 0)
            {
                return U[i] <= x && x < U[i + 1] ? 1 : 0;
            }
            float a = (x - U[i]) / (U[i + p] - U[i]);
            a = float.IsFinite(a) ? a : 0;
            float b = (U[i + p + 1] - x) / (U[i + p + 1] - U[i + 1]);
            b = float.IsFinite(b) ? b : 0;
            return a * N(i, p - 1, x) + b * N(i + 1, p - 1, x);
        }
        private Vector2 S(float t)
        {
            Vector2 a = Vector2.Zero;
            for(int i = 0; i <= n; i++)
            {
                a += N(i, d, t) * P[i% P.Count];
            }
            return a;
        }

        private void Init()
        {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            shader = new Shader(Util.ReadResource("line_vert.glsl"), Util.ReadResource("line_frag.glsl"));
            shader.Use();

            Generate();

            var vertexLocation = shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            var colorLocation = shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }
        private void Generate()
        {
            Vector3 a = this.color;

            float[] v = new float[precision * 12];

            float tmin = U[d];
            float tmax = U[m-d];


            Vector2 o = S(tmin);

            for (int i = 1; i < precision; i++)
            {
                float t = (tmax-tmin)*((float)i / (float)precision) + tmin;

                Vector2 n = S(t);

                v[i * 12 + 0] = o.X;
                v[i * 12 + 1] = o.Y;
                v[i * 12 + 2] = 1f;
                v[i * 12 + 3] = a.X;
                v[i * 12 + 4] = a.Y;
                v[i * 12 + 5] = a.Z;

                v[i * 12 + 6] = n.X;
                v[i * 12 + 7] = n.Y;
                v[i * 12 + 8] = 1f;
                v[i * 12 + 9] = a.X;
                v[i * 12 + 10] = a.Y;
                v[i * 12 + 11] = a.Z;
                o = n;
            }

            Vector3 b = new Vector3(0.3f, 0.3f, 0.3f);

            IEnumerable<float> tmp = v.AsEnumerable<float>();

            for (int i = 0; i < sP.Count; i++)
            {
                tmp = tmp.Concat(Util.genSquare(P[i%P.Count], g.c, Geometry.colors[(int)sP[i]]));
            }
            for (int i = d; i < U.Count-d-1; i++)
            {
                Vector2 n = S(U[i]);
                tmp = tmp.Concat(Util.genSquare(n, g.c, Geometry.colors[3]));
            }

            for (int i = 1; i < sP.Count; i++)
            {
                tmp = tmp.Concat(Util.genLine(P[(i - 1) % P.Count], P[i % P.Count], Geometry.colors[(int)sP[i-1]], Geometry.colors[(int)sP[i]]));
            }

            v = tmp.ToArray();
            count = v.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, count * sizeof(float), v, BufferUsageHint.StaticDraw);
        }
        public override void Render(Box2 clip, Matrix4 proj)
        {
            if(c != g.c)
            {
                Generate();
                c = g.c;
            }
            

            GL.BindVertexArray(vao);
            shader.Use();

            shader.SetMatrix4("model", Matrix4.Identity);
            shader.SetMatrix4("view", Matrix4.Identity);
            shader.SetMatrix4("projection", proj);

            
            GL.LineWidth(1);

            GL.DrawArrays(PrimitiveType.Lines, precision * 2, count - (precision*2));

            GL.LineWidth(3);

            GL.DrawArrays(PrimitiveType.Lines, 0, (precision * 2));
        }

        public override bool Click(Vector2 pos, bool down)
        {
            bool changed = false;

            for (int i = 0; i < sP.Count; i++)
            {
                if(Util.L1Distance(pos, P[i]) < g.c2)
                {
                    if(down)
                    {
                        if (edit)
                        {
                            remove_control(i);
                            changed = true;
                            break;
                        }
                        sP[i] = PointState.Drag;
                        changed = true;
                    }
                    else
                    {
                        sP[i] = PointState.Hover;
                    }
                }
                else
                {
                    sP[i] = PointState.Idle;
                }
            }

            if(down && !changed && edit)
            {
                add_control(pos);
                changed = true;
            }

            Generate();
            return changed;
        }
        public override bool Drag(Vector2 pos, Vector2 start)
        {
            bool changed = false;

            for(int i = 0; i < sP.Count; i++)
            {
                if(sP[i] != PointState.Drag)
                {
                    sP[i] = Util.L1Distance(P[i], pos) < g.c2 ? PointState.Hover : PointState.Idle;
                } else
                {
                    P[i] = Util.Snap(pos, new Vector2(g.grid.minor, g.grid.minor));
                    changed = true;
                }

            }
            Generate();
            return changed;
        }

    }
}
