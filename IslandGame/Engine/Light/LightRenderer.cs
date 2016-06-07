using IslandGame.Engine.OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace IslandGame.Engine.Light {

    public interface ILightRenderer {

        void RenderLight(Camera camera);

        void Resize(int width, int height);

        int LightCount {
            get;
        }

        void Delete();
    }

    public class DirectionalLightRenderer : ILightRenderer {
        private GLSLProgram shader;
        int staticbuffer;
        private List<DirectionalLight> staticlights = new List<DirectionalLight>();
        bool staticupdate = false;
        int dynamicbuffer;
        private List<DirectionalLight> dynamiclights = new List<DirectionalLight>();

        public DirectionalLightRenderer() {
            shader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/PostProgressVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/DirectionalLightFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            staticbuffer = GL.GenBuffer();
            dynamicbuffer = GL.GenBuffer();
        }

        public int LightCount{
            get {
                return staticlights.Count + dynamiclights.Count;
            }
        }

        public void Clear() {
            staticlights.Clear();
            dynamiclights.Clear();
            staticupdate = true;
        }

        public void Update() {
            staticupdate = true;
        }

        public void AddLight(DirectionalLight l) {
            staticlights.Add(l);
            staticupdate = true;
            if (staticlights.Count > 128) System.Console.WriteLine("Too many lights!");
        }

        public void RemoveLight(DirectionalLight l) {
            staticlights.Remove(l);
            staticupdate = true;
        }

        public void AddDynamicLight(DirectionalLight l) {
            dynamiclights.Add(l);
            if (dynamiclights.Count > 128) System.Console.WriteLine("Too many lights!");
        }

        public void RemoveDynamicLight(DirectionalLight l) {
            dynamiclights.Remove(l);
        }

        public void RenderLight(Camera camera) {
            shader.Bind();
            shader.SetUniform("viewPos", camera.Position);
            shader.BindUniformBlock("lights", 0);

            if(staticlights.Count > 0) {
                if (staticupdate) {
                    float[] data = new float[staticlights.Count * 8];
                    for(int i = 0; i < staticlights.Count; i++) {
                        float[] lightdata = staticlights[i].Data;
                        for(int j = 0; j < lightdata.Length; j++) {
                            data[i * 8 + j] = lightdata[j];
                        }
                    }

                    GL.BindBuffer(BufferTarget.UniformBuffer, staticbuffer);
                    GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);
                    staticupdate = false;
                }
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, staticbuffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, staticlights.Count);
            }
            if (dynamiclights.Count > 0) {
                float[] data = new float[dynamiclights.Count * 8];
                for (int i = 0; i < dynamiclights.Count; i++) {
                    float[] lightdata = dynamiclights[i].Data;
                    for (int j = 0; j < lightdata.Length; j++) {
                        data[i * 8 + j] = lightdata[j];
                    }
                }
                

                GL.BindBuffer(BufferTarget.UniformBuffer, dynamicbuffer);
                GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.StreamDraw);

                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, dynamicbuffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, dynamiclights.Count);
            }
        }

        public void Resize(int width, int height) {
            
        }

        public void Delete() {
            shader.Dispose();
            GL.DeleteBuffer(dynamicbuffer);
            GL.DeleteBuffer(staticbuffer);
        }
    }

    public class PointLightRenderer : ILightRenderer {
        private GLSLProgram shader;
        int staticbuffer;
        private List<PointLight> staticlights = new List<PointLight>();
        bool staticupdate = false;
        int dynamicbuffer;
        private List<PointLight> dynamiclights = new List<PointLight>();

        public PointLightRenderer() {
            shader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/PointLightVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/PointLightFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            staticbuffer = GL.GenBuffer();
            dynamicbuffer = GL.GenBuffer();
            
        }

        public void Update() {
            staticupdate = true;
        }

        public void Clear() {
            staticlights.Clear();
            dynamiclights.Clear();
            staticupdate = true;
        }

        public int LightCount {
            get {
                return staticlights.Count + dynamiclights.Count;
            }
        }

        public void AddLight(PointLight l) {
            staticlights.Add(l);
            staticupdate = true;
            if (staticlights.Count > 1024) System.Console.WriteLine("Too many lights!");
        }

        public void RemoveLight(PointLight l) {
            staticlights.Remove(l);
            staticupdate = true;
        }

        public void AddDynamicLight(PointLight l) {
            dynamiclights.Add(l);
            if (dynamiclights.Count > 1024) System.Console.WriteLine("Too many lights!");
        }

        public void RemoveDynamicLight(PointLight l) {
            dynamiclights.Remove(l);
        }

        public void RenderLight(Camera camera) {
            shader.Bind();
            shader.SetUniform("viewPos", camera.Position);
            Matrix4 mvp = camera.CameraMatrix;
            shader.SetUniform("mvp", false, ref mvp);
            shader.BindUniformBlock("lights", 0);

            if (staticlights.Count > 0) {
                if (staticupdate) {
                    float[] data = new float[staticlights.Count * 12];
                    for (int i = 0; i < staticlights.Count; i++) {
                        float[] lightdata = staticlights[i].Data;
                        for (int j = 0; j < lightdata.Length; j++) {
                            data[i * 12 + j] = lightdata[j];
                        }
                    }

                    GL.BindBuffer(BufferTarget.UniformBuffer, staticbuffer);
                    GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);
                    staticupdate = false;
                }
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, staticbuffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 36, staticlights.Count);
            }
            if (dynamiclights.Count > 0) {
                float[] data = new float[dynamiclights.Count * 12];
                for (int i = 0; i < dynamiclights.Count; i++) {
                    float[] lightdata = dynamiclights[i].Data;
                    for (int j = 0; j < lightdata.Length; j++) {
                        data[i * 12 + j] = lightdata[j];
                    }
                }


                GL.BindBuffer(BufferTarget.UniformBuffer, dynamicbuffer);
                GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.StreamDraw);

                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, dynamicbuffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 36, dynamiclights.Count);
            }
        }

        public void Resize(int width, int height) {
        }

        public void Delete() {
            shader.Dispose();
            GL.DeleteBuffer(dynamicbuffer);
            GL.DeleteBuffer(staticbuffer);
        }
    }

    public class SpotLightRenderer : ILightRenderer {
        private GLSLProgram shader;
        int staticbuffer;
        private List<SpotLight> staticlights = new List<SpotLight>();
        bool staticupdate = false;
        int dynamicbuffer;
        private List<SpotLight> dynamiclights = new List<SpotLight>();

        public SpotLightRenderer() {
            shader = new GLSLProgram()
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/SpotLightVertex.glsl", ShaderType.VertexShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/Light/SpotLightFragment.glsl", ShaderType.FragmentShader))
                .AttachShaderAndDelete(GLSLShader.FromFile("./Data/Shader/FX/GammaCorrection.glsl", ShaderType.FragmentShader))
                .Link();

            staticbuffer = GL.GenBuffer();
            dynamicbuffer = GL.GenBuffer();
            
        }

        public void Update() {
            staticupdate = true;
        }

        public void Clear() {
            staticlights.Clear();
            dynamiclights.Clear();
            staticupdate = true;
        }

        public int LightCount {
            get {
                return staticlights.Count + dynamiclights.Count;
            }
        }

        public void AddLight(SpotLight l) {
            staticlights.Add(l);
            staticupdate = true;
            if (staticlights.Count > 1024) System.Console.WriteLine("Too many lights!");
        }

        public void RemoveLight(SpotLight l) {
            staticlights.Remove(l);
            staticupdate = true;
        }

        public void AddDynamicLight(SpotLight l) {
            dynamiclights.Add(l);
            if (dynamiclights.Count > 1024) System.Console.WriteLine("Too many lights!");
        }

        public void RemoveDynamicLight(SpotLight l) {
            dynamiclights.Remove(l);
        }

        public void RenderLight(Camera camera) {
            shader.Bind();
            shader.SetUniform("viewPos", camera.Position);
            Matrix4 mvp = camera.CameraMatrix;
            shader.SetUniform("mvp", false, ref mvp);
            shader.BindUniformBlock("lights", 0);

            if (staticlights.Count > 0) {
                if (staticupdate) {
                    float[] data = new float[staticlights.Count * 16];
                    for (int i = 0; i < staticlights.Count; i++) {
                        float[] lightdata = staticlights[i].Data;
                        for (int j = 0; j < lightdata.Length; j++) {
                            data[i * 16 + j] = lightdata[j];
                        }
                    }

                    GL.BindBuffer(BufferTarget.UniformBuffer, staticbuffer);
                    GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);
                    staticupdate = false;
                }
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, staticbuffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 36, staticlights.Count);
            }
            if (dynamiclights.Count > 0) {
                float[] data = new float[dynamiclights.Count * 16];
                for (int i = 0; i < dynamiclights.Count; i++) {
                    float[] lightdata = dynamiclights[i].Data;
                    for (int j = 0; j < lightdata.Length; j++) {
                        data[i * 16 + j] = lightdata[j];
                    }
                }


                GL.BindBuffer(BufferTarget.UniformBuffer, dynamicbuffer);
                GL.BufferData(BufferTarget.UniformBuffer, data.Length * sizeof(float), data, BufferUsageHint.StreamDraw);

                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, dynamicbuffer);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 36, dynamiclights.Count);
            }
        }

        public void Resize(int width, int height) {
        }

        public void Delete() {
            shader.Dispose();
            GL.DeleteBuffer(dynamicbuffer);
            GL.DeleteBuffer(staticbuffer);
        }
    }

}
