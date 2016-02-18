// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Threading.Tasks;
	using Id3;
	using static CodeContracts;

	public class MusicIndexer
	{
		private readonly IMusicIndexWriter writer;

		public MusicIndexer(IMusicIndexWriter writer)
		{
			this.writer = writer;
		}

		public void IndexFolder(string path)
		{
			RequiresNotNull(path);

			Parallel.ForEach(this.EnumerateSongs(path), song =>
			{
				this.writer.SaveSong(song);
				Trace.WriteLine(song.RelativePath);
			});

			this.writer.Commit();
		}

		private IEnumerable<Song> EnumerateSongs(string path)
		{
			var musicFiles = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
			foreach (var musicFile in musicFiles)
			{
				using (var mp3 = new Mp3File(musicFile))
				{
					var title = string.Empty;
					var artist = string.Empty;
					var duration = TimeSpan.Zero;
					try
					{
						duration = mp3.Audio?.Duration ?? TimeSpan.Zero;
						var tag = mp3.GetTag(Id3TagFamily.FileStartTag);
						title = tag?.Title?.Value ?? string.Empty;
						artist = tag?.Artists?.Value ?? string.Empty;
					}
					catch (Exception)
					{
						Trace.WriteLine($"Error reading ID3 tag for: {musicFile}");
					}
					
					var songFileLen = (int)new FileInfo(musicFile).Length;
					var song = new Song(title, musicFile, artist, duration, File.GetLastWriteTime(musicFile),
						songFileLen);
					yield return song;
				}
			}
		}
	}
}