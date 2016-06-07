using OpenTK;
using System;

namespace IslandGame.Engine.Light {

    public class DirectionalLight {

        Vector3 direction;
        Vector3 color;

        public DirectionalLight(Vector3 direction) {
            this.direction = direction;
            this.color = new Vector3(1, 1, 1);
        }

        public Vector3 Direction {
            get {
                return direction;
            }
            set {
                direction = value;
            }
        }

        public Vector3 Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public DirectionalLight SetColor(Vector3 color) {
            this.color = color;
            return this;
        }

        public DirectionalLight SetColor(float r, float g, float b) {
            this.color = new Vector3(r, g, b);
            return this;
        }

        public float[] Data {
            get {
                return new float[] { direction.X, direction.Y, direction.Z, 0, color.X, color.Y, color.Z, 0};
            }
        }
    }

    public class PointLight {
        private Vector3 position;
        private Vector3 color;
        private float constant;
        private float linear;
        private float quadratic;

        public PointLight(Vector3 position) {
            this.position = position;
            this.color = new Vector3(1, 1, 1);
            this.constant = 1f;
            this.linear = 0.05f;
            this.quadratic = 0.005f;
        }

        public Vector3 Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public Vector3 Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public float Constant {
            get {
                return constant;
            }
            set {
                constant = value;
            }
        }

        public float Linear {
            get {
                return linear;
            }
            set {
                linear = value;
            }
        }

        public float Quadratic {
            get {
                return quadratic;
            }
            set {
                quadratic = value;
            }
        }

        public PointLight SetColor(Vector3 color) {
            this.color = color;
            return this;
        }

        public PointLight SetColor(float r, float g, float b) {
            this.color = new Vector3(r, g, b);
            return this;
        }

        public PointLight SetCLQ(float constant, float linear, float quadratic) {
            this.constant = constant;
            this.linear = linear;
            this.quadratic = quadratic;
            return this;
        }

        public float[] Data {
            get {
                float radius  = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / 1) * Math.Max(Math.Max(color.X, color.Y), color.Z)))) / (2 * quadratic);
                return new float[] { position.X, position.Y, position.Z, radius, color.X, color.Y, color.Z, constant, linear, quadratic, 0, 0};
            }
        }
    }

    public class SpotLight {

        private Vector3 direction;
        private Vector3 position;
        private Vector3 color;
        private float constant;
        private float linear;
        private float quadratic;
        private float innercutoff;
        private float outercutoff;

        public SpotLight(Vector3 position, Vector3 direction) {
            this.position = position;
            this.direction = direction;
            this.color = new Vector3(1, 1, 1);
            this.constant = 1f;
            this.linear = 0.01f;
            this.quadratic = 0.002f;
            this.innercutoff = 0.9f;
            this.outercutoff = 0.8f;
        }
        

        public Vector3 Direction {
            get {
                return direction;
            }
            set {
                direction = value;
            }
        }

        public float InnerCutOff {
            get {
                return innercutoff;
            }
            set {
                innercutoff = value;
            }
        }

        public float OuterCutOff {
            get {
                return outercutoff;
            }
            set {
                outercutoff = value;
            }
        }

        public Vector3 Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public Vector3 Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public float Constant {
            get {
                return constant;
            }
            set {
                constant = value;
            }
        }

        public float Linear {
            get {
                return linear;
            }
            set {
                linear = value;
            }
        }

        public float Quadratic {
            get {
                return quadratic;
            }
            set {
                quadratic = value;
            }
        }

        public SpotLight SetColor(Vector3 color) {
            this.color = color;
            return this;
        }

        public SpotLight SetColor(float r, float g, float b) {
            this.color = new Vector3(r, g, b);
            return this;
        }

        public SpotLight SetCLQ(float constant, float linear, float quadratic) {
            this.constant = constant;
            this.linear = linear;
            this.quadratic = quadratic;
            return this;
        }

        public SpotLight SetCutOff(float inner, float outer) {
            this.innercutoff = inner;
            this.outercutoff = outer;
            return this;
        }

        public float[] Data {
            get {
                float radius = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / 1) * Math.Max(Math.Max(color.X, color.Y), color.Z)))) / (2 * quadratic);
                return new float[] { position.X, position.Y, position.Z, radius, color.X, color.Y, color.Z, constant, linear, quadratic, innercutoff, outercutoff, direction.X, direction.Y, direction.Z, 0 };
            }
        }
    }

    /**abstract class Light {

        private Vector3 color = new Vector3(1,1,1);

        public abstract void Apply(Vector3 camerapos, Matrix4 mvp);

        public Vector3 Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public Light SetColor(Vector3 color) {
            this.color = color;
            return this;
        }

        public Light SetColor(float r, float g, float b) {
            this.color = new Vector3(r,g,b);
            return this;
        }

    }

    class DirectionalLight : Light {

        private static GLSLProgram shader;

        public static void Setup() {
            shader = new GLSLProgram()
                .AttachShaderAndDelete(new GLSLShader(Resources.PostProgressVertex, ShaderType.VertexShader))
                .AttachShaderAndDelete(new GLSLShader(Resources.DirectionalLightFragment, ShaderType.FragmentShader))
                .AttachShaderAndDelete(new GLSLShader(Resources.GammaCorrection, ShaderType.FragmentShader))
                .Link();
        }

        public static void Delete() {
            shader.Delete();
        }

        private Vector3 direction;

        public DirectionalLight(Vector3 direction) {
            this.direction = direction;
        }

        public Vector3 Direction {
            get {
                return direction;
            }
            set {
                direction = value;
            }
        }

        public override void Apply(Vector3 camerapos, Matrix4 mvp) {
            shader.Bind();
            shader.SetUniform("viewPos", camerapos);
            shader.SetUniform("lightDir", Direction);
            shader.SetUniform("lightColor", Color);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

    }

    class PointLight : Light {
    
        private static GLSLProgram shader;
    
        public static void Setup() {
            shader = new GLSLProgram()
                .AttachShaderAndDelete(new GLSLShader(Resources.PointLightVertex, ShaderType.VertexShader))
                .AttachShaderAndDelete(new GLSLShader(Resources.PointLightFragment, ShaderType.FragmentShader))
                .AttachShaderAndDelete(new GLSLShader(Resources.GammaCorrection, ShaderType.FragmentShader))
                .Link();
        }
    
        public static void Delete() {
            shader.Delete();
        }

        public static void Resize(float width, float height) {
            shader.Bind();
            shader.SetUniform("screen", width, height);
        }

        private Vector3 position;
        private float constant = 1f;
        private float linear = 0.05f;
        private float quadratic = 0.005f;
    
        public PointLight(Vector3 position) {
            this.position = position;
        }
        
        public PointLight SetCLQ(float constant, float linear, float quadratic) {
            this.linear = linear;
            this.constant = constant;
            this.quadratic = quadratic;
            return this;
        }

        public float Constant {
            get {
                return constant;
            }
            set {
                constant = value;
            }
        }

        public float Linear {
            get {
                return linear;
            }
            set {
                linear = value;
            }
        }

        public float Quadratic {
            get {
                return quadratic;
            }
            set {
                quadratic = value;
            }
        }

        public Vector3 Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }
        

        public override void Apply(Vector3 camerapos, Matrix4 mvp) {
            shader.Bind();

            shader.SetUniform("mvp", false, ref mvp);
            shader.SetUniform("viewPos", camerapos);
            shader.SetUniform("lightPosition", position);
            shader.SetUniform("lightColor", Color);
            shader.SetUniform("lightCLQ", constant, linear, quadratic);
            float lightMax = Math.Max(Math.Max(Color.X, Color.Y), Color.Z);
            float radius = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / 1) * lightMax))) / (2 * quadratic);
            shader.SetUniform("radius", radius);
            //shader.SetUniform("debug", 0);
            //GL.Disable(EnableCap.CullFace);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //shader.SetUniform("debug", 1);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            //GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.Enable(EnableCap.CullFace);
        }
    
    }

    class SpotLight : Light {

        private static GLSLProgram shader;

        public static void Setup() {
            shader = new GLSLProgram()
                .AttachShaderAndDelete(new GLSLShader(Resources.PostProgressVertex, ShaderType.VertexShader))
                .AttachShaderAndDelete(new GLSLShader(Resources.SpotLightFragment, ShaderType.FragmentShader))
                .AttachShaderAndDelete(new GLSLShader(Resources.GammaCorrection, ShaderType.FragmentShader))
                .Link();
        }

        public static void Delete() {
            shader.Delete();
        }

        private Vector3 position;
        private Vector3 direction;
        private float constant = 1f;
        private float linear = 0.01f;
        private float quadratic = 0.002f;
        private float cutoff = 0.9f;
        private float outercutoff = 0.8f;

        public SpotLight(Vector3 position, Vector3 direction) {
            this.position = position;
            this.direction = direction;
        }

        public SpotLight SetCLQ(float constant, float linear, float quadratic) {
            this.linear = linear;
            this.constant = constant;
            this.quadratic = quadratic;
            return this;
        }
        
        public SpotLight SetCutoff(float inner, float outer) {
            this.cutoff = inner;
            this.outercutoff = outer;
            return this;
        }

        public float Constant {
            get {
                return constant;
            }
            set {
                constant = value;
            }
        }

        public float Linear {
            get {
                return linear;
            }
            set {
                linear = value;
            }
        }

        public float Quadratic {
            get {
                return quadratic;
            }
            set {
                quadratic = value;
            }
        }

        public Vector3 Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public Vector3 Direction {
            get {
                return direction;
            }
            set {
                direction = value;
            }
        }

        public float InnerCutoff {
            get {
                return cutoff;
            }
            set {
                this.cutoff = value;
            }
        }

        public float OuterCutoff {
            get {
                return outercutoff;
            }
            set {
                this.outercutoff = value;
            }
        }

        public override void Apply(Vector3 camerapos, Matrix4 mvp) {
            shader.Bind();
            shader.SetUniform("viewPos", camerapos);
            shader.SetUniform("lightPosition", position);
            shader.SetUniform("lightColor", Color);
            shader.SetUniform("lightCLQ", constant, linear, quadratic);
            shader.SetUniform("lightDir", direction);
            shader.SetUniform("lightCutoff", cutoff);
            shader.SetUniform("lightOuterCutoff", outercutoff);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

    }**/

}
