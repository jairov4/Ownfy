// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Android
{
	using global::Android.App;
	using global::Android.OS;
	using global::Android.Widget;

	[Activity(Label = "Ownfy.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private int count = 1;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			this.SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			var button = this.FindViewById<Button>(Resource.Id.MyButton);

			button.Click += delegate { button.Text = $"{count++} clicks!"; };
		}
	}
}