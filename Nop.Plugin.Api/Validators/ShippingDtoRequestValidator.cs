using FluentValidation;
using Nop.Plugin.Api.DTOs.Shipping;

namespace Nop.Plugin.Api.Validators
{
    public class ShippingOptionsDtoRequestValidator : AbstractValidator<ShippingOptionsDtoRequest>
    {
        public ShippingOptionsDtoRequestValidator()
        {
            RuleFor(a => a.CustomerId).NotNull().GreaterThan(0).WithMessage("customerId required");
            RuleFor(a => a.ShippingAddress).NotNull().WithMessage("shipping address required"); ;
        }
    }
}