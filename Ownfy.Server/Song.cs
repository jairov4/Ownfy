// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;

	public class Song
	{
		public string RelativePath { get; }

		public string Name { get; }

		public string Artist { get; }

		public TimeSpan Length { get; }

		public DateTime LastModified { get; }

		public int FileLength { get; }

		public Song(string name, string relativePath, string artist, TimeSpan length, DateTime lastModified, int fileLength)
		{
			this.Name = name;
			this.RelativePath = relativePath;
			this.Artist = artist;
			this.Length = length;
			this.LastModified = lastModified;
			this.FileLength = fileLength;
		}
	}
}