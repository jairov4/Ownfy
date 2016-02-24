// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Android
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;
	using Core;
	using global::Android.App;
	using global::Android.Media;
	using global::Android.OS;
	using global::Android.Views;
	using global::Android.Widget;
	using Org.Json;
	using static System.Threading.Tasks.Task;
	using Uri = global::Android.Net.Uri;

	[Activity(Label = "Ownfy.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private SongResultsAdapter adapter;
		private ListView listView;
		private TextView loadingMessage;
		private MediaPlayer player;
		private ViewSwitcher switcher;
		private CancellationTokenSource searchToken;
		private TextView timeDisplay;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.SetContentView(Resource.Layout.Main);

			var playButton = this.FindViewById<Button>(Resource.Id.PlayPauseButton);
			var stopButton = this.FindViewById<Button>(Resource.Id.StopButton);
			var searchView = this.FindViewById<SearchView>(Resource.Id.SearchView);
			this.listView = this.FindViewById<ListView>(Resource.Id.listView1);
			this.timeDisplay = this.FindViewById<TextView>(Resource.Id.TimeDisplay);
			this.adapter = new SongResultsAdapter(this, new Song[0]);
			this.player = new MediaPlayer();
			
			this.switcher = this.FindViewById<ViewSwitcher>(Resource.Id.ViewSwitcher);
			this.loadingMessage = this.FindViewById<TextView>(Resource.Id.LoadingMessage);
			
			playButton.Click += this.PlayButton_Click;
			stopButton.Click += this.StopButton_Click;
			searchView.QueryTextSubmit += this.SearchView_QueryTextSubmit;
			searchView.QueryTextChange += this.SearchView_QueryTextChange;
			this.listView.ItemClick += this.ListView_ItemClick;
			this.player.BufferingUpdate += this.Player_BufferingUpdate;
			this.player.Error += this.Player_Error;

			this.ShowListViewMessage("Write in the search box to start.");
		}
		
		private void Player_Error(object sender, MediaPlayer.ErrorEventArgs e)
		{
			this.ShowListViewMessage($"Error on playback: {e.What}");
		}

		private void Player_BufferingUpdate(object sender, MediaPlayer.BufferingUpdateEventArgs e)
		{
			this.timeDisplay.Text = $"{e.Percent} %";
		}

		private void ShowListView()
		{
			if (this.switcher.CurrentView != this.loadingMessage) return;
			this.switcher.Reset();
			if (this.switcher.CurrentView == this.loadingMessage)
				this.switcher.ShowNext();
		}

		private void ShowListViewMessage(string message)
		{
			this.loadingMessage.Text = message;
			if (this.switcher.CurrentView != this.listView) return;
			this.switcher.Reset();
			if (this.switcher.CurrentView == this.listView)
				this.switcher.ShowNext();
		}

		private async void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			try
			{
				var song = this.adapter.GetItem(e.Position);
				this.timeDisplay.Text = song.Length.ToString("mm\\:ss");
				var url = Uri.Parse("http://hq.skivent.com.co:8080/stream/" + song.Id);
				await Run(() => this.player.Reset());

				this.player.SetDataSource(this, url);
				await Run(() => this.player.Prepare());
				this.player.Start();
			}
			catch (Exception ex)
			{
				this.ShowListViewMessage($"Error streaming music: {ex.Message}");
			}
		}

		private void SearchView_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
		{
		}

		private async void SearchView_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
		{
			try
			{
				this.searchToken?.Cancel();
				this.searchToken = new CancellationTokenSource();
				this.ShowListViewMessage("Loading...");
				this.adapter.Clear();
				var client = new WebClient();
				var qstring = e.Query.Replace(".", string.Empty).Replace("/", string.Empty);
				var uri = new System.Uri("http://hq.skivent.com.co:8080/search/" + qstring + ".json", UriKind.Absolute);
				var ret = await Run(() => client.DownloadString(uri), this.searchToken.Token);
				var songs = this.MapSongs(ret);
				if (songs.Any())
				{
					this.adapter = new SongResultsAdapter(this, songs);
					this.listView.Adapter = this.adapter;
					this.ShowListView();
				}
				else
				{
					this.ShowListViewMessage("No results");
				}
			}
			catch (Exception ex)
			{
				this.ShowListViewMessage($"Error loading search results: {ex.Message}");
			}
			finally
			{
				this.searchToken = null;
			}
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
			this.player.Stop();
			this.timeDisplay.Text = string.Empty;
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			if (this.player.IsPlaying) this.player.Pause();
			else this.player.Start();
		}
	}
}