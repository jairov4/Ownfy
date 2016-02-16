// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Collections.Generic;
	using System.IO;
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

			foreach (var song in this.EnumerateSongs(path))
			{
				this.writer.SaveSong(song);
			}

			this.writer.Commit();
		}

		private IEnumerable<Song> EnumerateSongs(string path)
		{
			var musicFiles = Directory.GetFiles(path, "*.mp3");
			foreach (var musicFile in musicFiles)
			{
				using (var mp3 = new Mp3File(musicFile))
				{
					var tag = mp3.GetTag(Id3TagFamily.FileStartTag);
					var songFileLen = (int)new FileInfo(musicFile).Length;
					var song = new Song(tag.Title.Value, tag.AudioFileUrl.Url, tag.Artists.Value, mp3.Audio.Duration, File.GetLastWriteTime(musicFile),
						songFileLen);
					yield return song;
				}
			}
		}
	}
}