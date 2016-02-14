// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IMusicRepository
	{
		Task<IReadOnlyList<Song>> SearchSongsByArtist(string artist, string searchText);

		Task<IReadOnlyList<Song>> SearchSong(string searchText);
	}
}