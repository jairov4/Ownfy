// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System.IO;
	using Nancy;

	public class FlushingStreamResponse : Response
	{
		public FlushingStreamResponse(Stream sourceStream, string mimeType, int bufferSize = 16*1024)
		{
			this.Contents = stream =>
			{
				var buffer = new byte[bufferSize];
				int read;
				while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					stream.Write(buffer, 0, read);
					stream.Flush();
				}
				sourceStream.Dispose();
			};

			this.StatusCode = HttpStatusCode.OK;
			this.ContentType = mimeType;
		}
	}
}