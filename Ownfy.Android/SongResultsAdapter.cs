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
		public SongResultsAdapter(Context ctx, IList<Song> contents) : base(ctx, Resource.Layout.SongResultsAdapter, Resource.Id.ItemView, contents)
		{
		}
		
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			return base.GetView(position, convertView, parent);
		}
	}
}