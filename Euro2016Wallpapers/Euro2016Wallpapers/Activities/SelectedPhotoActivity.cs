
using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Uri = Android.Net.Uri;
using Android.Net;
using Java.IO;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Util;
using System.IO;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.Design;
using Android.Support.Design.Widget;
using Android.Content.Res;
using Java.Net;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Reflection;
using Euro2016Wallpapers.Services;
using System.Threading.Tasks;

namespace Euro2016Wallpapers
{
	[Activity (MainLauncher = true, Label = "@string/app_name", Theme = "@style/AppTheme", ConfigurationChanges =
		(Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]		


	public class SelectedPhotoActivity : BaseActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;
		private string requestURL = "https://api.cloudinary.com/v1_1/wp-of-happiness/resources/image/upload/?prefix=";
		private const string ApiKey = "966956932715847";
		private const string ApiSecret = "grc0mV1_k8xuV8xLYZgPGMpbwDw";
	
		private LinearLayout mainSlider;
		private List<ImageModel> images;
		private List<ImageModel> images1;
		private RecyclerView recyclerView;
		private List<Bitmap> bitmaps;
		private ImageAdapter adapter;
		private bool newestBool = false;
		private bool categoryBool = false;
		private bool randomBool = false;

		private bool playersBool = false;
		private bool countryBool = false;
		private bool legendsBool = false;

		private bool retrying = false;
		private bool loadingData = false;

		private TextView newest;
		private TextView category;
		private TextView random;
		private TextView item;

		private LinearLayout playerslayout;
		private LinearLayout countrylayout;
		private LinearLayout legendslayout;

		private RelativeLayout loading;
		private RelativeLayout homePage;
		private RelativeLayout loadingRec;
		private MemoryLimitedLruCache _memoryCache;

		protected static IDatabaseServices DatabaseServices;
		private static readonly string DatabaseDirectory =	System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "../databases");
		public const string DatabaseFileName = "WOHdb";
		public long free;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.select_photo_drawer);
			ConstructActionBar ();
			UpdateTexts ();
			SetTitle (GetString (Resource.String.wallpapers));
			//DatabaseServices = new DataBaseServices (this);
			//CreateSqLiteDatabase ();
			loading = FindViewById<RelativeLayout> (Resource.Id.main_loading);
			loadingRec = FindViewById<RelativeLayout> (Resource.Id.main_loading_recycler);
			homePage = FindViewById<RelativeLayout> (Resource.Id.homepage);
			homePage.Visibility = ViewStates.Visible;
			bitmaps = new List<Bitmap> ();
			ThreadPool.QueueUserWorkItem (o => GetData ("newest"));
			recyclerView = FindViewById<RecyclerView> (Resource.Id.image_recycler);
			GridLayoutManager glm = new GridLayoutManager (this, 3);
			recyclerView.SetLayoutManager (glm);
			mainSlider = FindViewById<LinearLayout> (Resource.Id.main_slider);

			newest = mainSlider.FindViewById<TextView> (Resource.Id.newestItem);
			category = mainSlider.FindViewById<TextView> (Resource.Id.categoryItem);
			random = mainSlider.FindViewById<TextView> (Resource.Id.randomItem);
			newest.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);

			playerslayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.playerslayout);
			countrylayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.countrylayout);
			legendslayout = mainSlider.FindViewById<LinearLayout> (Resource.Id.legendslayout);
			var maxMemory = (int)(Java.Lang.Runtime.GetRuntime ().MaxMemory () / 1024);
			var cacheSize = maxMemory / 2;
			_memoryCache = new MemoryLimitedLruCache (cacheSize);
			newestBool = true;
			newest.Click += delegate {
				if (loadingData) {
					return;
				}
				loadingData = true;
				if (newestBool) {
					return;
				} else {				
					recyclerView.Visibility = ViewStates.Gone;
					loadingRec.Visibility = ViewStates.Visible;
					ThreadPool.QueueUserWorkItem (o => GetData ("newest"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						newest.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						newestBool = true;
					});
				}
			};
			category.Click += delegate {
				if (loadingData) {
					return;
				}
				loadingData = true;
				if (categoryBool) {
					return;
				} else {			
					recyclerView.Visibility = ViewStates.Gone;
					loadingRec.Visibility = ViewStates.Visible;
					ThreadPool.QueueUserWorkItem (o => GetData ("categories/players"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						ClickCategoryValidator ();
						mainSlider.FindViewById<LinearLayout> (Resource.Id.category_slider).Visibility = ViewStates.Visible;
						category.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						mainSlider.FindViewById<TextView> (Resource.Id.playersText).SetTextColor (Resources.GetColor (Resource.Color.blue_text_euro));
						mainSlider.FindViewById<ImageView> (Resource.Id.playersImage).SetImageResource (Resource.Drawable.ic_love_green);
						categoryBool = true;
						playersBool = true;
					});
				}

			};
			random.Click += delegate {
				if (loadingData) {
					return;
				}
				loadingData = true;
				if (randomBool) {
					return;
				} else {	
					recyclerView.Visibility = ViewStates.Gone;
					loadingRec.Visibility = ViewStates.Visible;
					ThreadPool.QueueUserWorkItem (o => GetData ("random"));
					RunOnUiThread (() => {
						ClickValidator ().SetBackgroundResource (Color.Transparent);
						random.Background = Resources.GetDrawable (Resource.Drawable.button_round_green);
						randomBool = true;
					});
				}
			};
			playerslayout.Click += delegate {
				if (loadingData) {
					return;
				}
				loadingData = true;
				playerslayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/players"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.playersText).SetTextColor (Resources.GetColor (Resource.Color.blue_text_euro));
					mainSlider.FindViewById<ImageView> (Resource.Id.playersImage).SetImageResource (Resource.Drawable.ic_love_green);
					ClickCategoryValidator ();
					playersBool = true;
				});
			};
			countrylayout.Click += delegate {
				if (loadingData) {
					return;
				}
				loadingData = true;
				countrylayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/country"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.countryText).SetTextColor (Resources.GetColor (Resource.Color.blue_text_euro));
					mainSlider.FindViewById<ImageView> (Resource.Id.countryImage).SetImageResource (Resource.Drawable.ic_happiness_green);
					ClickCategoryValidator ();
					countryBool = true;
				});

			};
			legendslayout.Click += delegate {
				if (loadingData) {
					return;
				}
				loadingData = true;
				legendslayout.Clickable = false;
				recyclerView.Visibility = ViewStates.Gone;
				loadingRec.Visibility = ViewStates.Visible;
				ThreadPool.QueueUserWorkItem (o => GetData ("categories/legends"));
				RunOnUiThread (() => {
					mainSlider.FindViewById<TextView> (Resource.Id.legendsText).SetTextColor (Resources.GetColor (Resource.Color.blue_text_euro));
					mainSlider.FindViewById<ImageView> (Resource.Id.legendsImage).SetImageResource (Resource.Drawable.ic_sport_green);
					ClickCategoryValidator ();
					legendsBool = true;
				});
			};

		}

		private Dictionary<string, List<ImageModel>>  ConstructImageFromApp ()
		{	
			var listOfImages = new Dictionary<string, List<ImageModel>> ();
			var newestList = new List<ImageModel> ();
			var playersList = new List<ImageModel> ();
			var countryList = new List<ImageModel> ();
			var legendsList = new List<ImageModel> ();
//			var coupleList = new List<ImageModel> ();
//			var motivationList = new List<ImageModel> ();
			newestList.Add (new ImageModel () {
				version = Resource.Drawable.sport_213, type = "local"
			});
			newestList.Add (new ImageModel () {
				version = Resource.Drawable.happiness_212, type = "local"
			});
			playersList.Add (new ImageModel () {
				version = Resource.Drawable.love_212, type = "local"
			});
			playersList.Add (new ImageModel () {
				version = Resource.Drawable.love_213, type = "local"
			});
			countryList.Add (new ImageModel () {
				version = Resource.Drawable.happiness_212, type = "local"
			});
			countryList.Add (new ImageModel () {
				version = Resource.Drawable.happiness_213, type = "local"
			});
			legendsList.Add (new ImageModel () {
				version = Resource.Drawable.sport_212, type = "local"
			});
			legendsList.Add (new ImageModel () {
				version = Resource.Drawable.sport_213, type = "local"
			});
//			coupleList.Add (new ImageModel () {
//				version = Resource.Drawable.couple_212, type = "local"
//			});
//			coupleList.Add (new ImageModel () {
//				version = Resource.Drawable.couple_213, type = "local"
//			});
//			motivationList.Add (new ImageModel () {
//				version = Resource.Drawable.motivation_212, type = "local"
//			});
//			motivationList.Add (new ImageModel () {
//				version = Resource.Drawable.motivation_213, type = "local"
//			});
			listOfImages.Add ("newest", newestList);
			listOfImages.Add ("categories/players", playersList);
			listOfImages.Add ("categories/country", countryList);			
			listOfImages.Add ("categories/legends", legendsList);
//			listOfImages.Add ("categories/couple", coupleList);
//			listOfImages.Add ("categories/motivation", motivationList);

			return listOfImages;
		}

		private void ClickCategoryValidator ()
		{
			if (playersBool) {
				playersBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.playersText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.playersImage).SetImageResource (Resource.Drawable.ic_love);
				playerslayout.Clickable = true;
			} else if (countryBool) {
				countryBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.countryText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.countryImage).SetImageResource (Resource.Drawable.ic_happiness);
				countrylayout.Clickable = true;
			} else if (legendsBool) {
				legendsBool = false;
				mainSlider.FindViewById<TextView> (Resource.Id.legendsText).SetTextColor (Resources.GetColor (Resource.Color.gray));
				mainSlider.FindViewById<ImageView> (Resource.Id.legendsImage).SetImageResource (Resource.Drawable.ic_sport);
				legendslayout.Clickable = true;
//			} else if (coupleBool) {
//				coupleBool = false;
//				mainSlider.FindViewById<TextView> (Resource.Id.CoupleText).SetTextColor (Resources.GetColor (Resource.Color.gray));
//				mainSlider.FindViewById<ImageView> (Resource.Id.CoupleImage).SetImageResource (Resource.Drawable.ic_couple);
//				couplelayout.Clickable = true;
//			} else if (motivationBool) {
//				motivationBool = false;
//				mainSlider.FindViewById<TextView> (Resource.Id.MotivationText).SetTextColor (Resources.GetColor (Resource.Color.gray));
//				mainSlider.FindViewById<ImageView> (Resource.Id.MotivationImage).SetImageResource (Resource.Drawable.ic_motivation);
//				motivationlayout.Clickable = true;
			}
			//return item;
		}

		private TextView ClickValidator ()
		{
			
			if (newestBool) {
				newestBool = false;
				item = newest;
			} else if (categoryBool) {
				categoryBool = false;
				item = category;
				ClickCategoryValidator ();
				mainSlider.FindViewById<LinearLayout> (Resource.Id.category_slider).Visibility = ViewStates.Gone;
			} else if (randomBool) {
				randomBool = false;
				item = random;
			}
			return item;
		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			var displayMetrics = new DisplayMetrics ();
			WindowManager.DefaultDisplay.GetMetrics (displayMetrics);
			var x = displayMetrics.WidthPixels / displayMetrics.Xdpi;

			int cardsNumber = (int)(x / 1.22);
			cardsNumber = cardsNumber > 1 ? cardsNumber : 1;


			base.OnWindowFocusChanged (hasFocus);
		}

		private void GetData (string type)
		{			
			images = new List<ImageModel> ();

			switch (type) {
			case "newest":
				{	
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/players":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/country":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
			case "categories/legends":
				{
					images = ConstructImageFromApp () [type];
					break;
				}
//			case "categories/couple":
//				{
//					images = ConstructImageFromApp () [type];
//					break;
//				}
//			case "categories/motivation":
//				{
//					images = ConstructImageFromApp () [type];
//					break;
//				}
			}

			var reqUrl = string.Format ("{0}{1}/&max_results=500", requestURL, type);
			var request = (HttpWebRequest)WebRequest.Create (reqUrl);
			request.Timeout = 10000;
			request.Method = "GET";
			request.ContentType = "application/json";
			request.Credentials = CredentialCache.DefaultCredentials;
			var encoded = System.Convert.ToBase64String (System.Text.Encoding.GetEncoding ("ISO-8859-1").GetBytes (ApiKey + ":" + ApiSecret));

			request.Headers.Add ("Authorization", "Basic " + encoded);
			try {
				var response = (HttpWebResponse)request.GetResponse ();
				var reader = new StreamReader (response.GetResponseStream ());
				var streamText = reader.ReadToEnd ();
				var deserializedStreamText = JsonConvert.DeserializeObject<Images> (streamText);
				images.AddRange (deserializedStreamText.resources);

			} catch (Exception ex) {
				HandleErrors (ex);
				retrying = true;
			}
			RunOnUiThread (() => {
				
				if (retrying) {
					
//					ShowRetry (loading, this);
//					loading.Visibility = ViewStates.Visible;
//					return;
				}
				ThreadPool.QueueUserWorkItem (o => DownloadImage ());
			});
		}

		private void DownloadImage ()
		{
//			if (bitmaps.Count > 0) {	
//				bitmaps.Clear ();
//			}
//			bitmaps = new List<Bitmap> ();
//			free = Java.Lang.Runtime.GetRuntime ().FreeMemory ();
//			foreach (var img in images) {		
//				if (!img.type.Equals ("local")) {	
//					if (DatabaseServices.CheckExist (img.url)) {
//						var bitm = DatabaseServices.GetImage (img.url);
//						bitmaps.Add (bitm);
//					} else {
//						var bitm = GetImageBitmapFromUrl (img.url);
//						bitmaps.Add (bitm);
//					}
////					if (_memoryCache.Get (img.url) == null) {
////						var bitm = GetImageBitmapFromUrl (img.url);
////						_memoryCache.Put (img.url, bitm);
////						DatabaseServices.GetImage (img.url);
////						bitmaps.Add (bitm);
////					} else {
////						var bitm = (Bitmap)_memoryCache.Get (img.url);
////						bitmaps.Add (bitm);
////					}
//				} else {	
////					var heigh = 170 * Resources.DisplayMetrics.Density;
//////					var bitm1 = ((BitmapDrawable)Resources.GetDrawable (img.version)).Bitmap;
//////					var bitscale = Bitmap.CreateScaledBitmap (bitm1, Resources.DisplayMetrics.WidthPixels / 3, (int)heigh, false);
////					var total = Java.Lang.Runtime.GetRuntime ().TotalMemory ();
////					free = Java.Lang.Runtime.GetRuntime ().FreeMemory ();
////					var bitscale = DecodeSampledBitmapFromResource (Resources, img.version, 100, 100);
//
//					BitmapFactory.Options options = await GetBitmapOptionsOfImage (img.version);
//					Bitmap bitmaptodispaly = await LoadScaledDownBitmapForDisplayAsync (Resources, options, 100, 100, img.version);
//					bitmaps.Add (bitmaptodispaly);
//				}
//
//			}
			RunOnUiThread (() => {
				homePage.Visibility = ViewStates.Gone;
				FindViewById<LinearLayout> (Resource.Id.selectedpagelayout).Visibility = ViewStates.Visible;
				free = Java.Lang.Runtime.GetRuntime ().FreeMemory ();
				adapter = new ImageAdapter (this, images);
				free = Java.Lang.Runtime.GetRuntime ().FreeMemory ();
				adapter.ItemClick += OnItemClick;
				recyclerView.SetAdapter (adapter);
				recyclerView.Visibility = ViewStates.Visible;
				loadingData = false;
			});
		}

		public Bitmap GetUrl (string url)
		{
			URL urlul = new URL (url);
			var Uti = Uri.Parse (urlul.ToURI ().ToString ());
			//var ins = ContentResolver.OpenInputStream (Uti);
			var ins = urlul.OpenConnection ().InputStream;
			var bmp = BitmapFactory.DecodeStream (ins);
			return bmp;
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
			return resizedBitmap;
		}

		public Bitmap DecodeSampledBitmapFromResource (Resources res, int resId,	int reqWidth, int reqHeight)
		{

			BitmapFactory.Options options = new BitmapFactory.Options ();
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeResource (res, resId, options);

			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize (options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;
			return BitmapFactory.DecodeResource (res, resId, options);
		}


		void OnItemClick (object sender, int position)
		{			
			var intent = new Intent (this, typeof(DownloadActivity));
			if (images [position].type.Equals ("local")) {
				var urlul = images [position].version;
				intent.PutExtra ("image-path", urlul);
			} else {
				intent.PutExtra ("image-number", images [position].url);
				intent.PutExtra ("image-name", images [position].public_id);
			}
			StartActivity (intent);
		}

		private bool CreateSqLiteDatabase ()
		{
			var strSqLitePathOnDevice = GetSQLitePathOnDevice ();
			var isSqLiteInitialized = false;
			try {
				if (System.IO.File.Exists (strSqLitePathOnDevice)) {
					isSqLiteInitialized = true;
				} else {
					var streamSqLite = Assets.Open (DatabaseFileName);
					Directory.CreateDirectory (DatabaseDirectory);
					var streamWrite = new FileStream (strSqLitePathOnDevice, FileMode.OpenOrCreate,
						                  FileAccess.Write);
					if (streamSqLite != null) {
						if (CopySQLiteOnDevice (streamSqLite, streamWrite)) {
							isSqLiteInitialized = true;
						}
					}
				}
			} catch (Exception) {
				var currentMethod = MethodBase.GetCurrentMethod ();
				/*if (currentMethod.DeclaringType != null)
                Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                    , currentMethod.DeclaringType.FullName
                    , currentMethod.Name
                    , exception.Message));*/
			}
			return isSqLiteInitialized;
		}

		private string GetSQLitePathOnDevice ()
		{
			var strSqLitePathOnDevice = string.Empty;
			try {
				strSqLitePathOnDevice = System.IO.Path.Combine (DatabaseDirectory, DatabaseFileName);
			} catch (Exception) {
				var currentMethod = MethodBase.GetCurrentMethod ();
				/* Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                 , currentMethod.DeclaringType.FullName
                 , currentMethod.Name
                 , exception.Message));*/
			}
			return strSqLitePathOnDevice;
		}

		private bool CopySQLiteOnDevice (Stream streamSqLite, Stream streamWrite)
		{
			bool isSuccess = false;
			const int length = 256;
			var buffer = new Byte[length];
			try {
				int bytesRead = streamSqLite.Read (buffer, 0, length);
				while (bytesRead > 0) {
					streamWrite.Write (buffer, 0, bytesRead);
					bytesRead = streamSqLite.Read (buffer, 0, length);
				}
				isSuccess = true;
			} catch (Exception) {
				var currentMethod = MethodBase.GetCurrentMethod ();
				/* Console.WriteLine(String.Format("CLASS : {0}; METHOD : {1}; EXCEPTION : {2}"
                    , currentMethod.DeclaringType.FullName
                    , currentMethod.Name
                    , exception.Message));*/
			} finally {
				streamSqLite.Close ();
				streamWrite.Close ();
			}
			return isSuccess;

		}

		public static int CalculateInSampleSize (BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth) {
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth) {
					inSampleSize *= 2;
				}

			}

			return (int)inSampleSize;
		}

		public async Task<Bitmap> LoadScaledDownBitmapForDisplayAsync (Resources res, BitmapFactory.Options options, int reqWidth, int reqHeight, int resou)
		{
			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize (options, reqWidth, reqHeight);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeResourceAsync (res, resou, options);
		}

		async Task<BitmapFactory.Options> GetBitmapOptionsOfImage (int resou)
		{
			BitmapFactory.Options options = new BitmapFactory.Options {
				InJustDecodeBounds = true
			};

			// The result will be null because InJustDecodeBounds == true.
			Bitmap result = await BitmapFactory.DecodeResourceAsync (Resources, resou, options);


			int imageHeight = options.OutHeight;
			int imageWidth = options.OutWidth;

			return options;
		}
	
	}
}

