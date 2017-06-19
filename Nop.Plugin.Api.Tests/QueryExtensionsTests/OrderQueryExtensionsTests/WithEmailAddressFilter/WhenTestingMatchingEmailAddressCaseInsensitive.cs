using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.QueryExtensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.QueryExtensionsTests.OrderQueryExtensionsTests.WithEmailAddressFilter
{
    [TestFixture]
    public class WhenTestingMatchingEmailAddressCaseInsensitive
    {
        [Test]
        public void ItShouldReturnTheCorrectResults()
        {
            var  orders = new List<Order>()
            {
                new Order() {Id = 2, BillingAddress = new Address() {Email = "123@test.com"} },
                new Order() {Id = 3, BillingAddress = new Address() {Email = "456@test.com"}},
                new Order() {Id = 1, BillingAddress = new Address() {Email = "test@test.com"}},
                new Order() {Id = 4, BillingAddress = new Address() {Email = "789@test.com"}},
            };

            var queryable = orders.AsQueryable();
            var result = queryable.WithEmailAddressFilter("TEST@TEST.COM").ToList();

            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].Id, 1);
        }
    }
}
