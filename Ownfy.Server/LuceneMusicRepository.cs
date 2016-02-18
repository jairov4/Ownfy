﻿// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Lucene.Net.Analysis.Standard;
	using Lucene.Net.Documents;
	using Lucene.Net.QueryParsers;
	using Lucene.Net.Search;
	using static System.Threading.Tasks.Task;
	using Directory = Lucene.Net.Store.Directory;
	using Version = Lucene.Net.Util.Version;

	public class LuceneMusicRepository : IMusicRepository
	{
		private readonly StandardAnalyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
		private readonly LuceneDocumentMapper mapper = new LuceneDocumentMapper();
		private readonly IndexSearcher searcher;

		public LuceneMusicRepository(Directory directory)
		{
			this.searcher = new IndexSearcher(directory, true);
		}

		public async Task<IReadOnlyList<Song>> SearchSongsByArtist(string artist, string searchText)
		{
			var hits_limit = 1000;
			var queries = new[] { QueryParser.Escape(artist), searchText };
			var fields = new[] { nameof(Song.Artist), nameof(Song.Name) };
			var query = MultiFieldQueryParser.Parse(Version.LUCENE_30, queries, fields, this.analyzer);
			var hits = await Run(() => this.searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs);
			var docs = hits.Select(x => Tuple.Create(x.Doc, this.searcher.Doc(x.Doc)));
			var results = this.mapper.GetSongs(docs).ToList();
			return results;
		}

		public async Task<IReadOnlyList<Song>> SearchSong(string searchText)
		{
			var hitsLimit = 1000;
			var fields = new[] { nameof(Song.Artist), nameof(Song.Name) };
			var parser = new MultiFieldQueryParser(Version.LUCENE_30, fields, this.analyzer);
			var query = parser.Parse(searchText);
			var hits = await Run(() => this.searcher.Search(query, null, hitsLimit, Sort.RELEVANCE).ScoreDocs);
			var docs = hits.Select(x => Tuple.Create(x.Doc, this.searcher.Doc(x.Doc)));
			var results = this.mapper.GetSongs(docs).ToList();
			return results;
		}

		public Stream GetSongStream(int id)
		{
			var doc = this.searcher.Doc(id);
			var song = this.mapper.GetSongs(new[] { Tuple.Create(id, doc) }).First();
			return File.OpenRead(song.RelativePath);
		}

		public void Close()
		{
			this.analyzer.Close();
			this.searcher.Dispose();
		}
	}
}