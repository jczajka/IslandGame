using OpenTK;
using System;
using System.Drawing;
using OpenTK.Input;
using System.Windows.Forms;
using IslandGame.Engine.Light;
using IslandGame.Engine;
using IslandGame.Engine.OpenGL;

namespace IslandGame {

    class IslandGame : SimpleGame{
        
        private DirectionalLight sun;
        
        private double time;

        private ModelInformation monkey;
        private Geometry specialmonkey;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            
            Keyboard.KeyDown += (object sender, KeyboardKeyEventArgs key) => {
                if (key.Key == Key.F11) {
                    if (WindowState != WindowState.Fullscreen) {
                        //VSync = VSyncMode.Adaptive;
                        WindowState = WindowState.Fullscreen;
                    } else {
                        //VSync = VSyncMode.Off;
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
                    ModelBatch.RowOrder = !ModelBatch.RowOrder;
                }
                if (key.Key == Key.Tab) {
                    LightManager.PointLightRenderer.Clear();
                    LightManager.DirectionalLightRenderer.Clear();
                    LightManager.SpotLightRenderer.Clear();
                }
                if(key.Key == Key.Space) {
                    ModelBatch.AddGeometry(new Geometry(monkey, new Matrix4(1,0,0,Camera.Position.X,
                                                                            0,1,0,Camera.Position.Y,
                                                                            0,0,1,Camera.Position.Z,
                                                                            0,0,0,1) * Matrix4.CreateFromQuaternion(Camera.Rotation) * Matrix4.CreateRotationY((float)Math.PI)));
                }
            };
            Mouse.ButtonDown += (object sender, MouseButtonEventArgs key) => {
                if (CursorVisible == false) {
                    if (key.Button == MouseButton.Left) {
                        Random rew = new Random();
                        LightManager.AddLight(new PointLight(Camera.Position).SetColor(new Vector3((float)rew.NextDouble(), (float)rew.NextDouble(), (float)rew.NextDouble()) * 4));
                    }
                    if (key.Button == MouseButton.Middle) {
                        Random rew = new Random();
                        LightManager.AddLight(new DirectionalLight(Camera.Forward).SetColor(new Vector3((float)rew.NextDouble(), (float)rew.NextDouble(), (float)rew.NextDouble()) * 0.5f));
                    }
                    if (key.Button == MouseButton.Right) {
                        Random rew = new Random();
                        LightManager.AddLight(new SpotLight(Camera.Position, Camera.Forward).SetColor(new Vector3((float)rew.NextDouble(), (float)rew.NextDouble(), (float)rew.NextDouble()) * 4).SetCLQ(1, 0.09f, 0.0001f));
                    }
                }
            };

            sun = new DirectionalLight(new Vector3(0, -1, 0));
            time = 30;
            LightManager.AddDynamicLight(sun);

            ModelBatch.AddGeometry(new Geometry(ModelBatch.LoadModel(Faces.FromFile(@".\Data\Model\World.lpm"))));
            monkey = ModelBatch.LoadModel(Faces.FromFile(@".\Data\Model\Monkey.lpm"));//new Model(faces)
            ModelBatch.AddGeometry((specialmonkey = new Geometry(monkey, Matrix4.Identity)));
            
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

            if (Keyboard[Key.Q]) time += e.Time * 2;
            if (!Keyboard[Key.X]) Title = "Lights: " + LightManager.LightCount + " | Bloom: " + Bloom + " | FXAA: " + Fxaa;

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
