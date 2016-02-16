// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Autofac;
	using Lucene.Net.Analysis.Standard;
	using Lucene.Net.Index;
	using Lucene.Net.Store;
	using Nancy;
	using Nancy.Hosting.Self;
	using Directory = Lucene.Net.Store.Directory;
	using Version = Lucene.Net.Util.Version;

	internal class Program
	{
		private static void Main(string[] args)
		{
			using (var container = Bootstrap())
			{
				try
				{
					Trace.Listeners.Add(new ConsoleTraceListener());
					var nancyHost = new NancyHost(new NancyBootstrapper(container), new Uri(Settings.Default.BaseAddress));
					nancyHost.Start();
					Console.ReadKey();
					nancyHost.Stop();
				}
				catch (Exception e)
				{
					Trace.WriteLine("Unexpected fatal error, the server was shutdown: " + e.Message);
				}
			}
		}

		private static IContainer Bootstrap()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<LuceneMusicIndexWriter>().As<IMusicIndexWriter>();

			builder.RegisterType<MusicIndexer>();

			builder.RegisterType<LuceneMusicRepository>()
				.As<IMusicRepository>();

			builder.Register<Directory>(x => InitializeIndexDirectory(true))
				.As<Directory>();

			builder.RegisterType<OwnfyWeb>()
				.UsingConstructor(typeof (string), typeof (IMusicRepository))
				.WithParameter(new PositionalParameter(0, string.Empty))
				.As<INancyModule>()
				.As<OwnfyWeb>();

			return builder.Build();
		}

		private static Directory InitializeIndexDirectory(bool create)
		{
			const string luceneDir = "index";
			var directoryTemp = FSDirectory.Open(luceneDir);
			if (IndexWriter.IsLocked(directoryTemp)) IndexWriter.Unlock(directoryTemp);

			if (create)
			{
				var w = new IndexWriter(directoryTemp, new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);
				w.Commit();
				w.Dispose();
			}

			var lockFilePath = Path.Combine(luceneDir, "write.lock");
			if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
			return directoryTemp;
		}
	}
}