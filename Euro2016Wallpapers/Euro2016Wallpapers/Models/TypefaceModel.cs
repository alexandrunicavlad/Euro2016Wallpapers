using System;
using Android.Graphics;

namespace Euro2016Wallpapers
{
	public class TypefaceModel
	{
		public TypefaceModel (Typeface typeface, String typefacename)
		{
			this.TypefaceName = typefacename;
			this.Typeface = typeface;
		}

		public String TypefaceName { get; set; }

		public Typeface Typeface { get; set; }
	}
}

