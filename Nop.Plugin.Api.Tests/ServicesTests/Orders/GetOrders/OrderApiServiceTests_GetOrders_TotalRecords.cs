using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrders
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrders_TotalRecords
    {
        private IOrderApiService _orderApiService;
        private List<Order> _existigOrders;
        private int _totalOrders;

        [SetUp]
        public void Setup()
        {
            _existigOrders = new List<Order>()
            {
                new Order() {Id = 2},
                new Order() {Id = 3},
                new Order() {Id = 1},
                new Order() {Id = 4},
                new Order() {Id = 5},
                new Order() {Id = 6},
                new Order() {Id = 7}
            };

            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.TableNoTracking).Return(_existigOrders.AsQueryable());

            _orderApiService = new OrderApiService(orderRepo);
        }

        [Test]
        public void ItShouldReturnTheCorrectTotalRecords()
        {
            _orderApiService.GetOrders(out _totalOrders);
            Assert.AreEqual(_totalOrders,7);
        }
    }
}
