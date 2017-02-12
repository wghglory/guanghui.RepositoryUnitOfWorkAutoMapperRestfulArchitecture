using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using Guanghui.Repository.UnitOfWork;
using Guanghui.BusinessEntities;
using Guanghui.BusinessServices.Interfaces;
using Guanghui.Repository;

namespace Guanghui.BusinessServices.Implementations
{
    /// <summary>
    /// Offers services for product specific CRUD operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            //_unitOfWork = new UnitOfWork();
            _unitOfWork = unitOfWork;    //unity constructor injection
        }


        public ProductEntity GetProductById(int productId)
        {
            var product = _unitOfWork.ProductRepository.GetById(productId);
            if (product != null)
            {
                #region Before 4.2
                ////Mapper.CreateMap<Product, ProductEntity>();  obsolete
                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<Product, ProductEntity>();
                //});
                //var productModel = Mapper.Map<Product, ProductEntity>(product);
                #endregion

                #region After 4.2
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Product, ProductEntity>();
                });
                IMapper mapper = config.CreateMapper();

                var productModel = mapper.Map<Product, ProductEntity>(product);
                #endregion

                return productModel;
            }
            return null;
        }

        public IEnumerable<ProductEntity> GetAllProducts()
        {
            IEnumerable<Product> products = _unitOfWork.ProductRepository.Get();
            if (products.Any())
            {
                #region Before 4.2
                //Mapper.CreateMap<Product, ProductEntity>();  //obsolete   

                // OR

                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<Product, ProductEntity>();
                //}); 

                //var productsModel = Mapper.Map<List<Product>, List<ProductEntity>>(products.ToList());
                #endregion

                #region After 4.2
                var config = new MapperConfiguration(cfg =>
                       {
                           cfg.CreateMap<Product, ProductEntity>();
                       });
                IMapper mapper = config.CreateMapper();

                var productsModel = mapper.Map<List<Product>, List<ProductEntity>>(products.ToList()); 
                #endregion
                return productsModel;
            }
            return null;
        }


        public int CreateProduct(ProductEntity productEntity)
        {
            using (var scope = new TransactionScope())
            {
                var product = new Product
                {
                    ProductName = productEntity.ProductName
                };
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();
                scope.Complete();
                return product.ProductId;
            }
        }


        public bool UpdateProduct(int productId, ProductEntity productEntity)
        {
            var success = false;
            if (productEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var product = _unitOfWork.ProductRepository.GetById(productId);
                    if (product != null)
                    {
                        product.ProductName = productEntity.ProductName;
                        _unitOfWork.ProductRepository.Update(product);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }


        public bool DeleteProduct(int productId)
        {
            var success = false;
            if (productId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var product = _unitOfWork.ProductRepository.GetById(productId);
                    if (product != null)
                    {

                        _unitOfWork.ProductRepository.Delete(product);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
    }
}
