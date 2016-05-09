
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
using Android.Support.Design.Widget;
using Android.Graphics;
using Android.Content.Res;
using Android.Util;

namespace Euro2016Wallpapers
{
	[Activity (Label = "SelectedPhotoActivity", Theme = "@style/AppTheme")]			
	public class  LanguageActivity : BaseActivity
	{
		private const int REQUEST_IMAGE_CAPTURE = 1;
		private const int REQUEST_IMAGE_ALBUM = 2;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.language_layout);
			SetLocale ("de");
			ConstructActionBar ();
			UpdateTexts ();
			SetTitle (GetString (Resource.String.Selectlanguage));

		}
	}
}

