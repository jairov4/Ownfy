// <copyright company="Skivent Ltda.">
// Copyright (c) 2013, All Right Reserved, http://www.skivent.com.co/
// </copyright>

namespace Ownfy.Server
{
	using System;
	using System.Collections.Generic;
	using Autofac;
	using Nancy;
	using Nancy.Bootstrapper;
	using Nancy.Bootstrappers.Autofac;
	using static CodeContracts;

	/// <summary>
	/// Bootstrapper that adapts Nancy to use a prepared <see cref="Autofac"/> container
	/// </summary>
	public class NancyBootstrapper : AutofacNancyBootstrapper
	{
		private readonly ILifetimeScope rootContainer;

		/// <summary>
		/// Initializes a new instance of the <see cref="NancyBootstrapper"/> class.
		/// </summary>
		/// <param name="rootContainer">The root container.</param>
		public NancyBootstrapper(IContainer rootContainer)
		{
			RequiresNotNull(rootContainer);
			// Overrides the default container creation
			this.rootContainer = rootContainer;
		}

		/// <summary>
		/// Gets the application container.
		/// </summary>
		/// <returns></returns>
		protected override ILifetimeScope GetApplicationContainer()
		{
			return this.rootContainer;
		}

		protected override void RegisterRequestContainerModules(ILifetimeScope container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
		{
			// Overrides the automatic module registration, our registration is done in app bootstrap
		}

		protected override INancyModule GetModule(ILifetimeScope container, Type moduleType)
		{
			RequiresNotNull(container);
			RequiresNotNull(moduleType);
			object module;
			return container.TryResolve(moduleType, out module) ? (INancyModule)module : null;
		}
	}
}