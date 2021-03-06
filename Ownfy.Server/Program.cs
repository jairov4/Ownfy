﻿// <copyright company="Skivent Ltda.">
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
	
	internal class Program
	{
		private static void Main(string[] args)
		{
			using (var container = Bootstrap())
			{
#if !DEBUG
				try
#endif
				{
					Trace.Listeners.Add(new ConsoleTraceListener());

					if (args.Length > 0 && args[0] == "--build-index")
					{
						var indexer = container.Resolve<MusicIndexer>();
						indexer.IndexFolder(Settings.Default.LibraryFolder);
						return;
					}

					using (var nancyHost = new NancyHost(new NancyBootstrapper(container), new Uri(Settings.Default.BaseAddress)))
					{
						nancyHost.Start();
						Console.ReadKey();
						nancyHost.Stop();
					}
				}
#if !DEBUG
				catch (Exception e)
				{
					Trace.WriteLine("Unexpected fatal error, the server was shutdown: " + e.Message);
				}
#endif
			}
		}

		private static IContainer Bootstrap()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<LuceneMusicIndexWriter>().As<IMusicIndexWriter>();

			builder.RegisterType<MusicIndexer>();

			builder.RegisterType<LuceneMusicRepository>()
				.As<IMusicRepository>();

			builder.Register<Directory>(x => InitializeIndexDirectory())
				.As<Directory>();

			builder.RegisterType<OwnfyWeb>()
				.UsingConstructor(typeof (string), typeof (IMusicRepository))
				.WithParameter(new PositionalParameter(0, string.Empty))
				.As<INancyModule>()
				.As<OwnfyWeb>();

			return builder.Build();
		}

		private static Directory InitializeIndexDirectory()
		{
			const string luceneDir = "index";
			var directoryTemp = FSDirectory.Open(luceneDir);
			if (IndexWriter.IsLocked(directoryTemp)) IndexWriter.Unlock(directoryTemp);
			
			var lockFilePath = Path.Combine(luceneDir, "write.lock");
			if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
			return directoryTemp;
		}
	}
}