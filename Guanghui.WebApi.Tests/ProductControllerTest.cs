using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Hosting;
using Guanghui.BusinessEntities;
using Guanghui.BusinessServices.Implementations;
using Guanghui.BusinessServices.Interfaces;
using Guanghui.Repository;
using Guanghui.Repository.GenericRepository;
using Guanghui.Repository.UnitOfWork;
using Guanghui.TestHelper;
using Guanghui.WebApi.Controllers;
using Guanghui.WebApi.ExceptionHandling;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;


namespace Guanghui.WebApi.Tests
{
    [TestFixture]
    public class ProductControllerTest
    {

        #region Variables

        private IProductService _productService;
        private ITokenService _tokenService;
        private IUnitOfWork _unitOfWork;
        private List<Product> _products;
        private List<Token> _tokens;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<Token> _tokenRepository;
        private WebApiDbContext _dbContext;
        private HttpClient _client;

        private HttpResponseMessage _response;
        private string _token;
        private const string ServiceBaseUrl = "http://localhost:26042/";

        #endregion

        #region Test fixture setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _products = SetUpProducts();
            _tokens = SetUpTokens();

            _dbContext = new Mock<WebApiDbContext>().Object;

            _tokenRepository = SetUpTokenRepository();
            _productRepository = SetUpProductRepository();

            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ProductRepository).Returns(_productRepository);
            unitOfWork.SetupGet(s => s.TokenRepository).Returns(_tokenRepository);
            _unitOfWork = unitOfWork.Object;

            _productService = new ProductService(_unitOfWork);
            _tokenService = new TokenService(_unitOfWork);

            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseUrl) };

            var tokenEntity = _tokenService.GenerateToken(1);
            _token = tokenEntity.AuthToken;

            _client.DefaultRequestHeaders.Add("Token", _token);
        }

        #endregion

        #region Setup
        /// <summary>
        /// Re-initializes test.
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseUrl) };
            _client.DefaultRequestHeaders.Add("Token", _token);
        }
        #endregion

        #region Private member methods

        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Product> SetUpProductRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Product>>(MockBehavior.Default, _dbContext);

            // Setup mocking behavior
            mockRepo.Setup(p => p.Get()).Returns(_products);

            mockRepo.Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(new Func<int, Product>(
                             id => _products.Find(p => p.ProductId.Equals(id))));

            mockRepo.Setup(p => p.Add((It.IsAny<Product>())))
                .Callback(new Action<Product>(newProduct =>
                {
                    dynamic maxProductId = _products.Last().ProductId;
                    dynamic nextProductId = maxProductId + 1;
                    newProduct.ProductId = nextProductId;
                    _products.Add(newProduct);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Product>()))
                .Callback(new Action<Product>(prod =>
                {
                    var oldProduct = _products.Find(a => a.ProductId == prod.ProductId);
                    oldProduct = prod;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Product>()))
                .Callback(new Action<Product>(prod =>
                {
                    var productToRemove =
                        _products.Find(a => a.ProductId == prod.ProductId);

                    if (productToRemove != null)
                        _products.Remove(productToRemove);
                }));

            // Return mock implementation object
            return mockRepo.Object;
        }

        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Token> SetUpTokenRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Token>>(MockBehavior.Default, _dbContext);

            // Setup mocking behavior
            mockRepo.Setup(p => p.Get()).Returns(_tokens);

            mockRepo.Setup(p => p.GetById(It.IsAny<int>()))
                .Returns(new Func<int, Token>(
                             id => _tokens.Find(p => p.TokenId.Equals(id))));

            mockRepo.Setup(p => p.Add((It.IsAny<Token>())))
                .Callback(new Action<Token>(newToken =>
                {
                    dynamic maxTokenId = _tokens.Last().TokenId;
                    dynamic nextTokenId = maxTokenId + 1;
                    newToken.TokenId = nextTokenId;
                    _tokens.Add(newToken);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Token>()))
                .Callback(new Action<Token>(token =>
                {
                    var oldToken = _tokens.Find(a => a.TokenId == token.TokenId);
                    oldToken = token;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Token>()))
                .Callback(new Action<Token>(prod =>
                {
                    var tokenToRemove =
                        _tokens.Find(a => a.TokenId == prod.TokenId);

                    if (tokenToRemove != null)
                        _tokens.Remove(tokenToRemove);
                }));

            // Return mock implementation object
            return mockRepo.Object;
        }

        /// <summary>
        /// Setup dummy products data
        /// </summary>
        /// <returns></returns>
        private static List<Product> SetUpProducts()
        {
            var prodId = new int();
            var products = DataInitializer.GetAllProducts();
            foreach (Product prod in products)
                prod.ProductId = ++prodId;
            return products;
        }

        /// <summary>
        /// Setup dummy tokens data
        /// </summary>
        /// <returns></returns>
        private static List<Token> SetUpTokens()
        {
            var tokId = new int();
            var tokens = DataInitializer.GetAllTokens();
            foreach (Token tok in tokens)
                tok.TokenId = ++tokId;
            return tokens;
        }

        #endregion

        #region Unit Tests

        /// <summary>
        /// Get all products test
        /// </summary>
        [Test]
        public void GetAllProductsTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = productController.Get();

            var responseResult = JsonConvert.DeserializeObject<List<Product>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
            var comparer = new ProductComparer();
            CollectionAssert.AreEqual(
                responseResult.OrderBy(product => product, comparer),
                _products.OrderBy(product => product, comparer), comparer);
        }

        /// <summary>
        /// Get product by Id
        /// </summary>
        [Test]
        public void GetProductByIdTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/2")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = productController.Get(2);

            var responseResult = JsonConvert.DeserializeObject<Product>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            AssertObjects.PropertyValuesAreEquals(responseResult,
                                                    _products.Find(a => a.ProductName.Contains("Mobile")));
        }

        /// <summary>
        /// Get product by wrong Id
        /// </summary>
        [Test]
        //[ExpectedException("WebApi.ErrorHelper.DataException")]
        public void GetProductByWrongIdTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/10")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var ex = Assert.Throws<DataException>(() => productController.Get(10));
            Assert.That(ex.ErrorCode, Is.EqualTo(1001));
            Assert.That(ex.ErrorDescription, Is.EqualTo("No product found for this id."));

        }

        /// <summary>
        /// Get product by invalid Id
        /// </summary>
        [Test]
        // [ExpectedException("WebApi.ErrorHelper.ApiException")]
        public void GetProductByInvalidIdTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/-1")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var ex = Assert.Throws<ApiException>(() => productController.Get(-1));
            Assert.That(ex.ErrorCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(ex.ErrorDescription, Is.EqualTo("Bad Request..."));
        }

        /// <summary>
        /// Create product test
        /// </summary>
        [Test]
        public void CreateProductTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var newProduct = new ProductEntity()
            {
                ProductName = "Android Phone"
            };

            var maxProductIdBeforeAdd = _products.Max(a => a.ProductId);
            newProduct.ProductId = maxProductIdBeforeAdd + 1;
            productController.Post(newProduct);
            var addedproduct = new Product() { ProductName = newProduct.ProductName, ProductId = newProduct.ProductId };
            AssertObjects.PropertyValuesAreEquals(addedproduct, _products.Last());
            Assert.That(maxProductIdBeforeAdd + 1, Is.EqualTo(_products.Last().ProductId));
        }

        /// <summary>
        /// Update product test
        /// </summary>
        [Test]
        public void UpdateProductTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var firstProduct = _products.First();
            firstProduct.ProductName = "Laptop updated";
            var updatedProduct = new ProductEntity() { ProductName = firstProduct.ProductName, ProductId = firstProduct.ProductId };
            productController.Put(firstProduct.ProductId, updatedProduct);
            Assert.That(firstProduct.ProductId, Is.EqualTo(1)); // hasn't changed
        }

        /// <summary>
        /// Delete product test
        /// </summary>
        [Test]
        public void DeleteProductTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            int maxId = _products.Max(a => a.ProductId); // Before removal
            var lastProduct = _products.Last();

            // Remove last Product
            productController.Delete(lastProduct.ProductId);
            Assert.That(maxId, Is.GreaterThan(_products.Max(a => a.ProductId))); // Max id reduced by 1
        }

        /// <summary>
        /// Delete product test with invalid id
        /// </summary>
        [Test]
        public void DeleteProductInvalidIdTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var ex = Assert.Throws<ApiException>(() => productController.Delete(-1));
            Assert.That(ex.ErrorCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(ex.ErrorDescription, Is.EqualTo("Bad Request..."));
        }

        /// <summary>
        /// Delete product test with wrong id
        /// </summary>
        [Test]
        public void DeleteProductWrongIdTest()
        {
            var productController = new ProductsController(_productService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseUrl + "api/Products/")
                }
            };
            productController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            int maxId = _products.Max(a => a.ProductId); // Before removal

            var ex = Assert.Throws<DataException>(() => productController.Delete(maxId + 1));
            Assert.That(ex.ErrorCode, Is.EqualTo(1002));
            Assert.That(ex.ErrorDescription, Is.EqualTo("Product is already deleted or not exist in system."));
        }

        #endregion

        #region Integration Test

        /// <summary>
        /// Get all products test
        /// </summary>
        [Test]
        public void GetAllProductsIntegrationTest()
        {
            #region To be written inside Setup method specifically for integration tests
            var client = new HttpClient { BaseAddress = new Uri(ServiceBaseUrl) };
            client.DefaultRequestHeaders.Add("Authorization", "Basic d2doZ2xvcnk6MTIz");  //wghglory:123 encode base64
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            _response = client.PostAsync("api/Authenticate/", null).Result;   //response header should contain Token

            if (_response != null && _response.Headers != null && _response.Headers.Contains("Token") && _response.Headers.GetValues("Token") != null)
            {
                client.DefaultRequestHeaders.Clear();
                _token = ((string[])(_response.Headers.GetValues("Token")))[0];
                client.DefaultRequestHeaders.Add("Token", _token);
            }
            #endregion

            //client made a new request with Token
            _response = client.GetAsync("api/Products/").Result;
            var responseResult =
                JsonConvert.DeserializeObject<List<ProductEntity>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
        }

        #endregion

        #region Tear Down
        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }

        #endregion

        #region TestFixture TearDown.

        /// <summary>
        /// TestFixture teardown
        /// </summary>
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _tokenService = null;
            _productService = null;
            _unitOfWork = null;
            _tokenRepository = null;
            _productRepository = null;
            _tokens = null;
            _products = null;
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }

        #endregion
    }
}
