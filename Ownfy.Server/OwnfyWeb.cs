// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using Nancy;

	public class OwnfyWeb : NancyModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OwnfyWeb"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public OwnfyWeb(string path) : base(path)
		{
			CodeContracts.RequiresNotNull(path);
			this.Get["/stream/{id}"] = _ => new Response();
		}
	}
}