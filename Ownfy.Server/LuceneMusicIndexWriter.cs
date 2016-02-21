// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Threading.Tasks;
	using Core;
	using Lucene.Net.Analysis.Standard;
	using Lucene.Net.Documents;
	using Lucene.Net.Index;
	using Lucene.Net.Store;
	using Lucene.Net.Util;
	using static System.Threading.Tasks.Task;
	using static Core.CodeContracts;

	public class LuceneMusicIndexWriter : IMusicIndexWriter
	{
		private readonly LuceneDocumentMapper mapper = new LuceneDocumentMapper();

		private readonly IndexWriter writer;

		public LuceneMusicIndexWriter(Directory luceneIndexDirectory)
		{
			RequiresNotNull(luceneIndexDirectory);
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);
			this.writer = new IndexWriter(luceneIndexDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
		}

		public void SaveSong(Song song)
		{
			RequiresNotNull(song);
			var doc = new Document();
			this.mapper.FillDocument(song, doc);
			this.writer.AddDocument(doc);
		}

		public void Commit()
		{
			this.writer.Commit();
			this.writer.Optimize();
		}

		public async Task Close()
		{
			await Run(() => this.writer.Dispose());
		}
	}
}