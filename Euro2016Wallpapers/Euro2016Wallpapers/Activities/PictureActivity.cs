
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.IO;
using Android.Provider;
using MoveText;
using Android.Graphics.Drawables;
using Android.Media;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using Android.Views.Animations;
using Android.Animation;



namespace Euro2016Wallpapers
{
	[Activity (Label = "PictureActivity", Theme = "@style/NoActionBar", ConfigurationChanges =
		(Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]			
	public class PictureActivity : Activity
	{

		private Bitmap bitmap;
		private MoveImageView imageView;
		private string imagePath;
		private Android.Net.Uri saveUri = null;
		private int textSize = 20;
		private TextModel textList;
		private ListView listView;
		private RelativeLayout loading;
		private bool retry = false;
		private RelativeLayout mainLayout;
		private RelativeLayout mainLoading;
		private Bundle extras;
		private byte[] byteArray;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.picture_layout_main);
			imageView = FindViewById<MoveImageView> (Resource.Id.picturefromcamera);
			mainLayout = FindViewById<RelativeLayout> (Resource.Id.picturelayout);
			mainLoading = FindViewById<RelativeLayout> (Resource.Id.main_loading);
			mainLayout.Visibility = ViewStates.Gone;
			mainLoading.Visibility = ViewStates.Visible;
			extras = Intent.Extras;
			var imageNumber = extras.GetString ("image-number");
			if (extras != null) {
				if (imageNumber != null) {
					if (!extras.GetString ("image-number").Equals ("")) {	
						var url = extras.GetString ("image-number");
						ThreadPool.QueueUserWorkItem (o => GetImageBitmapFromUrl (url));
					}
				} else {
					imagePath = extras.GetString ("image-path");
					saveUri = GetImageUri (imagePath);
					if (extras.GetString (MediaStore.ExtraOutput) != null) {						
						saveUri = GetImageUri (extras.GetString (MediaStore.ExtraOutput));

					}
					ThreadPool.QueueUserWorkItem (o => GetBitmap (imagePath));
				}

			}

			var cancelIcon = FindViewById<ImageView> (Resource.Id.cancelIcon);
			var saveIcon = FindViewById<ImageView> (Resource.Id.saveIcon);
			var editIcon = FindViewById<ImageView> (Resource.Id.editIcon);
			var settingIcon = FindViewById<ImageView> (Resource.Id.settingsIcon);
			var palletteIcon = FindViewById<ImageView> (Resource.Id.palletteIcon);
			var textIcon = FindViewById<ImageView> (Resource.Id.textIcon);
			var fontIcon = FindViewById<ImageView> (Resource.Id.fontIcon);

			cancelIcon.Click += delegate {
				OnBackPressed ();
			};
			saveIcon.Click += delegate {
				saveIcon.Clickable = false;
				imageView.DrawingCacheEnabled = true;
				Bitmap newbitmapnew = imageView.DrawingCache;
				mainLoading.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => SaveImage (newbitmapnew));	
			};
			editIcon.Click += delegate {		
				SelectText ();
				//imageView.EnterText (WindowManager.DefaultDisplay.Height);
				//imageView.Invalidate ();
			};
			settingIcon.Click += delegate {
				var density = Resources.DisplayMetrics.Density / 2;
				if (palletteIcon.Visibility == ViewStates.Visible) {					
					ObjectAnimator moverPallette = ObjectAnimator.OfFloat (palletteIcon, "translationX", -130 * density, 0);
					ObjectAnimator moverTextIcon = ObjectAnimator.OfFloat (textIcon, "translationX", -120 * density, 0);
					ObjectAnimator moverTextIconY = ObjectAnimator.OfFloat (textIcon, "translationY", 110 * density, 0);
					ObjectAnimator moverFontIcon = ObjectAnimator.OfFloat (fontIcon, "translationY", 130 * density, 0);
					moverPallette.Start ();
					moverTextIcon.Start ();
					moverTextIconY.Start ();
					moverFontIcon.Start ();
					moverPallette.AnimationEnd += (object sender, EventArgs e) => {
						settingIcon.SetImageResource (Resource.Drawable.ic_settings);
						palletteIcon.Visibility = ViewStates.Gone;
						textIcon.Visibility = ViewStates.Gone;
						fontIcon.Visibility = ViewStates.Gone;
					};
				} else {
					settingIcon.SetImageResource (Resource.Drawable.ic_close);
					palletteIcon.Visibility = ViewStates.Visible;
					textIcon.Visibility = ViewStates.Visible;
					fontIcon.Visibility = ViewStates.Visible;
					settingIcon.Background = Resources.GetDrawable (Resource.Drawable.round_green_main);
					ObjectAnimator moverPallette = ObjectAnimator.OfFloat (palletteIcon, "translationX", 0, -130 * density);
					ObjectAnimator moverTextIcon = ObjectAnimator.OfFloat (textIcon, "translationX", 0, -120 * density);
					ObjectAnimator moverTextIconY = ObjectAnimator.OfFloat (textIcon, "translationY", 0, 110 * density);
					ObjectAnimator moverFontIcon = ObjectAnimator.OfFloat (fontIcon, "translationY", 0, 130 * density);
					moverPallette.Start ();
					moverTextIcon.Start ();
					moverTextIconY.Start ();
					moverFontIcon.Start ();
				}

			};
			palletteIcon.Click += delegate {

				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetTitle (GetString (Resource.String.ChoseColor));

				var infate = LayoutInflater.Inflate (Resource.Layout.color_text_layout, null);
				var seekBar = infate.FindViewById<SeekBar> (Resource.Id.edit_seekBar);
				var indicator = infate.FindViewById<TextView> (Resource.Id.text_seekBar_indicator);
				alert.SetView (infate);
				alert.SetNegativeButton (GetString (Resource.String.cancelbutton), delegate {

				});
				AlertDialog alertDialog = alert.Show ();
				var colorSelected = Resource.Color.white;
				infate.FindViewById<ImageView> (Resource.Id.purpleColor).Click += delegate {
					colorSelected = Resource.Color.purple_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.blueColor).Click += delegate {
					colorSelected = Resource.Color.blue_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.blueLightColor).Click += delegate {
					colorSelected = Resource.Color.blue_light_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.greenColor).Click += delegate {
					colorSelected = Resource.Color.green_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.pinkColor).Click += delegate {
					colorSelected = Resource.Color.pink_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.redColor).Click += delegate {
					colorSelected = Resource.Color.red_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();
				};
				infate.FindViewById<ImageView> (Resource.Id.orangeColor).Click += delegate {
					colorSelected = Resource.Color.orange_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.yellowColor).Click += delegate {
					colorSelected = Resource.Color.yellow_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.blackColor).Click += delegate {
					colorSelected = Resource.Color.black_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
				infate.FindViewById<ImageView> (Resource.Id.whiteColor).Click += delegate {
					colorSelected = Resource.Color.white_text;
					imageView.TextColor (colorSelected);
					imageView.Invalidate ();
					alertDialog.Dismiss ();

				};
			};
			textIcon.Click += delegate {
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetTitle (GetString (Resource.String.Selectfontsize));

				var infate = LayoutInflater.Inflate (Resource.Layout.slider_size_layout, null);
				var seekBar = infate.FindViewById<SeekBar> (Resource.Id.edit_seekBar);
				var indicator = infate.FindViewById<TextView> (Resource.Id.text_seekBar_indicator);
				alert.SetView (infate);
				alert.SetPositiveButton (GetString (Resource.String.donebutton), delegate {		
					imageView.SizeText ((float)seekBar.Progress);
					textSize = seekBar.Progress;
					imageView.Invalidate ();
				});
				alert.SetNegativeButton (GetString (Resource.String.cancelbutton), delegate {
					
				});

				var isLoaded = false;
				seekBar.Progress = textSize;
				seekBar.LayoutChange += delegate(object sender, View.LayoutChangeEventArgs e) {
					var surfaceOrientation = Resources.Configuration.Orientation;
					if (isLoaded)
						return;

					isLoaded = true;
					indicator.Text = (seekBar.Progress).ToString ();
					indicator.Visibility = ViewStates.Visible;

					SeekBarIndicator (seekBar, indicator);
				};

				seekBar.ProgressChanged += delegate (object sender, SeekBar.ProgressChangedEventArgs args) {
					if (!args.FromUser)
						return;

					indicator.Text = (seekBar.Progress).ToString ();

					SeekBarIndicator (seekBar, indicator);
				};

				alert.Show ();
			};
			fontIcon.Click += delegate {
				var listOfTypeface = new List<TypefaceModel> ();
				listOfTypeface.Add (new TypefaceModel (Typeface.CreateFromAsset (Assets, "AlexBrush-Regular.ttf"), "Brush"));
				listOfTypeface.Add (new TypefaceModel (Typeface.CreateFromAsset (Assets, "Aller_Rg.ttf"), "Aller"));
				listOfTypeface.Add (new TypefaceModel (Typeface.CreateFromAsset (Assets, "ostrich-regular.ttf"), "Ostrich"));
				listOfTypeface.Add (new TypefaceModel (Typeface.Create (Typeface.Default, TypefaceStyle.Normal), "Normal"));
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetTitle (GetString (Resource.String.Selectfonttype));
				alert.SetNegativeButton (GetString (Resource.String.cancelbutton), delegate {

				});
				var listAdapter = new TypeFaceListAdapter (this, listOfTypeface);
				var infate = LayoutInflater.Inflate (Resource.Layout.textItems_listview, null);
				var listViewType = infate.FindViewById<ListView> (Resource.Id.listview);						
				var loadingType = infate.FindViewById<RelativeLayout> (Resource.Id.main_loading);
				loadingType.Visibility = ViewStates.Gone;
				listViewType.Visibility = ViewStates.Visible;
				alert.SetView (infate);
				listViewType.Adapter = listAdapter;
				AlertDialog alertDialog = alert.Show ();
				listViewType.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					alertDialog.Dismiss ();
					imageView.TypefaceText (listOfTypeface [e.Position].Typeface);
					imageView.Invalidate ();
				};
//				alert.SetAdapter (listAdapter, delegate(object sender, DialogClickEventArgs e) {
//					imageView.TypefaceText (listOfTypeface [e.Which].Typeface);
//					imageView.Invalidate ();
//				});					                               



			};
		}

		private void GetData (AlertDialog dialog)
		{
			var reqUrl = "https://wp-of-happiness.firebaseio.com/availableText.json";
			var request = (HttpWebRequest)WebRequest.Create (reqUrl);
			request.Timeout = 10000;
			request.Method = "GET";
			request.ContentType = "application/json";

			try {
				var response = (HttpWebResponse)request.GetResponse ();
				var reader = new System.IO.StreamReader (response.GetResponseStream ());
				var streamText = reader.ReadToEnd ();
				textList = JsonConvert.DeserializeObject <TextModel> (streamText);

			} catch (Exception ex) {				
				retry = true;
			}


			RunOnUiThread (() => {
				if (retry) {
					Toast.MakeText (this, GetString (Resource.String.ValidationText), ToastLength.Short);
					retry = false;
				} else {
					if (textList.ToString ().Equals ("")) {
						Toast.MakeText (this, GetString (Resource.String.ValidationText), ToastLength.Short);
					} else {
						loading.Visibility = ViewStates.Gone;
						listView.Visibility = ViewStates.Visible;
						var values = textList.english;
						var listAdapter = new TextListAdapter (this, values);
						listView.Adapter = listAdapter;
						listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
							dialog.Dismiss ();
							imageView.PutText (values [e.Position]);
							imageView.EnterText (WindowManager.DefaultDisplay.Height);
							imageView.Invalidate ();
						};
					}
				}
			});
		}

		private void SeekBarIndicator (SeekBar seekBar, TextView indicator)
		{
			RelativeLayout.LayoutParams p = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
			Rect thumbRect = seekBar.Thumb.Bounds;
			var abc = thumbRect.CenterX ();				
			p.SetMargins (abc, 0, 0, 0);
			indicator.LayoutParameters = p;
		}

		private void SelectText ()
		{
			
			var alert = new Android.App.AlertDialog.Builder (this);
			alert.SetTitle (GetString (Resource.String.ChoseText));
			var infate = LayoutInflater.Inflate (Resource.Layout.textItems_listview, null);
			alert.SetView (infate);
			AlertDialog alertDialog = alert.Show ();
			listView = infate.FindViewById<ListView> (Resource.Id.listview);						
			loading = infate.FindViewById<RelativeLayout> (Resource.Id.main_loading);
//			var dialog = new Dialog (this);
//			dialog.SetTitle (GetString (Resource.String.ChoseText));
//			dialog.Window.RequestFeature (WindowFeatures.NoTitle);
//			dialog.SetContentView (Resource.Layout.textItems_listview);
//			dialog.Window.SetGravity (GravityFlags.Center);
//			dialog.Window.SetLayout (WindowManager.DefaultDisplay.Width - 100, WindowManagerLayoutParams.WrapContent);
//			dialog.SetCancelable (true);
//			dialog.SetTitle (GetString (Resource.String.ChoseText));
//			dialog.SetCanceledOnTouchOutside (true);
//			listView = dialog.FindViewById<ListView> (Resource.Id.listview);	
//			loading = dialog.FindViewById<RelativeLayout> (Resource.Id.main_loading);
//			dialog.Show ();
			ThreadPool.QueueUserWorkItem (o => GetData (alertDialog));

//			var values = textList.english;
//			var listAdapter = new TextListAdapter (this, values);
//			listView.Adapter = listAdapter;
//			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
//				dialog.Dismiss ();
//				imageView.PutText (values [e.Position]);
//				imageView.EnterText (WindowManager.DefaultDisplay.Height);
//				imageView.Invalidate ();
//			};

		}

		private void GetBitmap (String path)
		{
			var uri = GetImageUri (path);
			var Uti = Android.Net.Uri.Parse (path);
			Bitmap b = null;
			System.IO.Stream ins = null;

			try {
				int IMAGE_MAX_SIZE = 1024;
				ins = ContentResolver.OpenInputStream (Uti);

				// Decode image size
				var o = new BitmapFactory.Options ();
				o.InJustDecodeBounds = true;

				BitmapFactory.DecodeStream (ins, null, o);
				ins.Close ();

				int scale = 1;
				if (o.OutHeight > IMAGE_MAX_SIZE || o.OutWidth > IMAGE_MAX_SIZE) {
					scale =
						(int)
						Math.Pow (2,
						(int)
							Math.Round (Math.Log (IMAGE_MAX_SIZE / (double)Math.Max (o.OutHeight, o.OutWidth)) /
						Math.Log (0.5)));
				}

				var o2 = new BitmapFactory.Options ();
				o2.InSampleSize = scale;
				ins = ContentResolver.OpenInputStream (Uti);

				b = BitmapFactory.DecodeStream (ins, null, o2);
				b = rotateBitmap (b, Uti);
				if (extras.GetString ("image-path") != null) {
					imageView.SetScaleType (ImageView.ScaleType.FitXy);
				}
				if (b.Width >= WindowManager.DefaultDisplay.Width) {
					
				}
				ins.Close ();


			} catch (Exception e) {
				retry = true;
				RunOnUiThread (() => {
					var toast = Toast.MakeText (this, GetString (Resource.String.ValidationRequestTimeOut), ToastLength.Short);
					toast.Show ();
				});
			}
			RunOnUiThread (() => {
				if (retry) {
					return;
				}
				if (Uti.Path.Contains ("capture")) {
					var tempoFile = new File (Uti.Path);
					tempoFile.Delete ();
				}
				imageView.SetImageBitmap (b);
				mainLayout.Visibility = ViewStates.Visible;
				mainLoading.Visibility = ViewStates.Gone;
			});
		}

		public Bitmap rotateBitmap (Bitmap bitmap, Android.Net.Uri uti)
		{
			try {				
				File f = new File (uti.Path);
				ExifInterface exif = new ExifInterface (f.Path);
				var orientation = exif.GetAttribute (ExifInterface.TagOrientation);
				Matrix matrix = new Matrix ();

				switch (orientation) {
				case "6":
					matrix.SetRotate (90);
					break;				
				default:
					return bitmap;
				}

				try {
					Bitmap oriented = Bitmap.CreateBitmap (bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
					bitmap.Recycle ();
					return oriented;
				} catch (Java.Lang.OutOfMemoryError e) {
					e.PrintStackTrace ();
					return bitmap;
				}
			} catch (IOException e) {
				e.PrintStackTrace ();
			}

			return bitmap;
		}

		private Android.Net.Uri GetImageUri (String path)
		{
			return Android.Net.Uri.FromFile (new Java.IO.File (path));
		}

		private void SaveImage (Bitmap finalBitmap)
		{					
			File myDir = new File (Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryPictures), "Wallpapers");
			if (!myDir.Exists ()) {
				myDir.Mkdirs ();
			}
			Random generator = new Random ();
			int n = 10000;
			n = generator.Next (n);
			String fname = "Image-" + n + ".jpg";
			File file = new File (myDir, fname);
			Intent mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
			var contentUri = Android.Net.Uri.FromFile (file);
			mediaScanIntent.SetData (contentUri);
			SendBroadcast (mediaScanIntent);

			if (file.Exists ())
				file.Delete (); 
			try {
				FileOutputStream fo = new FileOutputStream (file);
				System.IO.MemoryStream stream = new System.IO.MemoryStream ();
				finalBitmap.Compress (Bitmap.CompressFormat.Png, 100, stream);
				finalBitmap.Recycle ();
				byteArray = stream.ToArray ();
				fo.Write (byteArray);
				fo.Close ();	
			} catch (Exception ex) {					
				retry = true;
				RunOnUiThread (() => {
					var toast = Toast.MakeText (this, GetString (Resource.String.ValidationRequestTimeOut), ToastLength.Short);
					toast.Show ();
				});
			}			
			RunOnUiThread (() => {
				if (retry) {
					return;
				}
				mainLoading.Visibility = ViewStates.Gone;
				var alert = new Android.App.AlertDialog.Builder (this);
				alert.SetTitle (GetString (Resource.String.saveWallpapers));
				alert.SetNegativeButton (GetString (Resource.String.cancelbutton), delegate {
					OnBackPressed ();
				});
				alert.SetPositiveButton (GetString (Resource.String.saveWallpapersMess), delegate {
					DialogToSetWallpaper (byteArray);
				});
				var alertDialog = alert.Create ();
				alertDialog.SetTitle (Resources.GetString (Resource.String.saveWallpapers));
				alertDialog.Show ();
			});
		}

		private void DialogToSetWallpaper (byte[] bitmap)
		{	
			WallpaperManager myWallpaper = WallpaperManager.GetInstance (this);
			try {
				int height = Resources.DisplayMetrics.HeightPixels;
				int width = Resources.DisplayMetrics.WidthPixels;
				myWallpaper.SetBitmap (GetResizedBitmap (Decode (bitmap), width, height));
			} catch (IOException e) {
				e.PrintStackTrace ();
			}
			OnBackPressed ();
		}

		public Bitmap GetResizedBitmap (Bitmap bm, int newWidth, int newHeight)
		{
			int width = bm.Width;
			int height = bm.Height;
			float scaleWidth = ((float)newWidth) / width;
			float scaleHeight = ((float)newHeight) / height;
			Matrix matrix = new Matrix ();
			matrix.PostScale (scaleWidth, scaleHeight);
			Bitmap resizedBitmap = Bitmap.CreateBitmap (bm, 0, 0, width, height, matrix, false);
			bm.Recycle ();	
			return resizedBitmap;
		}

		public  Bitmap Decode (byte[] encodeByte)
		{
			try {				
				Bitmap bitmap = BitmapFactory.DecodeByteArray (encodeByte, 0, encodeByte.Length);
				return bitmap;
			} catch (Exception) {
				return null;
			}
		}

		public void GetImageBitmapFromUrl (string url)
		{
			Bitmap imageBitmap = null;
			using (var webClient = new WebClient ()) {
				try {					
					var imageBytes = webClient.DownloadData (url);
					if (imageBytes != null && imageBytes.Length > 0) {
						imageBitmap = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
						//					var heigh = (int)context.Resources.DisplayMetrics.HeightPixels / 4;
						//					var width = Convert.ToInt32 (context.Resources.DisplayMetrics.WidthPixels / 3.5);
						//					newImageBitmap = GetResizedBitmap (imageBitmap, heigh, width);
					}

				} catch (Exception ex) {					
					retry = true;
					RunOnUiThread (() => {
						var toast = Toast.MakeText (this, GetString (Resource.String.ValidationRequestTimeOut), ToastLength.Short);
						toast.Show ();
					});
				}			
				RunOnUiThread (() => {
					if (retry) {
						return;
					}
					imageView.SetImageBitmap (imageBitmap);
					mainLayout.Visibility = ViewStates.Visible;
					mainLoading.Visibility = ViewStates.Gone;
				});
			}

		}

		public Bitmap LoadAndResizeBitmap (string fileName, int width, int height)
		{			
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile (fileName, options);


			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width) {
				inSampleSize = outWidth > outHeight
					? outHeight / height
					: outWidth / width;
			}

			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile (fileName, options);

			return resizedBitmap;
		}

		protected void ShowRetry (RelativeLayout loading, Context context)
		{
			RunOnUiThread (() => {

				var progress = loading.FindViewById<ProgressBar> (Resource.Id.splash_progressBar);
				var retry = loading.FindViewById<Button> (Resource.Id.loading_retry);

				progress.Visibility = ViewStates.Gone;
				retry.Visibility = ViewStates.Visible;

				retry.Click += delegate {					
					
				};
			});
		}
	}


}

