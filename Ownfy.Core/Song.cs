// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Core
{
	using System;
	using static CodeContracts;

	public class Song
	{
		public int Id { get; }

		public string RelativePath { get; }

		public string Name { get; }

		public string Artist { get; }

		public TimeSpan Length { get; }

		public DateTime LastModified { get; }

		public int FileLength { get; }

		public Song(int id, string name, string relativePath, string artist, TimeSpan length, DateTime lastModified, int fileLength)
		{
			RequiresNotNull(name);
			RequiresNotNull(relativePath);
			RequiresNotNull(artist);

			this.Id = id;
			this.Name = name;
			this.RelativePath = relativePath;
			this.Artist = artist;
			this.Length = length;
			this.LastModified = lastModified;
			this.FileLength = fileLength;
		}
	}
}