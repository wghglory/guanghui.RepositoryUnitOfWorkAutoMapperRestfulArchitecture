using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Guanghui.BusinessServices.Interfaces;
using Guanghui.Repository;
using Guanghui.Repository.UnitOfWork;
using Guanghui.Repository.GenericRepository;
using Guanghui.BusinessServices.Implementations;
using Guanghui.BusinessEntities;
using Guanghui.TestHelper;

namespace Guanghui.BusinessServices.Tests
{
    /// <summary>
    /// Product Service Test
    /// </summary>
    public class ProductServiceTest
    {
        #region Variables

        private IProductService _productService;
        private IUnitOfWork _unitOfWork;
        private List<Product> _products;
        private GenericRepository<Product> _productRepository;
        private WebApiDbContext _dbContext;
        #endregion

        #region Test fixture setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _products = SetUpProducts();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Re-initializes test. 每个不同test执行前触发，使得不同test独立互不影响
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _products = SetUpProducts();
            _dbContext = new Mock<WebApiDbContext>().Object;
            _productRepository = SetUpProductRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ProductRepository).Returns(_productRepository);
            _unitOfWork = unitOfWork.Object;
            _productService = new ProductService(_unitOfWork);

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
        /// Setup dummy products data
        /// </summary>
        /// <returns></returns>
        private static List<Product> SetUpProducts()
        {
            int prodId = 0;
            var products = DataInitializer.GetAllProducts();
            foreach (Product prod in products)
                prod.ProductId = ++prodId;
            return products;

        }

        #endregion

        #region Unit Tests

        /// <summary>
        /// Service should return all the products
        /// </summary>
        [Test]
        public void GetAllProductsTest()
        {
            var productEntities = _productService.GetAllProducts();
            if (productEntities != null)
            {
                var productList =
                    productEntities.Select(
                        productEntity =>
                        new Product { ProductId = productEntity.ProductId, ProductName = productEntity.ProductName }).
                        ToList();
                var comparer = new ProductComparer();
                CollectionAssert.AreEqual(
                    productList.OrderBy(product => product, comparer),
                    _products.OrderBy(product => product, comparer), comparer);
            }
        }

        /// <summary>
        /// Service should return null
        /// </summary>
        [Test]
        public void GetAllProductsTestForNull()
        {
            _products.Clear();
            var productEntities = _productService.GetAllProducts();
            Assert.Null(productEntities);
            SetUpProducts();
        }

        /// <summary>
        /// Service should return product if correct id is supplied
        /// </summary>
        [Test]
        public void GetProductByRightIdTest()
        {
            var mobileProductEntity = _productService.GetProductById(2);
            if (mobileProductEntity != null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ProductEntity, Product>();
                });
                IMapper mapper = config.CreateMapper();

                var productModel = mapper.Map<ProductEntity, Product>(mobileProductEntity);

                AssertObjects.PropertyValuesAreEquals(productModel,
                                                      _products.Find(a => a.ProductName.Contains("Mobile")));
            }
        }

        /// <summary>
        /// Service should return null
        /// </summary>
        [Test]
        public void GetProductByWrongIdTest()
        {
            var productEntity = _productService.GetProductById(0);
            Assert.Null(productEntity);
        }

        /// <summary>
        /// Add new product test
        /// </summary>
        [Test]
        public void AddNewProductTest()
        {
            var newProductEntity = new ProductEntity()
            {
                ProductName = "Android Phone"
            };

            var maxProductIDBeforeAdd = _products.Max(a => a.ProductId);
            newProductEntity.ProductId = maxProductIDBeforeAdd + 1;
            _productService.CreateProduct(newProductEntity);
            var addedproduct = new Product() { ProductName = newProductEntity.ProductName, ProductId = newProductEntity.ProductId };
            AssertObjects.PropertyValuesAreEquals(addedproduct, _products.Last());
            Assert.That(maxProductIDBeforeAdd + 1, Is.EqualTo(_products.Last().ProductId));
        }

        /// <summary>
        /// Update product test
        /// </summary>
        [Test]
        public void UpdateProductTest()
        {
            var firstProduct = _products.First();
            firstProduct.ProductName = "Laptop updated";
            var updatedProductEntity = new ProductEntity()
            {
                ProductName = firstProduct.ProductName, ProductId = firstProduct.ProductId
            };
            _productService.UpdateProduct(firstProduct.ProductId, updatedProductEntity);
            Assert.That(firstProduct.ProductId, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstProduct.ProductName, Is.EqualTo("Laptop updated")); // Product name changed
        }

        /// <summary>
        /// Delete product test
        /// </summary>
        [Test]
        public void DeleteProductTest()
        {
            int maxId = _products.Max(a => a.ProductId); // Before removal
            var lastProduct = _products.Last();

            // Remove last Product
            _productService.DeleteProduct(lastProduct.ProductId);
            Assert.That(maxId, Is.GreaterThan(_products.Max(a => a.ProductId))); // Max id reduced by 1
        }

        #endregion


        #region Tear Down

        /// <summary>
        /// Tears down each test data，每个不同test执行结束后触发
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _productService = null;
            _unitOfWork = null;
            _productRepository = null;
            _dbContext?.Dispose();
            _products = null;
        }

        #endregion

        #region TestFixture TearDown.

        /// <summary>
        /// TestFixture teardown, used when all tests execution ends
        /// </summary>
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _products = null;
        }

        #endregion
    }
}
