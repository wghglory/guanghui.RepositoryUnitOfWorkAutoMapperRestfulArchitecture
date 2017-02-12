using Guanghui.Resolver;
using System.ComponentModel.Composition;
using Guanghui.Repository.UnitOfWork;


namespace Guanghui.Repository.Infrastructure
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IUnitOfWork, Guanghui.Repository.UnitOfWork.UnitOfWork>();
        }
    }
}
