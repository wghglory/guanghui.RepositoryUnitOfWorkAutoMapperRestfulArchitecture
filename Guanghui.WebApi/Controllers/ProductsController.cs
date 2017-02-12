using Guanghui.BusinessEntities;
using Guanghui.BusinessServices.Interfaces;
using Guanghui.WebApi.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Guanghui.WebApi.Controllers
{

    /*
        1. 如果没有先authenticateController，不会产生token，authorization一定失败。
        2. 如果authenticate用户密码正确通过，响应头包含刚才产生的token，数据库也有。在通过authentication情况下，
        请求ProductController必须把正确的token带到请求头，和数据库比对正确才能通过。如果没有token或者错误，失败。
        3. 如果两步都通过，token过期时间可以延长15min，只要用户在15min内用这个token可以访问其他service
            
    */

    //[EnableCors(origins: "*", headers: "*", methods: "get,post,put,delete")]
    [ActionFilters.AuthorizationRequired]
    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;

        #region Public Constructor

        public ProductsController(IProductService prodService)
        {
            //_productService = new ProductService();
            _productService = prodService;   //untiy constructor injection
        }

        #endregion

        // GET api/product
        public HttpResponseMessage Get()
        {
            var products = _productService.GetAllProducts();
            var productEntities = products as List<ProductEntity> ?? products.ToList();
            if (productEntities.Any())
                return Request.CreateResponse(HttpStatusCode.OK, productEntities);
            throw new DataException(1000, "Products not found", HttpStatusCode.NotFound);
        }

        // GET api/product/5
        public HttpResponseMessage Get(int id)
        {
            if (id > 0)
            {
                var product = _productService.GetProductById(id);
                if (product != null)
                    return Request.CreateResponse(HttpStatusCode.OK, product);

                throw new DataException(1001, "No product found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        // POST api/product
        public int Post([FromBody] ProductEntity productEntity)
        {
            return _productService.CreateProduct(productEntity);
        }

        // PUT api/product/5
        public bool Put(int id, [FromBody]ProductEntity productEntity)
        {
            if (id > 0)
            {
                return _productService.UpdateProduct(id, productEntity);
            }
            return false;
        }

        // DELETE api/product/5
        public bool Delete(int id)
        {
            if (id > 0)
            {
                var isSuccess = _productService.DeleteProduct(id);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new DataException(1002, "Product is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}
