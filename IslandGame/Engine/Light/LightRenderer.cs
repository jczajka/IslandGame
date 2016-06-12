using IslandGame.Engine.OpenGL;
using IslandGame.Engine.Scene;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace IslandGame.Engine.Light {

    public class GenericLightRenderer : Resource {

        int buffer;

        public GenericLightRenderer() {
            buffer = GL.GenBuffer();
        }

        public void RenderLight<T>(GameObject root, int vertsperdraw) where T : Light {
            int number = root.GetCount<T>();
            if(number > 0) {
                float[] data = null;
                int index = 0;
                root.Foreach<T>((GameObject go, T dl) => {
                    if(index == 0) {
                        data = new float[number * dl.Data.Length];
                    }
                    float[] lightdata = dl.Data;
                    for(int j = 0; j < lightdata.Length; j++) {
                        data[index] = lightdata[j];
                        index++;
                    }
                });

                GL.BindBuffer(BufferTarget.UniformBuffer, buffer);
                GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.StreamDraw);

                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, buffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, vertsperdraw, number);
            }
        }

        public override void Release() {
            GL.DeleteBuffer(buffer);
        }
    }

    public class LightScatteringRenderer : IDisposable{

        private int width, height;
        private GLSLProgram skyboxshader;
        private GLSLProgram radialblurshader;
        private GLSLProgram finalshader;
        private Framebuffer framebuffer;
        private Texture2D colortexture;
        private Framebuffer framebuffer2;
        private Texture2D colortexture2;

        public LightScatteringRenderer(int width, int height) {
            skyboxshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/SkyBoxVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/SunSkyBoxFragment.glsl", ShaderType.FragmentShader))
                .Link();
           
            radialblurshader = new GLSLProgram()
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/RadialBlurFragment.glsl", ShaderType.FragmentShader))
               .Link();

            finalshader = new GLSLProgram()
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/LightScatteringFragment.glsl", ShaderType.FragmentShader))
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
               .Link();

            colortexture = new Texture2D(width/2, height/2, PixelInternalFormat.R16, PixelFormat.Red, PixelType.UnsignedByte);
            colortexture.SetFiltering(TextureMinFilter.Linear, TextureMagFilter.Linear);
            framebuffer = new Framebuffer()
                .AttachTexture(colortexture, FramebufferAttachment.ColorAttachment0);
            framebuffer.CheckStatus();
            framebuffer.Unbind();

            colortexture2 = new Texture2D(width / 4, height / 4, PixelInternalFormat.R16, PixelFormat.Red, PixelType.UnsignedByte);
            colortexture2.SetFiltering(TextureMinFilter.Linear, TextureMagFilter.Linear);
            framebuffer2 = new Framebuffer()
                .AttachTexture(colortexture2, FramebufferAttachment.ColorAttachment0);
            framebuffer2.CheckStatus();
            framebuffer2.Unbind();

            this.width = width;
            this.height = height;

        }

        public void RenderLightScattering(GameObject root, Camera camera) {
            
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            root.Foreach<LightScatteringComponent>((GameObject go, LightScatteringComponent ls) => {
                foreach(DirectionalLight dl in go.GetComponents<DirectionalLight>()){
                    Framebuffer current = Framebuffer.Current;

                    framebuffer.Bind();
                    GL.Viewport(0, 0, width / 2, height / 2);
                    skyboxshader.Bind();
                    skyboxshader.SetUniform("lightDir", dl.Direction);
                    Matrix4 view = camera.ViewMatrix;
                    skyboxshader.SetUniform("view_matrix", false, ref view);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);


                    framebuffer2.Bind();
                    GL.Viewport(0, 0, width / 4, height / 4);
                    radialblurshader.Bind();
                    
                    Vector4 orlightpos = Vector4.Transform(new Vector4(dl.Direction.Normalized(), 1), view);
                    Vector2 lightpos = orlightpos.Xy / orlightpos.W;
                    lightpos += new Vector2(1, 1);
                    lightpos /= 2;
                    
                    radialblurshader.SetUniform("lightpos", lightpos);
                    colortexture.Bind(TextureUnit.Texture4);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);


                    current.Bind();
                    GL.Enable(EnableCap.Blend);
                    GL.Viewport(0, 0, width, height);
                    finalshader.Bind();
                    finalshader.SetUniform("lightcolor", ls.HasSpecialColor() ? ls.RayColor : dl.Color);
                    colortexture2.Bind(TextureUnit.Texture5);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                    GL.Disable(EnableCap.Blend);
                }
            });

            
        }

        public void Resize(int width, int height) {
            colortexture.Resize(width/2, height/2);
            colortexture2.Resize(width / 4, height / 4);
            this.width = width;
            this.height = height;
        }

        public void Dispose() {
            skyboxshader.Dispose();
            radialblurshader.Dispose();
            finalshader.Dispose();
            framebuffer.Dispose();
            colortexture.Dispose();
            framebuffer2.Dispose();
            colortexture2.Dispose();
        }

    }

}
