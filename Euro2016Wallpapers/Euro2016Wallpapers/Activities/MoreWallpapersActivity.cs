
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
using Android.Webkit;

namespace Euro2016Wallpapers
{
	[Activity (Label = "MoreWallpapersActivity", Theme = "@style/AppTheme", ConfigurationChanges =
		(Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]			
	public class MoreWallpapersActivity : ActionBarActivity
	{
		private Toolbar toolbar;
		WebView web_view;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.more_wallpapers_layout);
			ConstructActionBar ();
			web_view = FindViewById<WebView> (Resource.Id.webview);
			web_view.Settings.JavaScriptEnabled = true;
			web_view.SetWebViewClient (new MyWebViewClient ());
			web_view.LoadUrl ("http://www.google.com");
		}

		private void ConstructActionBar ()
		{
			toolbar = FindViewById<Toolbar> (Resource.Id.tool_bar);
			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetDisplayShowHomeEnabled (true);
			toolbar.NavigationClick += delegate {
				OnBackPressed ();
			};

			toolbar.NavigationIcon = Resources.GetDrawable (Resource.Drawable.ic_back);
			toolbar.FindViewById<TextView> (Resource.Id.titleName).Text = GetString (Resource.String.morewallpapers);


		}
	}

	public class MyWebViewClient : WebViewClient
	{
		public override bool ShouldOverrideUrlLoading (WebView view, string url)
		{
			view.LoadUrl (url);
			return true;
		}

		public override void OnPageStarted (WebView view, string url, Android.Graphics.Bitmap favicon)
		{
			base.OnPageStarted (view, url, favicon);
		}

		public override void OnPageFinished (WebView view, string url)
		{
			base.OnPageFinished (view, url);
		}

		public override void OnReceivedError (WebView view, ClientError errorCode, string description, string failingUrl)
		{
			base.OnReceivedError (view, errorCode, description, failingUrl);
		}
	}
}

