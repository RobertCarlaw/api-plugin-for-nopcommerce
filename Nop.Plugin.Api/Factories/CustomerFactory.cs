using System;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Factories
{
    public class CustomerFactory : IFactory<Customer>, IShoppingCartFactory<Customer>
    {
        private readonly ICustomerService _customerService;

        public CustomerFactory(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public Customer Initialize()
        {
            var defaultCustomer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                Active = true
            };

            return defaultCustomer;
        }

        public Customer CreateFor(ShoppingCartItemDto model)
        {
            Customer customer = null;

            if (model.CustomerId.HasValue)  
            {
                customer = _customerService.GetCustomerById(model.CustomerId.Value);
            }
            else if (!string.IsNullOrEmpty(model.CustomerGuid))
            {
                customer = _customerService.GetCustomerByGuid(Guid.Parse(model.CustomerGuid));
            }

            return customer ?? (_customerService.InsertGuestCustomer());
        }
    }
}