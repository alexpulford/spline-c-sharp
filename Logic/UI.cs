using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using System.Numerics;

namespace GraphTool
{
    class UI : Thing
    {
        private Graph g;
        private string error = "";

        Vector2 a, b;
        public UI(Graph graph)
        {
            g = graph;
            a = b = Vector2.Zero;
        }

        public override void Init()
        {

        }
        public override void Input(InputState old, InputState now)
        {
        }
        public override void Render(Camera camera)
        {
            ImGui.SetNextWindowSize(Util.Conv(g.UI));
            ImGui.SetNextWindowPos(Vector2.Zero);
            if (!ImGui.Begin("Settings", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
            {
                ImGui.End();
                return;
            }

            if (g.selected?.GetType() == typeof(LineSegment))
            {
                LineSegment l = ((LineSegment)g.selected);
                a = new Vector2(l.a.X, l.a.Y);
                b = new Vector2(l.b.X, l.b.Y);
                if (ImGui.InputFloat2("LA", ref a) || ImGui.InputFloat2("LB", ref b))
                {
                    l.a.X = a.X; l.a.Y = a.Y;
                    l.b.X = b.X; l.b.Y = b.Y;
                }
            }

            if (g.selected?.GetType() == typeof(BSpline))
            {
                BSpline s = ((BSpline)g.selected);
                if (ImGui.CollapsingHeader("B-Spline"))
                {
                    Vector3 c = new Vector3(s.color.X, s.color.Y, s.color.Z);

                    if (ImGui.ColorEdit3("Spline Color", ref c))
                    {
                        s.color.X = c.X;
                        s.color.Y = c.Y;
                        s.color.Z = c.Z;
                    }


                    if (ImGui.CollapsingHeader("Curve Type"))
                    {
                        string[] types = { "clamped", "closed" };
                        int current = (int)s.type - 1;

                        for (int i = 0; i < types.Length; i++)
                        {
                            if (ImGui.Selectable(types[i], current == i))
                            {
                                s.change_type((BSplineType)(i + 1));
                            }
                        }
                    }

                    if (ImGui.CollapsingHeader("Control Points"))
                    {
                        Vector2 tmp;
                        for(int i = 0; i < s.P.Count; i++)
                        {
                            tmp = Util.Conv(s.P[i]);
                            if(ImGui.InputFloat2("P"+i.ToString(), ref tmp))
                            {
                                s.P[i] = Util.Conv(tmp);
                            }

                        }
                        ImGui.Checkbox("Edit Mode", ref s.edit);
                    }
                    if (ImGui.CollapsingHeader("Knot Vector"))
                    {
                        for (int i = 0; i < s.U.Count; i++)
                        {
                            float tmp = s.U[i];
                            if(ImGui.SliderFloat("T" + i.ToString(), ref tmp, 0f, 1f))
                            {
                                s.U[i] = tmp;
                            }
                        }
                    }
                }
            }
            ImGui.End();
        }

        public override void Update()
        {
        }

        public override void Resize(int x, int y)
        {

        }

        public override void Destroy()
        {

        }
    }
}
