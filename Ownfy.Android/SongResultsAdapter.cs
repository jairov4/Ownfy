// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Android
{
	using System.Collections.Generic;
	using Core;
	using global::Android.Content;
	using global::Android.Views;
	using global::Android.Widget;

	public class SongResultsAdapter : ArrayAdapter<Song>
	{
		private readonly IList<Song> list;
		
		public SongResultsAdapter(Context ctx, IList<Song> contents)
			: base(ctx, Resource.Layout.SongResultsAdapter, Resource.Id.ItemView, contents)
		{
			this.list = contents;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			if (convertView == null)
			{
				var li = (LayoutInflater)this.Context.GetSystemService(Context.LayoutInflaterService);
				convertView = li.Inflate(Resource.Layout.SongResultsAdapter, null);
			}

			var artist = convertView.FindViewById<TextView>(Resource.Id.ArtistText);
			var title = convertView.FindViewById<TextView>(Resource.Id.TitleText);
			var length = convertView.FindViewById<TextView>(Resource.Id.LengthText);

			artist.SetText(this.list[position].Artist, TextView.BufferType.Normal);
			title.SetText(this.list[position].Name, TextView.BufferType.Normal);
			length.SetText(this.list[position].Length.ToString("mm\\:ss"), TextView.BufferType.Normal);

			return convertView;
		}
	}
}