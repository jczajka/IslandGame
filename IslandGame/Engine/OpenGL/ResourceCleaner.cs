using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IslandGame.Engine.OpenGL {

    class ResourceCleaner {

        private static List<Resource> trash = new List<Resource>();

        public static void Add(Resource resource) {
            trash.Add(resource);
        }

        public static void CleanUp(bool all) {
            int num = trash.Count;
            foreach(Resource d in trash) {
                if (all || !d.InUsage()) {
                    d.Release();
                    d.Dispose();
                    num--;
                    Trace.TraceInformation("Resources: " + num + " resources left");
                }
            }
            trash = trash.Where(x => x.InUsage()).ToList();
        }

    }

    public abstract class Resource : IDisposable {

        private bool used;

        public Resource() {
            used = true;
            ResourceCleaner.Add(this);
        }

        ~Resource() {
            Dispose();
        }

        public bool InUsage() {
            return used;
        }

        public void Dispose() {
            used = false;
        }

        public abstract void Release();
    }

}
