using OpenTK;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace IslandGame.Engine.OpenGL {

    class ModelBatch : Resource {

        private bool roworder = false;

        private int vao;
        private int vbo;
        private int ibo;
        private List<float> data;

        List<Geometry> geometry;

        public ModelBatch() {
            
            data = new List<float>();
            geometry = new List<Geometry>();
            
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ibo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0 * sizeof(float));
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));


            GL.BindBuffer(BufferTarget.ArrayBuffer, ibo);

            GL.EnableVertexAttribArray(10);
            GL.EnableVertexAttribArray(11);
            GL.EnableVertexAttribArray(12);
            GL.EnableVertexAttribArray(13);

            GL.VertexAttribPointer(10, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float),  0 * sizeof(float));
            GL.VertexAttribPointer(11, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float),  4 * sizeof(float));
            GL.VertexAttribPointer(12, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float),  8 * sizeof(float));
            GL.VertexAttribPointer(13, 4, VertexAttribPointerType.Float, false, 16 * sizeof(float), 12 * sizeof(float));

            GL.VertexAttribDivisor(10, 1);
            GL.VertexAttribDivisor(11, 1);
            GL.VertexAttribDivisor(12, 1);
            GL.VertexAttribDivisor(13, 1);

            GL.BindVertexArray(0);

        }

        public bool RowOrder {
            get {
                return roworder;
            }
            set {
                this.roworder = value;
            }
        }

        public ModelInformation LoadModel(Face[] faces) {
            ModelInformation mi = new ModelInformation(data.Count / 8, faces.Length * 3);

            for (int i = 0; i < faces.Length; i++) {
                for (int f = 0; f < 3; f++) {
                    data.Add(faces[i].v[f].X);
                    data.Add(faces[i].v[f].Y);
                    data.Add(faces[i].v[f].Z);
                    
                    data.Add(faces[i].c0);
                    data.Add(faces[i].c1);
                    data.Add(faces[i].c2);
                    
                    data.Add(faces[i].specularity);
                    data.Add(faces[i].reflectivity);
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * sizeof(float), data.ToArray(), BufferUsageHint.StaticDraw);

            return mi;
        }

        public void Draw() {

            float[] positions = new float[geometry.Count * 16];
            int[] indirect = new int[geometry.Count * 4];

            for(int i = 0; i < geometry.Count; i++) {

                Matrix4 m = geometry[i].Transform;

                positions[i * 16 +  0] = roworder ? m.M11 : m.M11;
                positions[i * 16 +  1] = roworder ? m.M12 : m.M21;
                positions[i * 16 +  2] = roworder ? m.M13 : m.M31;
                positions[i * 16 +  3] = roworder ? m.M14 : m.M41;
                                            
                positions[i * 16 +  4] = roworder ? m.M21 : m.M12;
                positions[i * 16 +  5] = roworder ? m.M22 : m.M22;
                positions[i * 16 +  6] = roworder ? m.M23 : m.M32;
                positions[i * 16 +  7] = roworder ? m.M24 : m.M42;
                                            
                positions[i * 16 +  8] = roworder ? m.M31 : m.M13;
                positions[i * 16 +  9] = roworder ? m.M32 : m.M23;
                positions[i * 16 + 10] = roworder ? m.M33 : m.M33;
                positions[i * 16 + 11] = roworder ? m.M34 : m.M43;
                                            
                positions[i * 16 + 12] = roworder ? m.M41 : m.M14;
                positions[i * 16 + 13] = roworder ? m.M42 : m.M24;
                positions[i * 16 + 14] = roworder ? m.M43 : m.M34;
                positions[i * 16 + 15] = roworder ? m.M44 : m.M44;

                indirect[i * 4 + 0] = geometry[i].ModelInformation.Count;
                indirect[i * 4 + 1] = 1;
                indirect[i * 4 + 2] = geometry[i].ModelInformation.First;
                indirect[i * 4 + 3] = i;

            }
            
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StreamDraw);
            GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, indirect, indirect.Length / 4, 16);
        }

        public override void Release() {
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(ibo);
        }

       public void AddGeometry(Geometry g) {
            geometry.Add(g);
        }

    }

    public class ModelInformation {
        private int first;
        private int count;
        public ModelInformation(int first, int count) {
            this.first = first;
            this.count = count;
        }
        public int First {
            get {
                return first;
            }
        }
        public int Count {
            get {
                return count;
            }
        }
    }

    public class Geometry {
        ModelInformation mi;
        Matrix4 transform;
        public Geometry(ModelInformation mi, Matrix4 transform) {
            this.mi = mi;
            this.transform = transform;
        }
        public Geometry(ModelInformation mi) : this(mi, Matrix4.Identity) {

        }
        public Matrix4 Transform {
            get {
                return transform;
            }
            set {
                this.transform = value;
            }
        }
        public ModelInformation ModelInformation {
            get {
                return mi;
            }
        }
    }

    public struct Face {
        public Vector3 v0, v1, v2;
        public float c0, c1, c2;
        public float specularity;
        public float reflectivity;

        public Vector3[] v
        {
            get
            {
                return new Vector3[] { v0, v1, v2 };
            }
        }
    }

    public static class Faces {
        public static Face[] FromFile(string path) {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            Face[] f = new Face[r.ReadInt32()];
            for (int i = 0; i < f.Length; i++) {
                f[i] = new Face() {
                    v0 = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()),
                    v1 = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()),
                    v2 = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()),

                    c0 = r.ReadSingle(),
                    c1 = r.ReadSingle(),
                    c2 = r.ReadSingle(),

                    specularity = r.ReadSingle(),
                    reflectivity = r.ReadSingle()
                };
            }

            r.Close();
            fs.Close();

            return f;
        }
    }
}
