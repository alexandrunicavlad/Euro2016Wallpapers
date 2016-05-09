using System;
using Android.Graphics;
using Android.Util;
using Android.Runtime;

namespace Euro2016Wallpapers
{
	public class MemoryLimitedLruCache : LruCache
	{
		//	public Func<string, Bitmap> GetSizeOf;

		public MemoryLimitedLruCache (int size) : base (size)
		{
		}

		protected override int SizeOf (Java.Lang.Object key, Java.Lang.Object value)
		{
			// android.graphics.Bitmap.getByteCount() method isn't currently implemented in Xamarin. Invoke Java method.
			IntPtr classRef = JNIEnv.FindClass ("android/graphics/Bitmap");
			var getBytesMethodHandle = JNIEnv.GetMethodID (classRef, "getByteCount", "()I");
			var byteCount = JNIEnv.CallIntMethod (value.Handle, getBytesMethodHandle);

			return byteCount / 1024;

		}

	}
}

