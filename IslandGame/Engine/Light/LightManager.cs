using IslandGame.Engine.OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace IslandGame.Engine.Light {
    
    class LightManager {

        private GLSLProgram skyboxshader;
        private GLSLProgram ambientlightshader;

        private DirectionalLightRenderer dlrenderer;
        private PointLightRenderer plrenderer;
        private SpotLightRenderer slrenderer;
        private List<ILightRenderer> lightrenderer = new List<ILightRenderer>();



        public LightManager() {
            skyboxshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/SkyBoxVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/SkyBoxFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            ambientlightshader = new GLSLProgram()
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/AmbientLightFragment.glsl", ShaderType.FragmentShader))
               .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
               .Link();

            dlrenderer = new DirectionalLightRenderer();
            lightrenderer.Add(dlrenderer);
            plrenderer = new PointLightRenderer();
            lightrenderer.Add(plrenderer);
            slrenderer = new SpotLightRenderer();
            lightrenderer.Add(slrenderer);

        }

        public void RenderLight(Camera camera, Texture2D gPosition, Texture2D gNormal, Texture2D gAlbedo) {

            
            //GL.Clear(ClearBufferMask.ColorBufferBit);

            skyboxshader.Bind();
            Matrix4 view = camera.ViewMatrix;
            skyboxshader.SetUniform("view_matrix", false, ref view);
            GL.DepthMask(false);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DepthMask(true);


            gPosition.Bind(TextureUnit.Texture0);
            gNormal.Bind(TextureUnit.Texture1);
            gAlbedo.Bind(TextureUnit.Texture2);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            ambientlightshader.Bind();
            ambientlightshader.SetUniform("lightColor", 0.05f, 0.05f, 0.05f);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            foreach (ILightRenderer r in lightrenderer) {
                r.RenderLight(camera);
            }

            GL.Disable(EnableCap.Blend);
        }

        public void Resize(int width, int height) {
            foreach (ILightRenderer r in lightrenderer) {
                r.Resize(width, height);
            }
        }

        public void Delete() {
            skyboxshader.Dispose();
            ambientlightshader.Dispose();
            foreach (ILightRenderer r in lightrenderer) {
                r.Delete();
            }
        }

        public void AddLight(DirectionalLight l) {
            dlrenderer.AddLight(l);
        }

        public void AddLight(PointLight l) {
            plrenderer.AddLight(l);
        }

        public void AddLight(SpotLight l) {
            slrenderer.AddLight(l);
        }

        public void RemoveLight(DirectionalLight l) {
            dlrenderer.RemoveLight(l);
        }

        public void RemoveLight(PointLight l) {
            plrenderer.RemoveLight(l);
        }

        public void RemoveLight(SpotLight l) {
            slrenderer.RemoveLight(l);
        }

        public void AddDynamicLight(DirectionalLight l) {
            dlrenderer.AddDynamicLight(l);
        }

        public void AddDynamicLight(PointLight l) {
            plrenderer.AddDynamicLight(l);
        }

        public void AddDynamicLight(SpotLight l) {
            slrenderer.AddDynamicLight(l);
        }

        public void RemoveDynamicLight(DirectionalLight l) {
            dlrenderer.RemoveDynamicLight(l);
        }

        public void RemoveDynamicLight(PointLight l) {
            plrenderer.RemoveDynamicLight(l);
        }

        public void RemoveDynamicLight(SpotLight l) {
            slrenderer.RemoveDynamicLight(l);
        }

        public int LightCount {
            get {
                int lc = 0;
                foreach(ILightRenderer lr in lightrenderer) {
                    lc += lr.LightCount;
                }
                return lc;
            }
        }

        public PointLightRenderer PointLightRenderer {
            get {
                return plrenderer;
            }
        }

        public SpotLightRenderer SpotLightRenderer {
            get {
                return slrenderer;
            }
        }

        public DirectionalLightRenderer DirectionalLightRenderer {
            get {
                return dlrenderer;
            }
        }

    }

}
