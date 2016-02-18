// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using Nancy;
	using Newtonsoft.Json;
	using static CodeContracts;

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
				return new FlushingStreamResponse(stream, "audio/mpeg3");
			};

			this.Get["/artist/{artist}", true] = async (parameters, ct) =>
			{
				var artistContents = await repository.SearchSongsByArtist((string)parameters.artist, string.Empty);
				return artistContents;
			};

			this.Get["/search/{searchText}", true] = async (parameters, ct) =>
			{
				var results = await repository.SearchSong((string)parameters.searchText);
				return JsonConvert.SerializeObject(results);
			};
		}
	}
}