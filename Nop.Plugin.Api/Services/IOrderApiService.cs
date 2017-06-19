﻿using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderApiService
    {
        IList<Order> GetOrdersByCustomerId(int customerId);

        IList<Order> GetOrders(out int totalRecords,IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                              int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
                              int sinceId = Configurations.DefaultSinceId, OrderStatus? status = null,
                              PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, int? customerId = null, string customerEmail = "", string customerName = "" );

        Order GetOrderById(int orderId);

        int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
                           PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
                           int? customerId = null);
    }
}