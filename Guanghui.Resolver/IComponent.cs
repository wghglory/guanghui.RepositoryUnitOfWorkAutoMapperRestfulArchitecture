using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guanghui.Resolver
{
    /// <summary>
    /// register underlying types with Unity
    /// </summary>
    public interface IComponent
    {
        void SetUp(IRegisterComponent registerComponent);
    }
}
