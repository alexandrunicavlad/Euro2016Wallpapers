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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Provider;
using Android.Graphics;
using Uri = Android.Net.Uri;
using System.Net;
using Java.Net;
using Euro2016Wallpapers.Services;




namespace Euro2016Wallpapers
{
	[Activity (Label = "SelectedPhotoActivity")]			
	public class BaseActivity : ActionBarActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private Bitmap _bitmap;
		private Uri selectedImage;
		private Java.IO.File photo;
		private DrawerLayout drawerLayout;
		private ActionBarDrawerToggle mDrawerToggle;
		private Toolbar toolbar;
		private LinearLayout leftMenu;
		private long count = 0;
		protected static IDatabaseServices DatabaseServices;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		}

		protected void ConstructActionBar ()
		{
			toolbar = FindViewById<Toolbar> (Resource.Id.tool_bar);

			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false);

			drawerLayout = FindViewById<DrawerLayout> (Resource.Id.DrawerLayout);
			var navigationView = FindViewById<NavigationView> (Resource.Id.nav_view);
			//			var inflate = LayoutInflater.Inflate (Resource.Layout.header, null);
			//			navigationView.AddHeaderView (inflate);	
			leftMenu = FindViewById<LinearLayout> (Resource.Id.left_menu);

			var wallpapers = leftMenu.FindViewById<LinearLayout> (Resource.Id.wallpapersLayout);
			var tutorial = leftMenu.FindViewById<LinearLayout> (Resource.Id.tutorialLayout);
			var morewallpapers = leftMenu.FindViewById<LinearLayout> (Resource.Id.morewallpapersLayout);
			DatabaseServices = new DataBaseServices (this);

			var drawerToggle = new ActionBarDrawerToggle (this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			drawerLayout.SetDrawerListener (drawerToggle);
			drawerToggle.SyncState ();

//			takePhoto.Click += delegate {
//				Intent pictureIntent = new Intent (MediaStore.ActionImageCapture);
//				drawerLayout.CloseDrawers ();
//				try {
//					photo = this.CreateTemporaryFile ("picture", ".jpg");
//					photo.Delete ();
//				} catch (Exception ex) {
//					Toast.MakeText (this, GetString (Resource.String.Pleasechecksd), ToastLength.Short);
//				}				
//				selectedImage = Uri.FromFile (photo);
//				pictureIntent.PutExtra (MediaStore.ExtraOutput, selectedImage);
//				StartActivityForResult (pictureIntent, REQUEST_IMAGE_CAPTURE);
//
//			};

//			choseFromPhoto.Click += delegate {
//				drawerLayout.CloseDrawers ();
//				Intent intent = new Intent (Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
//				StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
//
//			};

			wallpapers.Click += delegate {
				drawerLayout.CloseDrawers ();
				if (this is SelectedPhotoActivity) {
					
				} else {
					StartActivity (typeof(SelectedPhotoActivity));
					Finish ();
				}
			};

			tutorial.Click += delegate {
				CreateDisclaimerDialog ();
				//StartActivityForResult (typeof(LanguageActivity), 1);
				drawerLayout.CloseDrawers ();
			};
		}

		protected void ShowRetry (RelativeLayout loading, Context context)
		{
			RunOnUiThread (() => {
				
				var progress = loading.FindViewById<ProgressBar> (Resource.Id.splash_progressBar);
				var retry = loading.FindViewById<Button> (Resource.Id.loading_retry);

				progress.Visibility = ViewStates.Gone;
				retry.Visibility = ViewStates.Visible;

				retry.Click += delegate {					
					StartActivity (context.GetType ());
					Finish ();
				};
			});
		}

		public void UpdateTexts ()
		{
			leftMenu.FindViewById<TextView> (Resource.Id.wallpapersText).Text = GetString (Resource.String.wallpapers);
			leftMenu.FindViewById<TextView> (Resource.Id.tutorialText).Text = GetString (Resource.String.tutorial);
			leftMenu.FindViewById<TextView> (Resource.Id.morewallpapersText).Text = GetString (Resource.String.morewallpapers);
		}

		public void SetLocale (string lang)
		{
			Resources.Configuration.Locale = new Java.Util.Locale (lang);
			Resources.UpdateConfiguration (Resources.Configuration, Resources.DisplayMetrics);
		}

		private Java.IO.File CreateTemporaryFile (String part, String ext)
		{
			var tempDir = Android.OS.Environment.ExternalStorageDirectory;
			tempDir = new Java.IO.File (tempDir.AbsolutePath + "/capture/");
			if (!tempDir.Exists ()) {
				tempDir.Mkdirs ();
			}
			return Java.IO.File.CreateTempFile (part, ext, tempDir);
		}

		public void SetTitle (string title)
		{
			toolbar.FindViewById<TextView> (Resource.Id.titleName).Text = title;
		}

		public void SetIconRight (int resource)
		{
			toolbar.FindViewById<ImageView> (Resource.Id.iconRight).SetImageResource (resource);
		}

		public Bitmap GetImageBitmapFromUrl (string url)
		{
			Bitmap imageBitmap = null;
			using (var webClient = new WebClient ()) {
				try {					
					var position = url.IndexOf ("upload");
					var subUrl = url.Substring (0, position + 7);
					var afterUrl = url.Substring (position + 7);
					var heught = 170 * Resources.DisplayMetrics.Density;
					var newUrl = string.Format ("{0}w_{1},h_{2},c_fill/{3}", subUrl, Resources.DisplayMetrics.WidthPixels / 3, heught, afterUrl);
					var imageBytes = webClient.DownloadData (newUrl);
					count = count + imageBytes.Count ();
					if (imageBytes != null && imageBytes.Length > 0) {
						DatabaseServices.InsertImage (url, imageBytes);
						imageBitmap = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
						//					var heigh = (int)context.Resources.DisplayMetrics.HeightPixels / 4;
						//					var width = Convert.ToInt32 (context.Resources.DisplayMetrics.WidthPixels / 3.5);
						//					newImageBitmap = GetResizedBitmap (imageBitmap, heigh, width);
					}
				} catch (Exception ex) {					
				}

			}

			return imageBitmap;
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if ((requestCode == REQUEST_IMAGE_CAPTURE) && (resultCode == Result.Ok)) {	

				ContentResolver.NotifyChange (selectedImage, null);
				ContentResolver cr = ContentResolver;
				try {
					var intent = new Intent (this, typeof(PictureActivity));
					intent.SetDataAndType (selectedImage, "image/*");
					intent.PutExtra ("image-path", selectedImage.ToString ());
					StartActivityForResult (intent, REQUEST_IMAGE_CAPTURE);
				} catch (Exception e) {
					Toast.MakeText (this, GetString (Resource.String.Failedtoload), ToastLength.Short).Show ();
				}
			} else if ((requestCode == REQUEST_IMAGE_ALBUM) && (resultCode == Result.Ok) && (data != null)) {
				selectedImage = data.Data;
				try {
					var intent = new Intent (this, typeof(PictureActivity));
					intent.SetDataAndType (selectedImage, "image/*");
					intent.PutExtra ("image-path", selectedImage.ToString ());
					StartActivityForResult (intent, REQUEST_IMAGE_ALBUM);
					_bitmap = MediaStore.Images.Media.GetBitmap (this.ContentResolver, selectedImage);

				} catch (Exception) {

					var toast = Toast.MakeText (this, GetString (Resource.String.Failedtoload), ToastLength.Short);
					toast.Show ();
				}
			}
		}

		protected void HandleErrors (Exception e)
		{			
			switch (e.Message) {

			default:
				var message = e.Message;

				if (e is Newtonsoft.Json.JsonSerializationException) {
					ShowToast (Resource.String.TryAgainLater);
					break;
				} else {
					var splitedMessage = message.Split ('"');
					var splitedResponseStreamMessage = "";

					if (e.Message.Equals ("Request time out") || e.Message.Equals ("The request timed out")) {
						ShowToast (Resource.String.ValidationRequestTimeOut);
						break;
					}
					if (message.Length > 29) {
						splitedResponseStreamMessage = message.Substring (0, 29);
					}
					if (splitedMessage [0].Equals ("Error converting value ")) {
						message = Resources.GetString (Resource.String.ValidationErrorConverting);
					} else if (splitedResponseStreamMessage.Equals ("Error getting response stream")) {
						message = Resources.GetString (Resource.String.ValidationErrorConverting);
					} else if (e.Message.Equals ("Exception of type 'Java.Lang.IllegalStateException' was thrown.")) {
						message = Resources.GetString (Resource.String.TryAgain);
					} else if (e.Message.Equals ("Exception of type 'Android.Database.Sqlite.SQLiteException' was thrown.")) {
						message = Resources.GetString (Resource.String.TryAgain);
					} else if (e.Message.Equals ("Exception of type 'Android.Database.Sqlite.SQLiteMisuseException' was thrown.")) {
						message = Resources.GetString (Resource.String.TryAgain);
					} else if (e.Message.Equals ("Exception of type 'Java.Lang.NullPointerException' was thrown.")) {
						ShowToast (Resource.String.TryAgain);
						break;
					} else if (e.Message.Equals ("Error: NameResolutionFailure")) {
						ShowToast (Resource.String.ValidationRequestTimeOut);
						break;
					} else if (e.Message.Equals ("Cannot perform this operation because the connection pool has been closed.")) {
						ShowToast (Resource.String.TryAgain);
						break;
					} else if (e.Message.Equals ("Object reference not set to an instance of an object")) {
						ShowToast (Resource.String.ValidationCannotCast);
						break;
					} else if (e.Message.Contains ("'jobject' must not be IntPtr.Zero")) {
						ShowToast (Resource.String.ValidationIntPtr);
						break;
					} else if (e.Message.Contains ("Exception of type 'Java.IO.FileNotFoundException' was thrown.")) {
						ShowToast (Resource.String.ValidationIntPtr);
						break;
					} else if (e.Message.Contains ("attempt to re-open an already-closed object: SQLiteDatabase:")) {
						ShowToast (Resource.String.TryAgain);
						break;
					} else if (e.Message.Contains ("Error: ConnectFailure (Network is unreachable)")) {
						ShowToast (Resource.String.ValidationRequestTimeOut);
						break;
					} else {
						Xamarin.Insights.Report (e, Xamarin.Insights.Severity.Warning);
					}

				}

				//CreateDialog (message, true, false);
				break;
			}
		}

		protected void ShowToast (int message)
		{
			RunOnUiThread (() => {
				var toast = Toast.MakeText (this, message, ToastLength.Short);
				toast.Show ();
			});
		}

		protected void CreateDisclaimerDialog ()
		{
			var builder = new Android.App.AlertDialog.Builder (this);
			builder.SetPositiveButton (GetString (Resource.String.Okbutton), delegate {				
			});	
			var dialog = builder.Create ();	
			dialog.SetTitle (Resources.GetString (Resource.String.tutorial));
			dialog.SetMessage (GetString (Resource.String.ComingSoon));
			dialog.Show ();
		}


	}
}

