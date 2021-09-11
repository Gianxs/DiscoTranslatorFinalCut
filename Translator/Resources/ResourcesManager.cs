using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using DiscoTranslatorFinalCut.Helpers.UI;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator
{
	public class ResourcesManager
	{

		public static Assembly ResourceAssembly
		{
			get
			{
				return Assembly.GetAssembly(typeof(ResourcesManager));
			}
		}

		public static string BaseDir
		{
			get
			{

				return Path.GetDirectoryName(ResourceAssembly.Location) + Path.PathSeparator;

			}
		}


		public static byte[] loadResourceData(string name)
		{
			name = "DiscoTranslatorFinalCut.Translator.Resources." + name;

			UnmanagedMemoryStream stream = (UnmanagedMemoryStream)ResourceAssembly.GetManifestResourceStream(name);
			if (stream == null)
			{
				return null;
			}

			BinaryReader read = new BinaryReader(stream);
			return read.ReadBytes((int)stream.Length);
		}

		public static string loadResourceString(string name)
		{
			name = "DiscoTranslatorFinalCut.Translator.Resources." + name;

			UnmanagedMemoryStream stream = (UnmanagedMemoryStream)ResourceAssembly.GetManifestResourceStream(name);
			if (stream == null)
			{
				return null;
			}

			StreamReader read = new StreamReader(stream);
			return read.ReadToEnd();
		}

		public static Texture2D loadTexture(int x, int y, string filename)
		{
			try
			{
				Texture2D texture = new Texture2D(x, y, TextureFormat.ARGB32, false);
				texture.LoadImage(loadResourceData(filename), false);
				return FixTransparency(texture);
			}
			catch (Exception e)
			{

				PL.log.LogError(PL.PREFIX + " Exception Error " + e.Message);

			}

			return null;
		}

		//=========================================================================
		// Copy the values of adjacent pixels to transparent pixels color info, to
		// remove the white border artifact when importing transparent .PNGs.
		//Thx to petrucio for this -> http://answers.unity3d.com/questions/238922/png-transparency-has-white-borderhalo.html
		internal static Texture2D FixTransparency(Texture2D texture)
		{
			Color32[] pixels = texture.GetPixels32();
			int w = texture.width;
			int h = texture.height;

			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					int idx = y * w + x;
					Color32 pixel = pixels[idx];
					if (pixel.a == 0)
					{
						bool done = false;
						if (!done && x > 0) done = TryAdjacent(ref pixel, pixels[idx - 1]);        // Left   pixel
						if (!done && x < w - 1) done = TryAdjacent(ref pixel, pixels[idx + 1]);        // Right  pixel
						if (!done && y > 0) done = TryAdjacent(ref pixel, pixels[idx - w]);        // Top    pixel
						if (!done && y < h - 1) done = TryAdjacent(ref pixel, pixels[idx + w]);        // Bottom pixel
						pixels[idx] = pixel;
					}
				}
			}

			texture.SetPixels32(pixels);
			texture.Apply();

			return texture;
		}

		private static bool TryAdjacent(ref Color32 pixel, Color32 adjacent)
		{
			if (adjacent.a == 0) return false;

			pixel.r = adjacent.r;
			pixel.g = adjacent.g;
			pixel.b = adjacent.b;
			return true;
		}
		//=========================================================================
	}
}