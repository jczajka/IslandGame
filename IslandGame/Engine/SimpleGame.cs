using IslandGame.Engine.Light;
using IslandGame.Engine.OpenGL;
using IslandGame.Engine.Scene;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace IslandGame.Engine {

    abstract class SimpleGame : GameWindow{
        private Camera camera;
        private GameObject root;

        private GLSLProgram worldshader;
        private GLSLProgram ppshader;
        private GLSLProgram gaussianblurshader;
        private GLSLProgram bloomshader;

        private LightManager lightManager;
        private Vector3 ambientlight = new Vector3(0, 0, 0);

        private ModelBatch model;

        private Framebuffer gBuffer;
        private Renderbuffer gDepth;
        private Texture2D gPosition;
        private Texture2D gNormal;
        private Texture2D gAlbedo;
        private Texture2D gVelocity;

        private Framebuffer lightBuffer;
        private Texture2D lightTexture;

        private Texture2D[] blurTexture = new Texture2D[2];
        private Framebuffer[] blurFramebuffer = new Framebuffer[2];

        private Framebuffer bloomFramebuffer;
        private Texture2D bloomTexture;

        private LightScatteringRenderer lsr;

        private bool bloom = true;
        private bool lightscattering = true;
        private bool fxaa = true;
        private bool wireframe = false;

        public SimpleGame() {
            camera = new Camera(75f * (float)Math.PI / 180, (float)Width / (float)Height, 0.1f, 10000.0f);
            root = new GameObject();

            this.Load += LoadGame;
            this.RenderFrame += RenderGame;
            this.Resize += ResizeGame;
            this.Unload += (object sender, EventArgs e) => {
                ResourceCleaner.CleanUp(true);
            };
            this.UpdateFrame += (object sender, FrameEventArgs e) => {
                ResourceCleaner.CleanUp(false);
                root.Update(e.Time);
                camera.Update();
            };

        } 

        private void LoadGame(object sender, EventArgs e) {

            GL.ClearColor(0, 0, 0, 0);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.CullFace);
            GL.PointSize(5);
            
            worldshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/WorldVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/WorldFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/WorldGeometry.glsl", ShaderType.GeometryShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.VertexShader))
                .Link();

            ppshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            gaussianblurshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GaussianBlurFragment.glsl", ShaderType.FragmentShader))
                .Link();

            bloomshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/BloomFragment.glsl", ShaderType.FragmentShader))
                .Link();
            

            gDepth = new Renderbuffer(Width, Height, RenderbufferStorage.DepthComponent);
            gPosition = new Texture2D(Width, Height, PixelInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float);
            gNormal = new Texture2D(Width, Height, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat);
            gAlbedo = new Texture2D(Width, Height, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte);
            gVelocity = new Texture2D(Width, Height, PixelInternalFormat.Rg16f, PixelFormat.Rg, PixelType.HalfFloat);

            gBuffer = new Framebuffer()
                .AttachRenderBuffer(gDepth, FramebufferAttachment.DepthAttachment)
                .AttachTexture(gPosition, FramebufferAttachment.ColorAttachment0)
                .AttachTexture(gNormal, FramebufferAttachment.ColorAttachment1)
                .AttachTexture(gAlbedo, FramebufferAttachment.ColorAttachment2)
                .AttachTexture(gVelocity, FramebufferAttachment.ColorAttachment3);
            GL.DrawBuffers(4, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 });
            gBuffer.CheckStatus();
            gBuffer.Unbind();

            lightTexture = new Texture2D(Width, Height, PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat);
            lightTexture.SetWarpMode(TextureWrapMode.ClampToEdge);
            lightBuffer = new Framebuffer()
                .AttachTexture(lightTexture, FramebufferAttachment.ColorAttachment0);
            lightBuffer.CheckStatus();
            lightBuffer.Unbind();

            lightManager = new LightManager();
            
            for (int i = 0; i < 2; i++) {
                blurTexture[i] = new Texture2D((int)(Width / 2f), (int)(Height / 2f), PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat);
                blurTexture[i].SetFiltering(TextureMinFilter.Linear, TextureMagFilter.Linear);
                blurFramebuffer[i] = new Framebuffer().AttachTexture(blurTexture[i], FramebufferAttachment.ColorAttachment0);
                if (!blurFramebuffer[i].CheckStatus()) Console.WriteLine("Framebuffer error");
                blurFramebuffer[i].Unbind();
            }

            bloomTexture = new Texture2D((int)(Width / 1f), (int)(Height / 1f), PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat);
            bloomTexture.SetFiltering(TextureMinFilter.Linear, TextureMagFilter.Linear);
            bloomFramebuffer = new Framebuffer().AttachTexture(bloomTexture, FramebufferAttachment.ColorAttachment0);
            if (!bloomFramebuffer.CheckStatus()) Console.WriteLine("Framebuffer error");
            bloomFramebuffer.Unbind();

            model = new ModelBatch();

            lsr = new LightScatteringRenderer(Width, Height);
        }

        

        private void RenderGame(object sender, FrameEventArgs e) {
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if(wireframe) {

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Disable(EnableCap.CullFace);

                Matrix4 mvp = camera.CameraMatrix;
                worldshader.SetUniform("mvp", false, ref mvp);
                worldshader.Bind();
                model.Draw(root);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Enable(EnableCap.CullFace);

            } else {
                
                gBuffer.Bind();

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                worldshader.Bind();
                Matrix4 pmvp = camera.PreviousCameraMatrix;
                worldshader.SetUniform("premvp", false, ref pmvp);
                Matrix4 mvp = camera.CameraMatrix;
                worldshader.SetUniform("mvp", false, ref mvp);
                model.Draw(root);
                root.WorldRender(camera);

                lightBuffer.Bind();
                gPosition.Bind(TextureUnit.Texture0);
                gNormal.Bind(TextureUnit.Texture1);
                gAlbedo.Bind(TextureUnit.Texture2);
                lightManager.RenderLight(camera, root, ambientlight);
                root.LightRender(camera);

                if(lightscattering) {
                    lsr.RenderLightScattering(root, camera);
                }

                if (bloom) {
                    bloomshader.Bind();
                    bloomFramebuffer.Bind();
                    lightTexture.Bind(TextureUnit.Texture0);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                    GL.Viewport(0, 0, (int)(Width / 2f), (int)(Height / 2f));

                    gaussianblurshader.Bind();

                    blurFramebuffer[0].Bind();
                    bloomTexture.Bind(TextureUnit.Texture0);
                    gaussianblurshader.SetUniform("horizontal", 1);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                    blurFramebuffer[1].Bind();
                    blurTexture[0].Bind(TextureUnit.Texture0);
                    gaussianblurshader.SetUniform("horizontal", 0);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                }


                

                blurFramebuffer[0].Unbind();
                GL.Viewport(0, 0, Width, Height);
                ppshader.Bind();
                blurTexture[1].Bind(TextureUnit.Texture1);
                lightTexture.Bind(TextureUnit.Texture0);
                gVelocity.Bind(TextureUnit.Texture2);
                GL.Enable(EnableCap.Blend);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.Disable(EnableCap.Blend);
            }
            
            this.SwapBuffers();


        }

        private void ResizeGame(object sender, EventArgs e) {
            GL.Viewport(0, 0, this.Width, this.Height);
            camera.Aspect = (float)Width / (float)Height;

            gPosition.Resize(Width, Height);
            gNormal.Resize(Width, Height);
            gAlbedo.Resize(Width, Height);
            gDepth.Resize(Width, Height);
            gVelocity.Resize(Width, Height);

            lightTexture.Resize(Width, Height);

            blurTexture[0].Resize((int)(Width / 2f), (int)(Height / 2f));
            blurTexture[1].Resize((int)(Width / 2f), (int)(Height / 2f));

            bloomTexture.Resize(Width, Height);

            lsr.Resize(Width, Height);
        }

        public Camera Camera {
            get {
                return camera;
            }
        }
        
        public LightManager LightManager {
            get {
                return lightManager;
            }
        }

        public bool Bloom {
            get {
                return bloom;
            }
            set {
                bloom = value;
                blurFramebuffer[1].Bind();
                GL.Clear(ClearBufferMask.ColorBufferBit);
                blurFramebuffer[1].Unbind();
            }
        }

        public Vector3 AmbientLight {
            get {
                return ambientlight;
            }
            set {
                ambientlight = value;
            }
        }

        public bool Fxaa {
            get {
                return fxaa;
            }
            set {
                fxaa = value;
                ppshader.Bind();
                ppshader.SetUniform("fxaa", fxaa ? 1 : 0);
            }
        }

        public bool Wireframe {
            get {
                return wireframe;
            }
            set {
                wireframe = value;
            }
        }

        public bool LightScattering {
            get {
                return lightscattering;
            }
            set {
                lightscattering = value;
            }
        }

        public ModelBatch ModelBatch{
            get{
                return model;
            }
        }

        public GameObject Root {
            get {
                return root;
            }
        }

    }

}
