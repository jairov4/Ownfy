// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Diagnostics;
	using Autofac;
	using Nancy.Hosting.Self;

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
			return builder.Build();
		}
	}
}