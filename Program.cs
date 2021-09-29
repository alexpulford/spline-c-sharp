using System;

namespace GraphTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Window w = new Window("Spline Renderer");
            Renderer r = new Renderer(w);
            Graph g = new Graph(w.ClientSize.X, w.ClientSize.Y);
            UI ui = new UI(g);

            r.Add(ui);
            r.Add(g);
            
            w.Run();
        }
    }
}
