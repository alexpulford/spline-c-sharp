using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using System.Numerics;

namespace GraphTool
{
    partial class Grid
    {

        
        partial void Overlay(System.Drawing.Rectangle r)
        {
            Vector2 size = new Vector2(r.Width, r.Height);
            Vector2 off = new Vector2(r.X, r.Y);

            ImGui.SetNextWindowPos(off);
            ImGui.SetNextWindowSize(size);
            ImGui.SetNextWindowBgAlpha(0.0f);
            if (!ImGui.Begin("Overlay", ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs))
            {
                ImGui.End();
                return;
            }
            decimal start = (decimal)(g.rect.Min.X - (g.rect.Min.X % major));

            Vector2 half = new Vector2(size.X/2, size.Y/2);

            float yPos = Math.Clamp(half.Y + (g.rect.Center.Y / g.rect.Size.Y) * size.Y, 0, size.Y);
            float xPos = Math.Clamp(half.X + (g.rect.Center.X / g.rect.Size.X) * size.X, 0, size.X);

            while (start < (decimal)g.rect.Max.X)
            {
                String txt = start.ToString();

                Vector2 v = new Vector2(((float)start - g.rect.Min.X) * (size.X / (g.rect.Max.X - g.rect.Min.X)), yPos);

                if(yPos == size.Y)
                {
                    v.Y += -ImGui.CalcTextSize(txt).Y;
                }
                v.X += 3;

                ImGui.SetCursorPos(v);
                ImGui.Text(txt);
                start += (decimal)major;
            }

            start = (decimal)(g.rect.Min.Y - (g.rect.Min.Y % major));

            while (start < (decimal)g.rect.Max.Y)
            {
                String txt = start.ToString();

                Vector2 v = new Vector2(size.X-xPos, size.Y - (((float)start - g.rect.Min.Y) * (size.Y / (g.rect.Max.Y - g.rect.Min.Y))));

                v.X += 3;

                if(xPos == 0)
                {
                    v.X -= 6+ImGui.CalcTextSize(txt).X;
                }


                ImGui.SetCursorPos(v);
                ImGui.Text(txt);
                start += (decimal)major;
            }

            ImGui.End();
        }
    }
}
