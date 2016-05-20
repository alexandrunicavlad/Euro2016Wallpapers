
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
			var type = Intent.GetStringExtra ("type");
			if (type.Equals (GetString (Resource.String.morewallpapers))) {					
				ConstructActionBar (GetString (Resource.String.morewallpapers));
				FillWebView (GetString (Resource.String.moreappurl));
			} else if (type.Equals (GetString (Resource.String.scheduleresults))) {	
				ConstructActionBar (GetString (Resource.String.scheduleresults));
				FillWebView (GetString (Resource.String.resulturl));
			}
		}

		private void FillWebView (string url)
		{
			web_view = FindViewById<WebView> (Resource.Id.webview);
			web_view.Settings.JavaScriptEnabled = true;
			web_view.SetWebViewClient (new MyWebViewClient ());
			web_view.LoadUrl (url);
		}

		private void ConstructActionBar (string title)
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
			toolbar.FindViewById<TextView> (Resource.Id.titleName).Text = title;
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

