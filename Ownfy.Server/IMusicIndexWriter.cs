﻿// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Threading.Tasks;

	public interface IMusicIndexWriter
	{
		Task SaveSong(Song song);

		Task Commit();
	}
}