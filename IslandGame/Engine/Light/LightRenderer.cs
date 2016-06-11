using IslandGame.Engine.OpenGL;
using IslandGame.Engine.Scene;
using OpenTK.Graphics.OpenGL4;

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

}
