using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace IslandGame.Engine.OpenGL {

    abstract class Texture : Resource{

        private int id;
        private TextureTarget type;

        public Texture(TextureTarget type) {
            id = GL.GenTexture();
            this.type = type;
            GL.BindTexture(type, id);
        }

        public int ID {
            get {
                return id;
            }
        }

        public void Bind(TextureUnit t) {
            GL.ActiveTexture(t);
            GL.BindTexture(type, id);
        }

        public void SetWarpMode(TextureWrapMode mode) {
            GL.BindTexture(type, id);
            GL.TexParameter(type, TextureParameterName.TextureWrapS, (int)mode);
            GL.TexParameter(type, TextureParameterName.TextureWrapT, (int)mode);
            GL.TexParameter(type, TextureParameterName.TextureWrapR, (int)mode);
        }

        public void SetLODBias(float v) {
            GL.BindTexture(type, id);
            GL.TexParameter(type, TextureParameterName.TextureLodBias, v);
        }

        public void SetFiltering(TextureMinFilter min, TextureMagFilter mag) {
            GL.BindTexture(type, id);
            GL.TexParameter(type, TextureParameterName.TextureMinFilter, (int)min);
            GL.TexParameter(type, TextureParameterName.TextureMagFilter, (int)mag);
        }

        public override void Release() {
            GL.DeleteTexture(id);
        }

    }


    class Texture2D : Texture {

        private PixelInternalFormat internalformat;
        private OpenTK.Graphics.OpenGL4.PixelFormat format;
        private PixelType pixeltype;

        public Texture2D(int width, int height, PixelInternalFormat internalformat, OpenTK.Graphics.OpenGL4.PixelFormat format, PixelType type, IntPtr data) : base(TextureTarget.Texture2D){
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalformat, width, height, 0, format, PixelType.UnsignedByte, data);
            this.format = format;
            this.internalformat = internalformat;
            this.pixeltype = type;
        }

        public Texture2D(int width, int height, IntPtr data) : this(width, height, PixelInternalFormat.Rgba, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data) {

        }

        public Texture2D(int width, int height, PixelInternalFormat internalformat, OpenTK.Graphics.OpenGL4.PixelFormat format, PixelType type) : this(width, height, internalformat, format, type, IntPtr.Zero) {
            this.SetFiltering(TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            this.SetWarpMode(TextureWrapMode.ClampToEdge);
        }

        public Texture2D(int width, int height, PixelInternalFormat internalformat, OpenTK.Graphics.OpenGL4.PixelFormat format) : this(width, height, internalformat, format, PixelType.UnsignedByte, IntPtr.Zero) {
        }

        public void Resize(int width, int height) {
            GL.BindTexture(TextureTarget.Texture2D, this.ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalformat, width, height, 0, format, PixelType.UnsignedByte, IntPtr.Zero);
        }

        public static Texture2D FromFile(Bitmap bitmap) {
            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Texture2D t = new Texture2D(bmpdata.Width, bmpdata.Height, bmpdata.Scan0);
            bitmap.UnlockBits(bmpdata);
            t.SetFiltering(TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            return t;
        }

    }

}
