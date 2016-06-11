using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace IslandGame.Engine.OpenGL {

    class Framebuffer : Resource{

        public int id;

        public Framebuffer() {
            id = GL.GenFramebuffer();
            Bind();
        }

        public int ID {
            get {
                return id;
            }
        }

        public bool IsOK() {
            return GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferComplete;
        }

        public Framebuffer AttachTexture(Texture2D texture, FramebufferAttachment mode) {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, mode, TextureTarget.Texture2D, texture.ID, 0);
            return this;
        }

        public Framebuffer AttachRenderBuffer(Renderbuffer renderBuffer, FramebufferAttachment mode) {
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, mode, RenderbufferTarget.Renderbuffer, renderBuffer.ID);
            return this;
        }

        public void Bind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
        }

        public void Unbind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public override void Release() {
            GL.DeleteFramebuffer(id);
        }

        public bool CheckStatus() {
            switch (GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt)) {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Trace.TraceInformation("FBO: The framebuffer is complete and valid for rendering.");
                        return true;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Trace.TraceError("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Trace.TraceError("FBO: There are no attachments.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Trace.TraceError("FBO: Attachments are of different size. All attachments must have the same width and height.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Trace.TraceError("FBO: The color attachments have different format. All color attachments must have the same format.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Trace.TraceError("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Trace.TraceError("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Trace.TraceError("FBO: This particular FBO configuration is not supported by the implementation.");
                        break;
                    }
                default:
                    {
                        Trace.TraceError("FBO: Status unknown. (yes, this is really bad.)");
                        break;
                    }
            }
            return false;
        }
    }

    class Renderbuffer : Resource{

        private int id;
        private RenderbufferStorage mode;

        public Renderbuffer(int width, int height, RenderbufferStorage mode) {
            id = GL.GenRenderbuffer();
            this.mode = mode;
            Resize(width, height);
        }

        public void Resize(int width, int height) {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, id);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, mode, width, height);
        }

        public int ID {
            get{
                return id;
            }
        }

        public override void Release() {
            GL.DeleteRenderbuffer(id);
        }
    }

}
