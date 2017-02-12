using Guanghui.BusinessServices.Implementations;
using Guanghui.BusinessServices.Interfaces;
using Microsoft.Practices.Unity;
using System.Web.Http;


namespace Guanghui.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            //container.RegisterType<IProductService, ProductService>();

            ////不好！因为webapi不能直接引用数据层，repository Model. 解决方法，MEF, create a new Resolver project
            //container.RegisterType<UnitOfWork>(new HierarchicalLifetimeManager()); //HierarchicalLifetimeManager maintains the lifetime of the object and child object depends upon parent object's lifetime


            RegisterTypes(container);

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            //component initialization via MEF
            Guanghui.Resolver.ComponentLoader.LoadContainer(container, ".\\bin", "Guanghui.WebApi.dll");
            Guanghui.Resolver.ComponentLoader.LoadContainer(container, ".\\bin", "Guanghui.BusinessServices.dll");
            Guanghui.Resolver.ComponentLoader.LoadContainer(container, ".\\bin", "Guanghui.Repository.dll");
        }
    }
}