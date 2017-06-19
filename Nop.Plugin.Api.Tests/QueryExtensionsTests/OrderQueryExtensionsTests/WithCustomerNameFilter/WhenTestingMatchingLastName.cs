using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.QueryExtensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.QueryExtensionsTests.OrderQueryExtensionsTests.WithCustomerNameFilter
{
    [TestFixture]
    public class WhenTestingMatchingLastName
    {
        [Test]
        public void ItShouldReturnTheCorrectResults()
        {
            var orders = new List<Order>()
            {
                new Order() {Id = 2, BillingAddress = new Address() {FirstName = "John", LastName = "Blogs"} },
                new Order() {Id = 3, BillingAddress = new Address() {FirstName = "Bobb", LastName = "bloggings"} },
                new Order() {Id = 1, BillingAddress = new Address() {FirstName = "Bob", LastName = "blockings"} },
                new Order() {Id = 4, BillingAddress = new Address() {FirstName = "Pete", LastName = "blackburn"} }
            };

            var queryable = orders.AsQueryable();
            var result = queryable.WithCustomerNameFilter("Blo").ToList();

            Assert.AreEqual(result.Count, 3);
			Assert.IsTrue(result.Any(a=>a.Id == 3));
            Assert.IsTrue(result.Any(a => a.Id == 2));
            Assert.IsTrue(result.Any(a => a.Id == 1));
        }
    }
}
