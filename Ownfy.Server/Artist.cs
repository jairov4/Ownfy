// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.Collections.Generic;

	public class Artist
	{
		public ICollection<string> Names { get; }

		public Artist(ICollection<string> names)
		{
			this.Names = names;
		}
	}
}