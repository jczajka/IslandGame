using IslandGame.Engine.OpenGL;
using IslandGame.Engine.Scene;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace IslandGame.Engine.Light {
    
    class LightManager {

        private GLSLProgram skyboxshader;
        private GLSLProgram ambientlightshader;
        private GLSLProgram directionallightshader;
        private GLSLProgram pointlightshader;
        private GLSLProgram spotlightshader;

        private GenericLightRenderer lightrenderer;

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

            directionallightshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/DirectionalLightFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            pointlightshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/PointLightVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/PointLightFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            spotlightshader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/SpotLightVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/SpotLightFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            lightrenderer = new GenericLightRenderer();

        }

        public void RenderLight(Camera camera, GameObject root) {
            
            Matrix4 view = camera.ViewMatrix;
            Matrix4 mvp = camera.CameraMatrix;

            skyboxshader.Bind();
            skyboxshader.SetUniform("view_matrix", false, ref view);
            GL.DepthMask(false);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DepthMask(true);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            ambientlightshader.Bind();
            ambientlightshader.SetUniform("lightColor", 0.05f, 0.05f, 0.05f);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            directionallightshader.Bind();
            directionallightshader.SetUniform("viewPos", camera.Position);
            directionallightshader.BindUniformBlock("lights", 0);
            lightrenderer.RenderLight<DirectionalLight>(root, 6);

            pointlightshader.Bind();
            pointlightshader.SetUniform("viewPos", camera.Position);
            pointlightshader.SetUniform("mvp", false, ref mvp);
            pointlightshader.BindUniformBlock("lights", 0);
            lightrenderer.RenderLight<PointLight>(root, 36);

            spotlightshader.Bind();
            spotlightshader.SetUniform("viewPos", camera.Position);
            spotlightshader.SetUniform("mvp", false, ref mvp);
            spotlightshader.BindUniformBlock("lights", 0);
            lightrenderer.RenderLight<SpotLight>(root, 36);

            GL.Disable(EnableCap.Blend);
        }

    }

}
