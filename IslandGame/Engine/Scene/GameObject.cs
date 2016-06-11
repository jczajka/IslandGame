using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandGame.Engine.Scene {

   public class GameObject {

        private List<GameObject> children;
        private List<GameComponent> components;

        public GameObject() {
            children = new List<GameObject>();
            components = new List<GameComponent>();
        }        

        public GameObject Add(GameObject go) {
            children.Add(go);
            return this;
        }

        public GameObject Remove(GameObject go) {
            children.Remove(go);
            return this;
        }

        public GameObject Add(GameComponent gc) {
            components.Add(gc);
            return this;
        }

        public GameObject Remove(GameComponent gc) {
            components.Remove(gc);
            return this;
        }

        public void Update() {
            foreach(GameComponent gc in components) {
                gc.Update(this);
            }
            foreach(GameObject go in children) {
                go.Update();
            }
        }

        public void Foreach<T>(Action<GameObject, T> callback) where T : GameComponent{
            foreach(GameComponent gc in components) {
                if(gc is T) {
                    callback(this, (T)gc);
                }
            }
            foreach(GameObject go in children) {
                go.Foreach<T>(callback);
            }
        }

        public List<T> GetComponents<T>() where T : GameComponent {
            List<T> result = new List<T>();
            foreach(GameComponent gc in components) {
                if(gc is T) {
                    result.Add((T)gc);
                }
            }
            return result;
        }

        public int GetCount<T>() where T : GameComponent{
            int nr = 0;
            foreach(GameComponent gc in components) {
                if(gc is T) {
                    nr++;
                }
            }
            foreach(GameObject go in children) {
                nr += go.GetCount<T>();
            }
            return nr;
        }

    }

    public interface GameComponent {

        void Update(GameObject parent);

    }

}
