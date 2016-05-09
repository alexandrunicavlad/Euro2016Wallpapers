using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Euro2016Wallpapers;
using Android.Content;


namespace MoveText
{
	public class HighlightView
	{
		// The View displaying the image.
		private View context;
		private string _text;
		private float _fontSize;
		private int _color;
		private Context contextActivity;

		public enum ModifyMode
		{
			None,
			Move,
			Grow
		}

		private ModifyMode mode = ModifyMode.None;

		private RectF imageRect;
		private RectF cropRect;
		public Matrix matrix;
		private bool maintainAspectRatio = false;
		private float initialAspectRatio;
		private Paint focusPaint = new Paint ();
		private Paint noFocusPaint = new Paint ();
		private Paint outlinePaint = new Paint ();

		public HighlightView (View ctx, string txt, float fontSize, int color)
		{
			context = ctx;
			_text = txt;
			_fontSize = fontSize;
			_color = color;

		}

		public bool Focused { get; set; }

		public bool Hidden { get; set; }

		public Rect DrawRect 
		{ get; private set; }

		public Rect CropRect {
			get {
				return new Rect ((int)cropRect.Left, (int)cropRect.Top,
					(int)cropRect.Right, (int)cropRect.Bottom);
			}
		}

		public ModifyMode Mode {
			get { return mode; }
			set {
				if (value != mode) {
					mode = value;
					context.Invalidate ();
				}
			}
		}

		public void Draw (Canvas canvas)
		{
			if (Hidden) {
				return;
			}

			canvas.Save ();
			var cropImage = Android.App.Application.Context;

			if (!Focused) {
				outlinePaint.Color = Color.White;
				canvas.DrawRect (DrawRect, outlinePaint);
			} else {
				var viewDrawingRect = new Rect ();
				context.GetDrawingRect (viewDrawingRect);
				Paint paint = new Paint ();
				Android.Graphics.Paint.FontMetrics fm = new Android.Graphics.Paint.FontMetrics ();
				paint.Color = cropImage.Resources.GetColor (Resource.Color.black_transparent);
				paint.TextSize = _fontSize;
				paint.GetFontMetrics (fm);
				var top1 = canvas.Height / 2 - 50;
				var bottom1 = canvas.Height / 2 + 50;
				var right1 = canvas.Width;
				var des = paint.Descent ();
				var asc = paint.Ascent ();
				int xPosition = canvas.Width / 2;
				var yPosition = (int)((canvas.Height / 2) - ((paint.Descent () + paint.Ascent ()) / 2));			
				canvas.DrawRect (0, top1, right1, bottom1, paint);				
				paint.Color = cropImage.Resources.GetColor (_color);
				paint.TextAlign = Paint.Align.Center;
				canvas.DrawText (_text, xPosition, yPosition, paint);

			}
		}

		public void Invalidate ()
		{
			DrawRect = computeLayout ();
		}

		public void Setup (Matrix m, Rect imageRect, RectF cropRect, bool maintainAspectRatio)
		{
			matrix = new Matrix (m);

			this.cropRect = cropRect;
			this.imageRect = new RectF (imageRect);
			this.maintainAspectRatio = maintainAspectRatio;

			initialAspectRatio = cropRect.Width () / cropRect.Height ();
			DrawRect = computeLayout ();

			focusPaint.SetARGB (125, 50, 50, 50);
			noFocusPaint.SetARGB (125, 50, 50, 50);
			outlinePaint.StrokeWidth = 3;
			outlinePaint.SetStyle (Paint.Style.Stroke);
			outlinePaint.AntiAlias = true;

			mode = ModifyMode.None;
			init ();
		}

		private void init ()
		{
			var resources = context.Resources;

//			resizeDrawableWidth = resources.GetDrawable (Resource.Drawable.camera_crop_width);
//			resizeDrawableHeight = resources.GetDrawable (Resource.Drawable.camera_crop_height);
		}

		// Maps the cropping rectangle from image space to screen space.
		private Rect computeLayout ()
		{
			var r = new RectF (cropRect.Left, cropRect.Top,
				        cropRect.Right, cropRect.Bottom);
			matrix.MapRect (r);
			return new Rect ((int)Math.Round (r.Left), (int)Math.Round (r.Top),
				(int)Math.Round (r.Right), (int)Math.Round (r.Bottom));
		}


	}
}