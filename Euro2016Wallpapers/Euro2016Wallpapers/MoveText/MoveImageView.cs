using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Graphics;
using Android.Views;
using MoveText;
using System.Collections.Generic;
using Euro2016Wallpapers;
using Android.Support.V4.View;
using Android.Text;


namespace MoveText
{

	public class MoveImageView : ImageView
	{

		private Context context;
		private float x = 0;
		private float y;
		private Boolean putText = false;
		private int textColor = Resource.Color.white;
		private string textString = "testingggg";
		private float textSize = 20f;
		private Typeface typefaces = Typeface.Create (Typeface.Default, TypefaceStyle.Normal);

		public MoveImageView (Context context, IAttributeSet attrs)
			: base (context, attrs)
		{
			SetLayerType (Android.Views.LayerType.Software, null);
			this.context = context;
		}

		public void EnterText (int height)
		{
			putText = true;
			y = height / 2;
		}

		public void PutText (string text)
		{			
			textString = text;
		}

		public void TypefaceText (Typeface typeface)
		{
			typefaces = typeface;
		}

		protected override void OnDraw (Canvas canvas)
		{
			{
				base.OnDraw (canvas);
				if (putText) {
					CreateBitmapWithText (canvas);
				}
			}
		}

		public void SizeText (float size)
		{			
			textSize = size;
		}

		public void TextColor (int color)
		{
			textColor = color;
		}

		public void CreateBitmapWithText (Canvas canvas)
		{
			string str = textString;
			var activit = (PictureActivity)context;
			TextPaint mTextPaint = new TextPaint ();
			mTextPaint.TextSize = textSize * activit.Resources.DisplayMetrics.Density + 1;		
			mTextPaint.TextAlign = Paint.Align.Center;
			mTextPaint.Color = Resources.GetColor (textColor);
			mTextPaint.SetTypeface (typefaces);
			StaticLayout mTextLayout = new StaticLayout (str, mTextPaint, canvas.Width - 20, Android.Text.Layout.Alignment.AlignNormal, 1.0f, 0.0f, false);
//			Bitmap b = Bitmap.CreateBitmap (canvas.Width, mTextLayout.Height, Bitmap.Config.Alpha8);
//			Canvas c = new Canvas (b);
//			Paint paint = new Paint ();
//			paint.Color = Resources.GetColor (Resource.Color.black_transparent);
//			c.DrawRect (0, 0, canvas.Width, mTextLayout.Height, paint);
//			//canvas.DrawBitmap (b, 0, y, paint);
//			b.Recycle ();
//			b = null;
			canvas.Save ();
			canvas.Translate (canvas.Width / 2, y);
			mTextLayout.Draw (canvas);
			canvas.Restore ();

		}

		public override bool OnTouchEvent (MotionEvent ev)
		{
			var cropImage = (PictureActivity)context;

			switch (ev.Action) {
			case MotionEventActions.Down:
				x = ev.GetX ();
				y = ev.GetY ();
				Invalidate ();
				break;
			case MotionEventActions.Move:
				x = ev.GetX ();
				y = ev.GetY ();
				Invalidate ();
				break;
			}
			return true;	

		}
	}
}

