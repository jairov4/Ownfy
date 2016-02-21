// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Globalization;
	using System.Text.RegularExpressions;
	using Nancy;
	using Newtonsoft.Json;
	using static Core.CodeContracts;

	public class OwnfyWeb : NancyModule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OwnfyWeb"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="repository">The music repository</param>
		public OwnfyWeb(string path, IMusicRepository repository) : base(path)
		{
			RequiresNotNull(path);

			this.Get["/stream/{id:int}"] = parameters =>
			{
				var stream = repository.GetSongStream((int)parameters.id);
				var res = new FlushingStreamResponse(stream, "audio/mpeg3");

				var len = stream.Length;
				foreach (var s in this.Request.Headers["Range"])
				{
					var start = s.Split('=')[1];
					var m = Regex.Match(start, @"(\d+)-(\d+)?");
					start = m.Groups[1].Value;
					var end = len - 1;
					if (!string.IsNullOrWhiteSpace(m.Groups[2].Value))
					{
						end = Convert.ToInt64(m.Groups[2].Value);
					}

					var startI = Convert.ToInt64(start);
					var length = len - startI;
					res.WithHeader("content-range", "bytes " + start + "-" + end + "/" + len);
					res.WithHeader("content-length", length.ToString(CultureInfo.InvariantCulture));
					res.StatusCode = HttpStatusCode.PartialContent;
				}

				return res;
			};

			this.Get["/artist/{artist}", true] = async (parameters, ct) =>
			{
				var artistContents = await repository.SearchSongsByArtist((string)parameters.artist, string.Empty);
				return artistContents;
			};

			this.Get["/search/{searchText}", true] = async (parameters, ct) =>
			{
				var results = await repository.SearchSong((string)parameters.searchText);
				return results;
			};
		}
	}
}