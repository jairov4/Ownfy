// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Lucene.Net.Documents;

	public class LuceneDocumentMapper
	{
		public void FillDocument(Song song, Document doc)
		{
			if (!string.IsNullOrWhiteSpace(song.RelativePath))
				doc.Add(new Field(nameof(song.RelativePath), song.RelativePath, Field.Store.YES, Field.Index.ANALYZED));

			if (!string.IsNullOrWhiteSpace(song.Name))
				doc.Add(new Field(nameof(song.Name), song.Name, Field.Store.YES, Field.Index.ANALYZED));

			if (!string.IsNullOrWhiteSpace(song.Artist))
				doc.Add(new Field(nameof(song.Artist), song.Artist, Field.Store.YES, Field.Index.ANALYZED));

			var fieldFileLenght = new NumericField(nameof(song.FileLength), Field.Store.YES, false);
			fieldFileLenght.SetIntValue(song.FileLength);
			doc.Add(fieldFileLenght);

			var fieldLenght = new NumericField(nameof(song.Length), Field.Store.YES, false);
			fieldLenght.SetIntValue((int)song.Length.TotalMilliseconds);
			doc.Add(fieldLenght);

			doc.Add(new Field(nameof(song.LastModified), DateTools.DateToString(song.LastModified, DateTools.Resolution.SECOND), Field.Store.YES,
				Field.Index.NOT_ANALYZED));
		}

		public IEnumerable<Song> GetSongs(IEnumerable<Tuple<int, Document>> documents)
		{
			foreach (var pair in documents)
			{
				var document = pair.Item2;
				var name = document.Get(nameof(Song.Name)) ?? string.Empty;
				var relativePath = document.Get(nameof(Song.RelativePath)) ?? string.Empty;
				var artist = document.Get(nameof(Song.Artist)) ?? string.Empty;
				var fileLength = int.Parse(document.Get(nameof(Song.FileLength)) ?? "0");
				var length = int.Parse(document.Get(nameof(Song.Length)) ?? "0");
				var lastModified = DateTools.StringToDate(document.Get(nameof(Song.LastModified)));

				var song = new Song(
					pair.Item1,
					name,
					relativePath,
					artist,
					TimeSpan.FromMilliseconds(length),
					lastModified,
					fileLength);
				yield return song;
			}
		}
	}
}