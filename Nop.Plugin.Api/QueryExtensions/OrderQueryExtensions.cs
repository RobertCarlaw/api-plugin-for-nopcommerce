using System;
using System.Data.Entity;
using System.Linq;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.QueryExtensions
{
    public static class OrderQueryExtensions
    {
        public static IQueryable<Order> WithOrderIdFilter(this IQueryable<Order> query, int orderId)
        {
            if (orderId > 0)
                query = query.Where(a => a.Id == orderId);
          
            return query;
        }

        public static IQueryable<Order> WithFromDateRangeFilter(this IQueryable<Order> query, DateTime? from)
        {
            if (from != null)
                query = query.Where(a => a.CreatedOnUtc > from.Value);

            return query;
        }

        [DbFunctionAttribute("Edm", "TruncateTime")]
        public static IQueryable<Order> WithToDateRangeFilter(this IQueryable<Order> query, DateTime? to)
        {
            if (to != null)
                query = query.Where(a => a.CreatedOnUtc < to.Value);

            return query;
        }

        public static IQueryable<Order> WithEmailAddressFilter(this IQueryable<Order> query, string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
                query = query.Where(a => a.BillingAddress.Email.ToLower().Contains(emailAddress.ToLower())); 

            return query;
        }

        public static IQueryable<Order> WithCustomerNameFilter(this IQueryable<Order> query, string customerName)
        {
            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(a => a.BillingAddress.FirstName.ToLower().Contains(customerName.ToLower())
                                         || a.BillingAddress.LastName.ToLower().Contains(customerName.ToLower()));
                
            return query;
        }
    }
}
