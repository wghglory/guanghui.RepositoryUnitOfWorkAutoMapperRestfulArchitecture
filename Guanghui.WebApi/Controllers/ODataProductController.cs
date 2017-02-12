//using Guanghui.BusinessEntities;
//using Guanghui.BusinessServices.Interfaces;
//using Guanghui.WebApi.ExceptionHandling;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.OData;

//namespace Guanghui.WebApi.Controllers
//{

    
//    public class ODataProductController : ODataController
//    {
//        private readonly IProductService _productService;

//        #region Public Constructor

//        public ODataProductController(IProductService prodService)
//        {
//            //_productService = new ProductService();
//            _productService = prodService;   //untiy constructor injection
//        }

//        #endregion

//        // GET api/product
//        [EnableQuery]
//        public HttpResponseMessage Get()
//        {
//            var products = _productService.GetAllProducts();
//            var productEntities = products as List<ProductEntity> ?? products.ToList();
//            if (productEntities.Any())
//                return Request.CreateResponse(HttpStatusCode.OK, productEntities);
//            throw new DataException(1000, "Products not found", HttpStatusCode.NotFound);
//        }

//        // GET api/product/5
//        [EnableQuery]
//        public HttpResponseMessage Get(int id)
//        {
//            if (id > 0)
//            {
//                var product = _productService.GetProductById(id);
//                if (product != null)
//                    return Request.CreateResponse(HttpStatusCode.OK, product);

//                throw new DataException(1001, "No product found for this id.", HttpStatusCode.NotFound);
//            }
//            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
//        }

//        // POST api/product
//        public int Post([FromBody] ProductEntity productEntity)
//        {
//            return _productService.CreateProduct(productEntity);
//        }

//        // PUT api/product/5
//        public bool Put(int id, [FromBody]ProductEntity productEntity)
//        {
//            if (id > 0)
//            {
//                return _productService.UpdateProduct(id, productEntity);
//            }
//            return false;
//        }

//        // DELETE api/product/5
//        public bool Delete(int id)
//        {
//            if (id > 0)
//            {
//                var isSuccess = _productService.DeleteProduct(id);
//                return isSuccess;
//                throw new DataException(1002, "Product is already deleted or not exist in system.", HttpStatusCode.NoContent);
//            }
//            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
//        }
//    }
//}
