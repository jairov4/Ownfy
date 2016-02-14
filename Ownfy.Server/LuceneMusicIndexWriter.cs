// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Threading.Tasks;
	using Lucene.Net.Analysis.Standard;
	using Lucene.Net.Documents;
	using Lucene.Net.Index;
	using Lucene.Net.Store;
	using Lucene.Net.Util;
	using static System.Threading.Tasks.Task;
	using static CodeContracts;

	public class LuceneMusicIndexWriter : IMusicIndexWriter
	{
		private readonly IndexWriter writer;

		public LuceneMusicIndexWriter(Directory luceneIndexDirectory)
		{
			RequiresNotNull(luceneIndexDirectory);
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);
			this.writer = new IndexWriter(luceneIndexDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
		}

		public async Task SaveSong(Song song)
		{
			RequiresNotNull(song);
			var doc = new Document();
			FillDocument(song, doc);
			await Run(() => this.writer.AddDocument(doc));
		}

		public async Task Commit()
		{
			await Run(() => this.writer.Optimize());
			await Run(() => this.writer.Commit());
		}

		public async Task Close()
		{
			await Run(() => this.writer.Dispose());
		}

		private static void FillDocument(Song song, Document doc)
		{
			doc.Add(new Field(nameof(song.RelativePath), song.RelativePath, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field(nameof(song.Name), song.Name, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field(nameof(song.Artist), song.Artist, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new NumericField(nameof(song.FileLength), song.FileLength, Field.Store.YES, false));
			doc.Add(new NumericField(nameof(song.Length), (int)song.Length.TotalMilliseconds, Field.Store.YES, false));
			doc.Add(new Field(nameof(song.LastModified), DateTools.DateToString(song.LastModified, DateTools.Resolution.SECOND), Field.Store.YES,
				Field.Index.NOT_ANALYZED));
		}
	}
}