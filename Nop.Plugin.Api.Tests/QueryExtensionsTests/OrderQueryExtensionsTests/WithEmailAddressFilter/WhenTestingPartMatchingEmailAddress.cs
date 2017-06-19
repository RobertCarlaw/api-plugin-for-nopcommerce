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
    public class WhenTestingPartMatchingEmailAddress
    {
        [Test]
        public void ItShouldReturnTheCorrectResults()
        {
            var  orders = new List<Order>()
            {
                new Order() {Id = 2, BillingAddress = new Address() {Email = "546@test.com"} },
                new Order() {Id = 3, BillingAddress = new Address() {Email = "456@abc.com"}},
                new Order() {Id = 1, BillingAddress = new Address() {Email = "test@test.com"}},
                new Order() {Id = 4, BillingAddress = new Address() {Email = "789@dec.com"}},
            };

            var queryable = orders.AsQueryable();
            var result = queryable.WithEmailAddressFilter("test").ToList();

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0].Id, 2);
            Assert.AreEqual(result[1].Id, 1);
        }
    }
}
