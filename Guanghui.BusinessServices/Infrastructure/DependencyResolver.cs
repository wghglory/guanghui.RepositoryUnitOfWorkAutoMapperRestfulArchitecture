using Guanghui.BusinessServices.Implementations;
using Guanghui.BusinessServices.Interfaces;
using Guanghui.Resolver;
using System.ComponentModel.Composition;

namespace Guanghui.BusinessServices.Infrastructure
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IProductService, ProductService>();
            registerComponent.RegisterType<IUserService, UserService>();
            registerComponent.RegisterType<ITokenService, TokenService>();


        }
    }
}
