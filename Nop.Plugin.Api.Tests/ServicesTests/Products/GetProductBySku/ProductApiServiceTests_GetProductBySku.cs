using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Products.GetProductBySku
{
    [TestFixture]
    public class ProductApiServiceTests_GetProductBySku
    {
        [Test]
        public void WhenNullIsReturnedByTheRepository_ShouldReturnNull()
        {
            string productSku = "A";

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
            productRepo.Stub(x => x.Table).Return((new List<Product>()).AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
            var result = cut.GetProductBySku(productSku);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void WhenNegativeOrZeroProductIdPassed_ShouldReturnNull(int negativeOrZeroProductId)
        {
            // Aranges
            var productRepoStub = MockRepository.GenerateStub<IRepository<Product>>();
            productRepoStub.Stub(x => x.Table).Return((new List<Product>()).AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productRepoStub, productCategoryRepo, vendorRepo);
            var result = cut.GetProductById(negativeOrZeroProductId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WhenProductIsReturnedByTheRepository_ShouldReturnTheSameProduct()
        {
            string productSku = "A";
            var product = new Product() { Sku = "A", Name = "some name" };

            // Arange
            var productRepo = MockRepository.GenerateStub<IRepository<Product>>();
 
            var list = new List<Product>();
            list.Add(product);

            productRepo.Stub(x => x.Table).Return(list.AsQueryable());

            var productCategoryRepo = MockRepository.GenerateStub<IRepository<ProductCategory>>();
            var vendorRepo = MockRepository.GenerateStub<IRepository<Vendor>>();

            // Act
            var cut = new ProductApiService(productRepo, productCategoryRepo, vendorRepo);
            var result = cut.GetProductBySku(productSku);

            // Assert
            Assert.AreSame(product, result);
        }
    }
}