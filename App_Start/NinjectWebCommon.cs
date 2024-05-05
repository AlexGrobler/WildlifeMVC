using System;
using System.Web.Mvc;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc;
using WildlifeMVC.Models;
using WildlifeMVC.Services;

namespace YourProject
{
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