﻿using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.QueryExtensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.QueryExtensionsTests.OrderQueryExtensionsTests.WithOrderIdFilter
{
    [TestFixture]
    public class WhenTestingWithNoOrderIdParameter
    {
        [Test]
        public void ItShouldFilterByOrderId()
        {
            var  orders = new List<Order>()
            {
                new Order() {Id = 2 },
                new Order() {Id = 3 },
                new Order() {Id = 1 },
            };

            var queryable = orders.AsQueryable();
            var result = queryable.WithOrderIdFilter(0).ToList();

            Assert.AreEqual(result.Count, 3);
        }
    }
}
