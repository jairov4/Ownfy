// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Android
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Core;
	using global::Android.App;
	using global::Android.Media;
	using global::Android.OS;
	using global::Android.Widget;
	using Org.Json;

	[Activity(Label = "Ownfy.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private ListView listView;
		private MediaPlayer player;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			this.SetContentView(Resource.Layout.Main);

			var playButton = this.FindViewById<Button>(Resource.Id.PlayButton);
			var pauseButton = this.FindViewById<Button>(Resource.Id.PauseButton);
			var stopButton = this.FindViewById<Button>(Resource.Id.StopButton);
			var searchView = this.FindViewById<SearchView>(Resource.Id.SearchView);
			this.listView = this.FindViewById<ListView>(Resource.Id.listView1);
			this.player = new MediaPlayer();

			playButton.Click += this.PlayButton_Click;
			pauseButton.Click += this.PauseButton_Click;
			stopButton.Click += this.StopButton_Click;
			searchView.QueryTextSubmit += this.SearchView_QueryTextSubmit;
			searchView.QueryTextChange += this.SearchView_QueryTextChange;
		}

		private void SearchView_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
		{
		}

		private async void SearchView_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
		{
			var client = new WebClient();
			var qstring = e.Query.Replace(".", string.Empty).Replace("/", string.Empty);
			var uri = new Uri("http://192.168.0.6:7565/search/" + qstring + ".json", UriKind.Absolute);
			var ret = await Task.Run(() => client.DownloadString(uri));
			var songs = this.MapSongs(ret);
			var adapter = new SongResultsAdapter(this, songs);
			this.listView.Adapter = adapter;
		}

		private List<Song> MapSongs(string ret)
		{
			var arr = new JSONArray(ret);
			var list = new List<Song>();
			for (var i = 0; i < arr.Length(); i++)
			{
				var item = (JSONObject)arr.Get(i);
				var id = item.GetInt("id");
				var name = item.GetString("name");
				var relativePath = item.GetString("relativePath");
				var artist = item.GetString("artist");
				var lenght = this.JsonToTimeSpan(item.GetJSONObject("length"));
				var lastModified = DateTime.Parse(item.GetString("lastModified"));
				var fileLenght = item.GetInt("fileLength");
				var song = new Song(id,
					name,
					relativePath,
					artist,
					lenght,
					lastModified,
					fileLenght);
				list.Add(song);
			}

			return list;
		}
		
		private TimeSpan JsonToTimeSpan(JSONObject time)
		{
			var days = time.GetInt("days");
			var hours = time.GetInt("hours");
			var minutes = time.GetInt("minutes");
			var seconds = time.GetInt("seconds");
			var milliseconds = time.GetInt("milliseconds");			
			return new TimeSpan(days, hours, minutes, seconds, milliseconds);
		}


		private void StopButton_Click(object sender, EventArgs e)
		{
		}

		private void PauseButton_Click(object sender, EventArgs e)
		{
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
		}
	}
}