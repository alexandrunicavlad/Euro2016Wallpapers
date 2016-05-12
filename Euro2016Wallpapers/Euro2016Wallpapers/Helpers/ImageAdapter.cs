using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Graphics.Drawables;
using Java.Net;
using System.Collections.Generic;
using Java.IO;
using Android.Graphics;
using System.Net;
using Android.Support.V7.Widget;
using Square.Picasso;
using Android.Content.Res;

namespace Euro2016Wallpapers
{
	public class ImageAdapter : RecyclerView.Adapter
	{
		Context context;
		List<ImageModel> images;

		public event EventHandler<int> ItemClick;

		public ImageAdapter (Context c, List<ImageModel> imgs)
		{
			context = c;
			images = imgs;
		}

		public ImageAdapter ()
		{			
		}

		public override int ItemCount {
			get {
				return images.Count;
			}
		}

		public override void 
		OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			PhotoViewHolder vh = holder as PhotoViewHolder;
			var item = images [position];
			var height = 170 * context.Resources.DisplayMetrics.Density;

			if (item.type.Equals ("local")) {				
				Picasso.With (context).Load (item.version).Resize (context.Resources.DisplayMetrics.WidthPixels / 3, (int)height).CenterCrop ().Into (vh.Image, delegate {							
					var b =	System.GC.GetTotalMemory (true);
				}, delegate {
					var a =	System.GC.GetTotalMemory (true);
					Picasso.With (context).Load (item.version).Resize (context.Resources.DisplayMetrics.WidthPixels / 3, (int)height).CenterCrop ().Into (vh.Image);
				});		
			} else {	
				var positionChar = item.url.IndexOf ("upload");
				var subUrl = item.url.Substring (0, positionChar + 7);
				var afterUrl = item.url.Substring (positionChar + 7);
				var heught = 170 * context.Resources.DisplayMetrics.Density;
				var newUrl = string.Format ("{0}w_{1},h_{2},c_fill/{3}", subUrl, context.Resources.DisplayMetrics.WidthPixels / 3, heught, afterUrl);
				Picasso.With (context).Load (newUrl)	
					.Into (vh.Image, delegate {							
					var b =	System.GC.GetTotalMemory (true);
				}, delegate {
					var a =	System.GC.GetTotalMemory (true);
				});	
			}	

		}

		public override RecyclerView.ViewHolder OnCreateViewHolder (Android.Views.ViewGroup parent, int viewType)
		{	
			View itemView = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.picture_layout, parent, false);
			PhotoViewHolder vh = new PhotoViewHolder (itemView, OnClick); 
			return vh;
		}

		void OnClick (int position)
		{
			if (ItemClick != null)
				ItemClick (this, position);
		}

	}

	public class CropSquareTransformation : Java.Lang.Object, ITransformation
	{
		public Bitmap Transform (Bitmap source)
		{
			int size = Math.Min (source.Width, source.Height);
			int x = (source.Width - size) / 2;
			int y = (source.Height - size) / 2;
			Bitmap result = Bitmap.CreateBitmap (source, x, y, size, size);
			if (result != source) {
				source.Recycle ();
			}
			return result;
		}

		public string Key {
			get { return "square()"; } 
		}
	}

	public class PhotoViewHolder : RecyclerView.ViewHolder
	{
		public ImageView Image { get; private set; }

		public int Position { get; set; }

		public PhotoViewHolder (View itemView, Action<int> listener) : base (itemView)
		{			
			Image = itemView.FindViewById<ImageView> (Resource.Id.imageView);

			itemView.Click += (sender, e) => listener (base.Position);
		}


	}
}

