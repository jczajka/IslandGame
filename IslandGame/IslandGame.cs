using OpenTK;
using System;
using System.Drawing;
using OpenTK.Input;
using System.Windows.Forms;
using IslandGame.Engine.Light;
using IslandGame.Engine;
using IslandGame.Engine.OpenGL;
using IslandGame.Engine.Scene;

namespace IslandGame {

    class IslandGame : SimpleGame{
        
        private DirectionalLight sun;

        private double time;

        private ModelInformation monkey;
        private ModelRenderer specialmonkey;
        private GameObject monkeynode;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            
            Keyboard.KeyDown += (object sender, KeyboardKeyEventArgs key) => {
                if(key.Key == Key.F12) {
                    if(VSync == VSyncMode.Off) {
                        VSync = VSyncMode.Adaptive;
                    } else {
                        VSync = VSyncMode.Off;
                    }
                }
                if (key.Key == Key.F11) {
                    if (WindowState != WindowState.Fullscreen) {
                        WindowState = WindowState.Fullscreen;
                    } else {
                        WindowState = WindowState.Normal;
                    }
                }
                if (key.Key == Key.F10) {
                    Wireframe = !Wireframe;
                }
                if (key.Key == Key.F9) {
                    Bloom = !Bloom;
                }
                if (key.Key == Key.F8) {
                    Fxaa = !Fxaa;
                }
                if(key.Key == Key.F7) {
                    LightScattering = !LightScattering;
                }
                if(key.Key == Key.F1) {
                    ModelBatch.RowOrder = !ModelBatch.RowOrder;
                }
                if(key.Key == Key.F6) {
                    sun.Color = (sun.Color.Length == 0 ? new Vector3(1,1,1) : new Vector3(0, 0,0));
                }
                if (key.Key == Key.Tab) {
                    Root.Remove(monkeynode);
                    Root.Add(monkeynode = new GameObject());
                }
                if(key.Key == Key.Space) {
                    monkeynode.Add(new ModelRenderer(monkey, new Matrix4(1,0,0,Camera.Position.X,
                                                                         0,1,0,Camera.Position.Y,
                                                                         0,0,1,Camera.Position.Z,
                                                                         0,0,0,1) * Matrix4.CreateFromQuaternion(Camera.Rotation) * Matrix4.CreateRotationY((float)Math.PI)));
                }
            };
            Mouse.ButtonDown += (object sender, MouseButtonEventArgs key) => {
                if (CursorVisible == false) {
                    if (key.Button == MouseButton.Left) {
                        Random rew = new Random();
                        monkeynode.Add(new PointLight(Camera.Position).SetColor(new Vector3((float)rew.NextDouble(), (float)rew.NextDouble(), (float)rew.NextDouble()) * 4));
                    }
                    if (key.Button == MouseButton.Middle) {
                        Random rew = new Random();
                        monkeynode.Add(new DirectionalLight(Camera.Forward).SetColor(new Vector3((float)rew.NextDouble(), (float)rew.NextDouble(), (float)rew.NextDouble()) * 0.5f));
                    }
                    if (key.Button == MouseButton.Right) {
                        Random rew = new Random();
                        monkeynode.Add(new SpotLight(Camera.Position, Camera.Forward).SetCLQ(1, 0.09f, 0.0001f).SetColor(new Vector3((float)rew.NextDouble(), (float)rew.NextDouble(), (float)rew.NextDouble()) * 4));
                    }
                }
            };

            sun = new DirectionalLight(new Vector3(0, -1, 0));
            time = 30;
            Root.Add(sun);
            Root.Add(new LightScatteringComponent(new Vector3(1.0f, 0.8f, 0.3f)));

           
            monkey = ModelBatch.LoadModel(Faces.FromFile(@".\Data\Model\Monkey.lpm"));//new Model(faces)
            Root.Add((specialmonkey = new ModelRenderer(monkey, Matrix4.Identity)));
            Root.Add(new ModelRenderer(ModelBatch.LoadModel(Faces.FromFile(@".\Data\Model\World.lpm"))));
            Root.Add(monkeynode = new GameObject());
            
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            if (Mouse[MouseButton.Left] && CursorVisible == true) {
                CursorVisible = false;
            }
            if (Keyboard[Key.Escape] && CursorVisible == false) {
                CursorVisible = true;
            }
            Vector2 mousepos = Vector2.Zero;
            if (CursorVisible == false) {
                Point p = System.Windows.Forms.Cursor.Position;
                //TODO middle of window instead of screen
                mousepos = new Vector2((float)p.X / (float)Screen.FromPoint(p).Bounds.Width * 2 - 1, (float)p.Y / (float)Screen.FromPoint(p).Bounds.Height * 2 - 1);
                System.Windows.Forms.Cursor.Position = new Point(Screen.FromPoint(p).Bounds.Width / 2, Screen.FromPoint(p).Bounds.Height / 2);
            }
            Camera.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), mousepos.X);
            Camera.Rotation *= Quaternion.FromAxisAngle(Camera.Right, mousepos.Y);

            if (Keyboard[Key.Left])  Camera.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -0.1f);
            if (Keyboard[Key.Right]) Camera.Rotation *= Quaternion.FromAxisAngle(new Vector3(0, 1, 0),  0.1f);
            if (Keyboard[Key.Up])    Camera.Rotation *= Quaternion.FromAxisAngle(Camera.Left, 0.1f);
            if (Keyboard[Key.Down])  Camera.Rotation *= Quaternion.FromAxisAngle(Camera.Left, -0.1f);

            if (Keyboard[Key.W]) Camera.Position += Camera.Forward * (Keyboard[Key.ShiftLeft] ? 600 : 100) * (float)e.Time;
            if (Keyboard[Key.S]) Camera.Position += Camera.Back    * (Keyboard[Key.ShiftLeft] ? 600 : 100) * (float)e.Time;
            if (Keyboard[Key.A]) Camera.Position += Camera.Left    * (Keyboard[Key.ShiftLeft] ? 600 : 100) * (float)e.Time;
            if (Keyboard[Key.D]) Camera.Position += Camera.Right   * (Keyboard[Key.ShiftLeft] ? 600 : 100) * (float)e.Time;

            if (Keyboard[Key.Q]) time += e.Time * 6;
            if (!Keyboard[Key.X]) Title = "Lights: " + Root.GetCount<Light>() + " | Bloom: " + Bloom + " | FXAA: " + Fxaa + " | LightScattering: " + LightScattering;

            sun.Direction = -new Vector3((float)Math.Cos(time / 20), (float)Math.Sin(time / 20), (float)Math.Sin(time / 20) * 0.5f).Normalized();

            Vector3 z = Vector3.Normalize(Camera.Position - new Vector3(0, 200, 0));
            Vector3 x = Vector3.Normalize(Vector3.Cross(new Vector3(0, 1, 0), z));
            Vector3 y = Vector3.Normalize(Vector3.Cross(z, x));

            Matrix4 rot = new Matrix4(new Vector4(x.X, y.X, z.X, 0.0f),
                                      new Vector4(x.Y, y.Y, z.Y, 0.0f),
                                      new Vector4(x.Z, y.Z, z.Z, 0.0f),
                                      Vector4.UnitW);
            
            specialmonkey.Transform = new Matrix4(1, 0, 0, 0,
                                                  0, 1, 0, 200,
                                                  0, 0, 1, 0,
                                                  0, 0, 0, 1) * rot;
            
            //ModelBatch.AddGeometry(new Geometry(monkey, Matrix4.CreateFromQuaternion(Camera.Rotation) * Matrix4.CreateTranslation(Camera.Position)));

        }
        
    }

}
