using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace IslandGame.Engine.OpenGL {

    public class GLSLProgram : Resource { 

        private int id;
        private Dictionary<string, int> uniforms;

        public GLSLProgram() {
            id = GL.CreateProgram();
            uniforms = new Dictionary<string, int>();
        }

        public GLSLProgram Link() {
            GL.LinkProgram(id);

            int status;
            GL.GetProgram(id, GetProgramParameterName.LinkStatus, out status);

            if (status != 1) {
                string info;
                GL.GetProgramInfoLog(id, out info);
                Trace.TraceError(info);
                Dispose();
            }

            return this;
        }

        public GLSLProgram Bind() {
            GL.UseProgram(id);
            return this;
        }

        public GLSLProgram AttachShader(GLSLShader shader) {
            GL.AttachShader(id, shader.ID);
            return this;
        }

        public GLSLProgram AttachShaderAndDelete(GLSLShader shader) {
            GL.AttachShader(id, shader.ID);
            shader.Dispose();
            return this;
        }

        public override void Release() {
            GL.DeleteProgram(id);
            uniforms.Clear();
        }

        public int ID {
            get {
                return id;
            }
        }

        public int GetUniform(string name) {
            if (!uniforms.ContainsKey(name)) {
                uniforms.Add(name, GL.GetUniformLocation(id, name));
            }
            return uniforms[name];
        }

        public int GetUniformBlock(string name) {
            if (!uniforms.ContainsKey("ub_" + name)) {
                uniforms.Add("ub_" + name, GL.GetUniformBlockIndex(id, name));
            }
            return uniforms["ub_" + name];
        }

        public void BindUniformBlock(string name, int slot) {
            BindUniformBlock(GetUniformBlock(name), slot);
        }

        public void BindUniformBlock(int location, int slot) {
            GL.UniformBlockBinding(id, location, slot);
        }

        #region UNIFORMS_LOCATION

        public void SetUniform(int location, int value) {
            GL.Uniform1(location, value);
        }

        public void SetUniform(int location, float value) {
            GL.Uniform1(location, value);
        }

        public void SetUniform(int location, int xValue, int yValue) {
            GL.Uniform2(location, xValue, yValue);
        }

        public void SetUniform(int location, float xValue, float yValue) {
            GL.Uniform2(location, xValue, yValue);
        }

        public void SetUniform(int location, Vector2 vec) {
            GL.Uniform2(location, vec);
        }

        public void SetUniform(int location, int xValue, int yValue, int zValue) {
            GL.Uniform3(location, xValue, yValue, zValue);
        }

        public void SetUniform(int location, float xValue, float yValue, float zValue) {
            GL.Uniform3(location, xValue, yValue, zValue);
        }

        public void SetUniform(int location, Vector3 vec) {
            GL.Uniform3(location, vec);
        }

        public void SetUniform(int location, float xValue, float yValue, float zValue, float wValue) {
            GL.Uniform4(location, xValue, yValue, zValue, wValue);
        }

        public void SetUniform(int location, int xValue, int yValue, int zValue, int wValue) {
            GL.Uniform4(location, xValue, yValue, zValue, wValue);
        }

        public void SetUniform(int location, Vector4 vec) {
            GL.Uniform4(location, vec);
        }

        public void SetUniform(int location, Color color) {
            GL.Uniform4(location, color);
        }

        public void SetUniform(int location, bool transpose, Matrix4 matrix) {
            GL.UniformMatrix4(location, transpose, ref matrix);
        }

        public void SetUniform(int location, bool transpose, Matrix3 matrix) {
            GL.UniformMatrix3(location, transpose, ref matrix);
        }

        public void SetUniform(int location, bool transpose, Matrix2 matrix) {
            GL.UniformMatrix2(location, transpose, ref matrix);
        }

        #endregion

        #region UNIFORMS_NAME

        public void SetUniform(string name, int value) {
            SetUniform(GetUniform(name), value);
        }

        public void SetUniform(string name, float value) {
            SetUniform(GetUniform(name), value);
        }

        public void SetUniform(string name, int xValue, int yValue) {
            SetUniform(GetUniform(name), xValue, yValue);
        }

        public void SetUniform(string name, float xValue, float yValue) {
            SetUniform(GetUniform(name), xValue, yValue);
        }

        public void SetUniform(string name, Vector2 vec) {
            SetUniform(GetUniform(name), vec);
        }

        public void SetUniform(string name, int xValue, int yValue, int zValue) {
            SetUniform(GetUniform(name), xValue, yValue, zValue);
        }

        public void SetUniform(string name, float xValue, float yValue, float zValue) {
            SetUniform(GetUniform(name), xValue, yValue, zValue);
        }

        public void SetUniform(string name, Vector3 vec) {
            SetUniform(GetUniform(name), vec);
        }

        public void SetUniform(string name, float xValue, float yValue, float zValue, float wValue) {
            SetUniform(GetUniform(name), xValue, yValue, zValue, wValue);
        }

        public void SetUniform(string name, int xValue, int yValue, int zValue, int wValue) {
            SetUniform(GetUniform(name), xValue, yValue, zValue, wValue);
        }

        public void SetUniform(string name, Vector4 vec) {
            SetUniform(GetUniform(name), vec);
        }

        public void SetUniform(string name, bool transpose, ref Matrix4 matrix) {
            SetUniform(GetUniform(name), transpose, matrix);
        }

        public void SetUniform(string name, bool transpose, ref Matrix3 matrix) {
            SetUniform(GetUniform(name), transpose, matrix);
        }

        public void SetUniform(string name, bool transpose, ref Matrix2 matrix) {
            SetUniform(GetUniform(name), transpose, matrix);
        }

        public void SetUniform(string name, Color color) {
            SetUniform(GetUniform(name), color);
        }

        #endregion

    }

    public class GLSLShader : Resource{

        private int id;

        public GLSLShader(string source, ShaderType type) {
            id = GL.CreateShader(type);

            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            int status;
            GL.GetShader(id, ShaderParameter.CompileStatus, out status);

            if (status != 1) {
                string info;
                GL.GetShaderInfoLog(id, out info);
                Trace.TraceError(info);
                Dispose();
            }

        }
        
        public int ID {
            get {
                return id;
            }
        }


        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        public static GLSLShader FromFile(string file, ShaderType type) {
            if (!cache.ContainsKey(file)) {
                try {
                    using (StreamReader sr = new StreamReader(file)) {
                        string line = sr.ReadToEnd();
                        cache.Add(file, line);
                    }
                } catch (Exception e) {
                    throw e;
                }
            }
            return new GLSLShader(cache[file], type);
        }

        public override void Release() {
            GL.DeleteShader(id);
        }
        
    }

}
