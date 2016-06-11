using IslandGame.Engine.Scene;
using OpenTK;
using System;

namespace IslandGame.Engine.Light {

    public abstract class Light : GameComponent {

        Vector3 color;

        public Light() {
            this.color = new Vector3(1, 1, 1);
        }

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
            this.color = new Vector3(r, g, b);
            return this;
        }

        public abstract float[] Data {
            get;
        }
        
        public void Update(GameObject parent) {
            
        }

    }


    public class DirectionalLight : Light{

        Vector3 direction;

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

        public override float[] Data {
            get {
                return new float[] { direction.X, direction.Y, direction.Z, 0, Color.X, Color.Y, Color.Z, 0 };
            }
        }
    }

    public class PointLight : Light{

        private Vector3 position;
        private float constant;
        private float linear;
        private float quadratic;

        public PointLight(Vector3 position) {
            this.position = position;
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

        public PointLight SetCLQ(float constant, float linear, float quadratic) {
            this.constant = constant;
            this.linear = linear;
            this.quadratic = quadratic;
            return this;
        }

        public override float[] Data {
            get {
                float radius  = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / 1) * Math.Max(Math.Max(Color.X, Color.Y), Color.Z)))) / (2 * quadratic);
                return new float[] { position.X, position.Y, position.Z, radius, Color.X, Color.Y, Color.Z, constant, linear, quadratic, 0, 0};
            }
        }
    }

    public class SpotLight : Light{

        private Vector3 direction;
        private Vector3 position;
        private float constant;
        private float linear;
        private float quadratic;
        private float innercutoff;
        private float outercutoff;

        public SpotLight(Vector3 position, Vector3 direction) {
            this.position = position;
            this.direction = direction;
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

        public override float[] Data {
            get {
                float radius = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0 / 1) * Math.Max(Math.Max(Color.X, Color.Y), Color.Z)))) / (2 * quadratic);
                return new float[] { position.X, position.Y, position.Z, radius, Color.X, Color.Y, Color.Z, constant, linear, quadratic, innercutoff, outercutoff, direction.X, direction.Y, direction.Z, 0 };
            }
        }
    }

}
