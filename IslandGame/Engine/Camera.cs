using OpenTK;
using System;

namespace IslandGame {

    public class Camera {

        private Quaternion rotation;
        private Vector3 position;
        private float fov;
        private float aspect;
        private float zFar;
        private float zNear;

        public Camera(float fov, float aspect, float zNear, float zFar) {
            rotation = new Quaternion(0,0,0);
            position = new Vector3(0,0,0);
            this.zFar = zFar;
            this.zNear = zNear;
            this.fov = fov;
            this.aspect = aspect;
        }

        public Matrix4 CameraMatrix {
            get {
                Matrix4 perspectiveM = Matrix4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);
                Matrix4 rotationM = Matrix4.CreateFromQuaternion(rotation);
                Matrix4 positionM = Matrix4.CreateTranslation(-position);
                return positionM * rotationM * perspectiveM;
            }
        }

        public Matrix4 ViewMatrix {
            get {
                Matrix4 perspectiveM = Matrix4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);
                Matrix4 rotationM = Matrix4.CreateFromQuaternion(rotation);
                return rotationM * perspectiveM;
            }
        }

        public Quaternion Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
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

        public float Fov {
            get {
                return fov;
            }
            set {
                this.fov = value;
            }
        }

        public float Aspect {
            get {
                return aspect;
            }
            set {
                this.aspect = value;
            }
        }

        public float ZNear {
            get {
                return zNear;
            }
            set {
                this.zNear = value;
            }
        }

        public float ZFar {
            get {
                return zFar;
            }
            set {
                this.zFar = value;
            }
        }

        public Vector3 Forward {
            get {
                return rotate(new Vector3(0, 0, -1), Quaternion.Conjugate(rotation)); 
            }
        }

        public Vector3 Back {
            get {
                return rotate(new Vector3(0, 0,  1), Quaternion.Conjugate(rotation));
            }
        }

        public Vector3 Left {
            get {
                return rotate(new Vector3(-1, 0, 0), Quaternion.Conjugate(rotation));
            }
        }

        public Vector3 Right {
            get {
                return rotate(new Vector3( 1, 0, 0), Quaternion.Conjugate(rotation));
            }
        }

        public Vector3 Up {
            get {
                return rotate(new Vector3(0, 1, 0), Quaternion.Conjugate(rotation));
            }
        }

        public Vector3 Down {
            get {
                return rotate(new Vector3(0, -1, 0), Quaternion.Conjugate(rotation));
            }
        }

        private Vector3 rotate(Vector3 vec, Quaternion q) {
            float num = q.X + q.X;
            float num2 = q.Y + q.Y;
            float num3 = q.Z + q.Z;
            float num4 = q.X * num;
            float num5 = q.Y * num2;
            float num6 = q.Z * num3;
            float num7 = q.X * num2;
            float num8 = q.X * num3;
            float num9 = q.Y * num3;
            float num10 = q.W * num;
            float num11 = q.W * num2;
            float num12 = q.W * num3;
            return new Vector3((1.0f - (num5 + num6)) * vec.X + (num7 - num12) * vec.Y + (num8 + num11) * vec.Z,
                               (num7 + num12) * vec.X + (1.0f - (num4 + num6)) * vec.Y + (num9 - num10) * vec.Z,
                               (num8 - num11) * vec.X + (num9 + num10) * vec.Y + (1.0f - (num4 + num5)) * vec.Z);
        }

    }

}
