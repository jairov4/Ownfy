// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Collections.Generic;
	using Lucene.Net.Documents;

	public class LuceneDocumentMapper
	{
		public void FillDocument(Song song, Document doc)
		{
			doc.Add(new Field(nameof(song.RelativePath), song.RelativePath, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field(nameof(song.Name), song.Name, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field(nameof(song.Artist), song.Artist, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new NumericField(nameof(song.FileLength), song.FileLength, Field.Store.YES, false));
			doc.Add(new NumericField(nameof(song.Length), (int)song.Length.TotalMilliseconds, Field.Store.YES, false));
			doc.Add(new Field(nameof(song.LastModified), DateTools.DateToString(song.LastModified, DateTools.Resolution.SECOND), Field.Store.YES,
				Field.Index.NOT_ANALYZED));
		}

		public IEnumerable<Song> GetSongs(IEnumerable<Document> documents)
		{
			foreach (var document in documents)
			{
				var name = document.Get(nameof(Song.Name));
				var relativePath = document.Get(nameof(Song.RelativePath));
				var artist = document.Get(nameof(Song.Artist));
				var fileLength = (NumericField)document.GetFieldable(nameof(Song.FileLength));
				var length = (NumericField)document.GetFieldable(nameof(Song.Length));
				var lastModified = DateTools.StringToDate(document.Get(nameof(Song.LastModified)));
				var song = new Song(
					name,
					relativePath,
					artist,
					TimeSpan.FromMilliseconds((int)length.NumericValue), 
					lastModified, 
					(int)fileLength.NumericValue);
				yield return song;
			}
		}
	}
}