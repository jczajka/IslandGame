using OpenTK;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using IslandGame.Engine.Scene;
using System;

namespace IslandGame.Engine.OpenGL {

    class ModelBatch : Resource {

        private bool roworder = false;

        private int vao;
        private int vbo;
        private int ibo;
        private List<float> data;

        public ModelBatch() {
            
            data = new List<float>();
            
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

        public void Draw(GameObject root) {

            int number = root.GetCount<ModelRenderer>();
            if(number > 0) {

                float[] positions = new float[number * 16];
                int[] indirect = new int[number * 4];
                
                int index = 0;

                root.Foreach<ModelRenderer>((GameObject go, ModelRenderer mr) => {
                    Matrix4 m = mr.Transform;

                    positions[index * 16 +  0] = roworder ? m.M11 : m.M11;
                    positions[index * 16 +  1] = roworder ? m.M12 : m.M21;
                    positions[index * 16 +  2] = roworder ? m.M13 : m.M31;
                    positions[index * 16 +  3] = roworder ? m.M14 : m.M41;
                                              
                    positions[index * 16 +  4] = roworder ? m.M21 : m.M12;
                    positions[index * 16 +  5] = roworder ? m.M22 : m.M22;
                    positions[index * 16 +  6] = roworder ? m.M23 : m.M32;
                    positions[index * 16 +  7] = roworder ? m.M24 : m.M42;
                                             
                    positions[index * 16 +  8] = roworder ? m.M31 : m.M13;
                    positions[index * 16 +  9] = roworder ? m.M32 : m.M23;
                    positions[index * 16 + 10] = roworder ? m.M33 : m.M33;
                    positions[index * 16 + 11] = roworder ? m.M34 : m.M43;
                                               
                    positions[index * 16 + 12] = roworder ? m.M41 : m.M14;
                    positions[index * 16 + 13] = roworder ? m.M42 : m.M24;
                    positions[index * 16 + 14] = roworder ? m.M43 : m.M34;
                    positions[index * 16 + 15] = roworder ? m.M44 : m.M44;
                
                    indirect[index * 4 + 0] = mr.ModelInformation.Count;
                    indirect[index * 4 + 1] = 1;
                    indirect[index * 4 + 2] = mr.ModelInformation.First;
                    indirect[index * 4 + 3] = index;
                    index++;
                });

                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ibo);
                GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StreamDraw);
                GL.MultiDrawArraysIndirect(PrimitiveType.Triangles, indirect, indirect.Length / 4, 16);
            }
        }

        public override void Release() {
            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(ibo);
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

    public class ModelRenderer : GameComponent{

        ModelInformation mi;
        Matrix4 transform;

        public ModelRenderer(ModelInformation mi, Matrix4 transform) {
            this.mi = mi;
            this.transform = transform;
        }

        public ModelRenderer(ModelInformation mi) : this(mi, Matrix4.Identity) {

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
