// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Autofac;
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

			builder.RegisterType<MusicRepository>()
				.As<IMusicRepository>();

			builder.Register<Directory>(x => FSDirectory.Open(new DirectoryInfo("index")))
				.As<Directory>();

			builder.RegisterType<OwnfyWeb>()
				.UsingConstructor(typeof(string), typeof(IMusicRepository))
				.WithParameter(new PositionalParameter(0, string.Empty))
				.As<INancyModule>();

			return builder.Build();
		}
	}
}