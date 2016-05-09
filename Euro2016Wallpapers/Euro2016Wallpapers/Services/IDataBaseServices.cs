using System;
using Android.Graphics;

namespace Euro2016Wallpapers.Services
{
	public interface IDatabaseServices
	{
		void InsertImage (string url, byte[] img);

		void CloseDatabase ();

		Bitmap GetImage (string url);

		bool CheckExist (string url);
	}
}

