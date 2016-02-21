// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Threading.Tasks;
	using Core;

	public interface IMusicIndexWriter
	{
		void SaveSong(Song song);

		void Commit();
	}
}