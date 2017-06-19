﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.QueryExtensions;
using NUnit.Framework;

namespace Nop.Plugin.Api.Tests.QueryExtensionsTests.OrderQueryExtensionsTests.WithToDateRangeFilter
{
    [TestFixture]
    public class WhenTestingMatchingToDate
    {
        [Test]
        public void ItShouldReturnTheCorrectResults()
        {
            var dateTime = new DateTime(2017, 6, 14, 14, 55, 22);

            var  orders = new List<Order>()
            {
                new Order() {Id = 2, CreatedOnUtc = dateTime.AddDays(-1)},
                new Order() {Id = 3, CreatedOnUtc = dateTime.AddSeconds(1)},
                new Order() {Id = 1, CreatedOnUtc = dateTime.AddHours(-2)},
                new Order() {Id = 4, CreatedOnUtc = dateTime},
            };

            var queryable = orders.AsQueryable();
            var result = queryable.WithToDateRangeFilter(dateTime).ToList();

            Assert.AreEqual(result.Count, 2);
        }
    }
}