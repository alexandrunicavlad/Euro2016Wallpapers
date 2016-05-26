using Android.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.OS;
using Android.Widget;

namespace Euro2016Wallpapers
{
	[Activity (Theme = "@style/AppTheme", ConfigurationChanges =
		(Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]			
	public class TutorialActivity : ActionBarActivity
	{
		private Toolbar toolbar;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.tutorial_layout);
			ConstructActionBar (GetString (Resource.String.tutorial));
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
}

