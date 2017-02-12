using Guanghui.BusinessEntities;
using System.Collections.Generic;


namespace Guanghui.BusinessServices.Interfaces
{
    /// <summary>
    /// Product Service Contract
    /// </summary>
    public interface IProductService
    {
        ProductEntity GetProductById(int productId);
        IEnumerable<ProductEntity> GetAllProducts();
        int CreateProduct(ProductEntity productEntity);
        bool UpdateProduct(int productId,ProductEntity productEntity);
        bool DeleteProduct(int productId);
    }
}
