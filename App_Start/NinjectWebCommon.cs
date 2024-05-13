using System;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.Services;

namespace YourProject
{
    //Ninject is a library which enables dependency injection for old ASP .NET framwork versions, DI is native to ASP.NET Core
    //DI and service layers are considered the standard approach to handling db and API calls
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start()
        {
            bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            kernel.Bind<wildlife_DBEntities>().ToSelf().InRequestScope();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
            return kernel;
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ISpeciesService>().To<SpeciesService>();
            kernel.Bind<ISightingService>().To<SightingService>();

        }
    }
}