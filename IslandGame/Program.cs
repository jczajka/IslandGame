using IslandGame.Engine.Scene;
using OpenTK;
using System;
using System.Diagnostics;

namespace IslandGame {
    class Program {
        static void Main(string[] args) {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            using (var game = new IslandGame()) {
                game.Title = "Test Game";
                game.VSync = VSyncMode.Off;
                game.Width = 1280;
                game.Height = 720;
                game.X = (DisplayDevice.GetDisplay(DisplayIndex.Default).Width - game.Width) / 2;
                game.Y = (DisplayDevice.GetDisplay(DisplayIndex.Default).Height - game.Height) / 2;
            
                game.Run(60);
            }

            //new Octree(new Vector3(0, 0, 0), 1, 2);
            

        }

    }
}
