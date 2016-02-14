// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Collections.Generic;
	using System.IO;
	using Id3;

	public class MusicIndexer
	{
		public IEnumerable<Song> IndexFolder(string path)
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