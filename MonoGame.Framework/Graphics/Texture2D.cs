#region License
/*
Microsoft Public License (Ms-PL)
MonoGame - Copyright © 2009 The MonoGame Team

All rights reserved.

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
purpose and non-infringement.
*/
#endregion License

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

using OpenTK.Graphics.ES11;

using Microsoft.Xna.Framework.Content;


namespace Microsoft.Xna.Framework.Graphics
{
    public class Texture2D : Texture
    {
		private ESImage texture;
		
		protected int textureId = -1;
		protected int _width;
		protected int _height;
		private bool _mipmap;
		private Rectangle _bounds = new Rectangle(0, 0, 0, 0);
		
		internal bool IsSpriteFontTexture {get;set;}
		
		// my change
		// --------
		public uint ID
		{
			get
			{ 
				if (texture == null)
					return (uint)textureId;
				else
					return texture.Name;
				
			}
		}
		// --------
		internal ESImage Image
		{
			get 
			{
				return texture;
			}
		}
		
        public Rectangle Bounds
        {
            get
            {
				_bounds.Width = _width;
				_bounds.Height = _height;
                return _bounds;
            }
        }
		
		internal Texture2D(ESImage theImage)
		{
			texture = theImage;
			_width = texture.ImageWidth;
			_height = texture.ImageHeight;
			_format = texture.Format;
		}
		
		public Texture2D(GraphicsDevice graphicsDevice, int width, int height): 
			this (graphicsDevice, width, height, false, SurfaceFormat.Color)
		{
			
		}
		
		public Texture2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat format)
		{
			this.graphicsDevice = graphicsDevice;
			this._width = width;
			this._height = height;
			this._format = format;
			this._mipmap = mipMap;
			
			generateOpenGLTexture();
		}
		
		private void generateOpenGLTexture() 
		{
			// modeled after this
			// http://steinsoft.net/index.php?site=Programming/Code%20Snippets/OpenGL/no9
			
			GL.GenTextures(1,ref textureId);
			GL.BindTexture(All.Texture2D, textureId);
			
			if (_mipmap)
			{
				// Taken from http://www.flexicoder.com/blog/index.php/2009/11/iphone-mipmaps/
				GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)All.LinearMipmapNearest);
				GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)All.Linear);
				GL.TexParameter(All.Texture2D, All.GenerateMipmap, (int)All.True);
			}
			else
			{
				GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)All.Linear);
				GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)All.Linear);
			}
			
			GL.BindTexture(All.Texture2D, 0);
			
		}

        public Color GetPixel(int x, int y)
        {
			var result = new Color(0, 0, 0, 0);		
			
			return result;
        }

        public void SetPixel(int x, int y, byte red, byte green, byte blue, byte alpha)
        {
            throw new NotImplementedException();
        }

        public void SetData<T>(T[] data)
        {
			throw new NotImplementedException();
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public static Texture2D FromFile(GraphicsDevice graphicsDevice, Stream textureStream)
        {
            MonoTouch.Foundation.NSData nsData = MonoTouch.Foundation.NSData.FromStream(textureStream);

			UIImage image = UIImage.LoadFromData(nsData);
			
			if (image == null)			
			{
				throw new ContentLoadException("Error loading Texture2D Stream");
			}
			
			ESImage theTexture = new ESImage(image, graphicsDevice.PreferedFilter);			
			Texture2D result = new Texture2D(theTexture);
			
			return result;
        }

        public static Texture2D FromFile(GraphicsDevice graphicsDevice, Stream textureStream, int numberBytes)
        {
            throw new NotImplementedException();
        }

        public static Texture2D FromFile(GraphicsDevice graphicsDevice, string filename, int width, int height)
        {
			UIImage image = UIImage.FromBundle(filename);
			if (image == null)
			{
				throw new ContentLoadException("Error loading file: " + filename);
			}			
			
			ESImage theTexture;
			
			if ( width == 0 && height == 0 )
			{
				theTexture = new ESImage(image, graphicsDevice.PreferedFilter);
			}
			else
			{
				var small = image.Scale (new SizeF (width, height));
				theTexture = new ESImage(small, graphicsDevice.PreferedFilter);
			}
			Texture2D result = new Texture2D(theTexture);
			// result.Name = Path.GetFileNameWithoutExtension(filename);
			result.Name = filename;
			return result;					
        }

        public static Texture2D FromFile(GraphicsDevice graphicsDevice, string filename)
		{
			return FromFile( graphicsDevice, filename, 0, 0 );
        }
		
        public void SetData<T>(T[] data, int startIndex, int elementCount, SetDataOptions options)
        {
            throw new NotImplementedException();
        }

        public void SetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount, SetDataOptions options)
        {
            throw new NotImplementedException();
        }

        public void GetData<T>(T[] data)
        {	
			// TODO Causese AV on Device, but not simulator GetData<T>(0, null, data, 0, Width * Height);
			
			if (data == null )
			{
				throw new ArgumentException("data cannot be null");
			}			
        }

        public void GetData<T>(T[] data, int startIndex, int elementCount)
        {
            GetData<T>(0, null, data, startIndex, elementCount);
        }

        public void GetData<T>(int level, Rectangle? rect, T[] data, int startIndex, int elementCount)
        {
            if (data == null )
			{
				throw new ArgumentException("data cannot be null");
			}
			
			if (data.Length < startIndex + elementCount)
			{
				throw new ArgumentException("The data passed has a length of " + data.Length + " but " + elementCount + " pixels have been requested.");
			}
			
			Rectangle r;
			if (rect != null)
			{
				r = rect.Value;
			}
			else
			{
				r = new Rectangle(0, 0, Width, Height);
			}
			
			int sz = 0;
						
			byte[] pixel = new byte[4];
			int pos;
			IntPtr pixelOffset;
			// Get the Color values
			if ((typeof(T) == typeof(Color))) 
			{	
				// Load up texture into memory
				UIImage uiImage = UIImage.FromBundle(this.Name);
				if (uiImage == null)
				{
					throw new ContentLoadException("Error loading file via UIImage: " + Name);
				}
				
				CGImage image = uiImage.CGImage;
				if (image == null)
				{
					throw new ContentLoadException("Error with CGIamge: " + Name);
				}
				
				int	width,height,i;
		        CGContext context = null;
		        IntPtr imageData;
		        CGColorSpace colorSpace;
		        IntPtr tempData;
		        bool hasAlpha;
		        CGImageAlphaInfo info;
		        CGAffineTransform transform;
		        Size imageSize;
		        SurfaceFormat pixelFormat;
		        bool sizeToFit = false;
				
				info = image.AlphaInfo;
				hasAlpha = ((info == CGImageAlphaInfo.PremultipliedLast) || (info == CGImageAlphaInfo.PremultipliedFirst) || (info == CGImageAlphaInfo.Last) || (info == CGImageAlphaInfo.First) ? true : false);
				
				if (image.ColorSpace != null)
				{
					pixelFormat = SurfaceFormat.Color;
				}
				else 
				{	
					pixelFormat = SurfaceFormat.Alpha8;
				}
		
				imageSize = new Size(image.Width,image.Height);
				transform = CGAffineTransform.MakeIdentity();
				width = imageSize.Width;
		
				if((width != 1) && ((width & (width - 1))!=0)) {
					i = 1;
					while((sizeToFit ? 2 * i : i) < width)
						i *= 2;
					width = i;
				}
				height = imageSize.Height;
				if((height != 1) && ((height & (height - 1))!=0)) {
					i = 1;
					while((sizeToFit ? 2 * i : i) < height)
						i *= 2;
					height = i;
				}
				// TODO: kMaxTextureSize = 1024
				while((width > 1024) || (height > 1024)) 
				{
					width /= 2;
					height /= 2;
					transform = CGAffineTransform.MakeScale(0.5f,0.5f);
					imageSize.Width /= 2;
					imageSize.Height /= 2;
				}
				
				switch(pixelFormat) 
				{		
					case SurfaceFormat.Color:
						colorSpace = CGColorSpace.CreateDeviceRGB();
						imageData = Marshal.AllocHGlobal(height * width * 4);
						context = new CGBitmapContext(imageData, width, height, 8, 4 * width, colorSpace,CGImageAlphaInfo.PremultipliedLast);
						colorSpace.Dispose();
						break;
					case SurfaceFormat.Alpha8:
						imageData = Marshal.AllocHGlobal(height * width);
						context = new CGBitmapContext(imageData, width, height, 8, width, null, CGImageAlphaInfo.Only);
						break;				
					default:
						throw new NotSupportedException("Invalid pixel format"); 
				}
					
				context.ClearRect(new RectangleF(0,0,width,height));
	 			context.TranslateCTM(0, height - imageSize.Height);
				
				if (!transform.IsIdentity)
				{
					context.ConcatCTM(transform);
				}
				
				context.DrawImage(new RectangleF(0, 0, image.Width, image.Height), image);
				
				//Convert "RRRRRRRRRGGGGGGGGBBBBBBBBAAAAAAAA" to "RRRRRGGGGGGBBBBB"
				/*
				if(pixelFormat == SurfaceFormat.Rgb32) {
					tempData = Marshal.AllocHGlobal(height * width * 2);
					
					int d32;
					short d16;
					int inPixel32Count=0,outPixel16Count=0;
					for(i = 0; i < width * height; ++i, inPixel32Count+=sizeof(int))
					{
						d32 = Marshal.ReadInt32(imageData,inPixel32Count);
						short R = (short)((((d32 >> 0) & 0xFF) >> 3) << 11);
						short G = (short)((((d32 >> 8) & 0xFF) >> 2) << 5);
						short B = (short)((((d32 >> 16) & 0xFF) >> 3) << 0);
						d16 = (short)  (R | G | B);
						Marshal.WriteInt16(tempData,outPixel16Count,d16);
						outPixel16Count += sizeof(short);
					}
					Marshal.FreeHGlobal(imageData);
					imageData = tempData;			
				}									
				*/
				
				int count = 0;
				
				// Loop through and extract the data
				for(int y = r.Top; y < r.Bottom; y++ )
				{
					for( int x = r.Left; x < r.Right; x++ )
					{
						var result = new Color(0, 0, 0, 0);						
						
						switch(this.Format) 
						{
							case SurfaceFormat.Color /*kTexture2DPixelFormat_RGBA8888*/:
							case SurfaceFormat.Dxt3 :
							    sz = 4;
								pos = ( (y * imageSize.Width) + x ) * sz;								
								pixelOffset = new IntPtr(imageData.ToInt64() + pos);							
								Marshal.Copy(pixelOffset, pixel, 0, 4);	
								result.R = pixel[0];
								result.G = pixel[1];
								result.B = pixel[2];
								result.A = pixel[3];
								break;
							case SurfaceFormat.Bgra4444 /*kTexture2DPixelFormat_RGBA4444*/:
								sz = 2;
								pos = ( (y * imageSize.Width) + x ) * sz;
								pixelOffset = new IntPtr(imageData.ToInt64() + pos);

								Marshal.Copy(pixelOffset, pixel, 0, 4);	
							
								result.R = pixel[0];
								result.G = pixel[1];
								result.B = pixel[2];
								result.A = pixel[3];
								break;
							case SurfaceFormat.Bgra5551 /*kTexture2DPixelFormat_RGB5A1*/:
								sz = 2;
								pos = ( (y * imageSize.Width) + x ) * sz;
								pixelOffset = new IntPtr(imageData.ToInt64() + pos);
								Marshal.Copy(pixelOffset, pixel, 0, 4);	
							
								result.R = pixel[0];
								result.G = pixel[1];
								result.B = pixel[2];
								result.A = pixel[3];
								break;
							case SurfaceFormat.Alpha8 /*kTexture2DPixelFormat_A8*/:
								sz = 1;
								pos = ( (y * imageSize.Width) + x ) * sz;
								pixelOffset = new IntPtr(imageData.ToInt64() + pos);								
								Marshal.Copy(pixelOffset, pixel, 0, 4);	
							
								result.A = pixel[0];
								break;
							default:
								throw new NotSupportedException("Texture format");
						}
						data[((y * imageSize.Width) + x)] = (T)(object)result;
						
						count++;
						if (count >= elementCount) 
							return;
					}
				}
								
				context.Dispose();
				Marshal.FreeHGlobal(imageData);	
			}	
			else
			{
				throw new NotImplementedException();
			}
        }
	}
}

