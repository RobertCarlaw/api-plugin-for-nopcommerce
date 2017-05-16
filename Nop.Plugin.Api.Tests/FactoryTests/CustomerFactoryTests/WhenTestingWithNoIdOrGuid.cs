
using System;
using System.Web.Http.Routing.Constraints;
using AutoMock;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Factories;
using Nop.Services.Customers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.FactoryTests.CustomerFactoryTests
{
    [TestFixture]
    public class WhenTestingWithIdOrGuid
    {
        private RhinoAutoMocker<CustomerFactory> autoMocker = new RhinoAutoMocker<CustomerFactory>();
        readonly ShoppingCartItemDto model = new ShoppingCartItemDto();
        private ICustomerService _service;

        [SetUp]
        public void When()
        {
            autoMocker.Get<ICustomerService>()
                .Stub(a => a.InsertGuestCustomer())
                .Return(new Customer() {Id = 444});

            _service = autoMocker.Get<ICustomerService>();
            var result = autoMocker.ClassUnderTest.CreateFor(model);
        }

        [Test]
        public void ItShouldCallInsertGuestCustomer()
        {
            _service.AssertWasCalled(a=>a.InsertGuestCustomer());
        }

        [Test]
        public void ItShouldNotCallGetCustomerById()
        {
            _service.AssertWasNotCalled(a => a.GetCustomerById(Arg<int>.Is.Anything));
        }

        [Test]
        public void ItShouldNotCallGetCustomerByGuid()
        {
            _service.AssertWasNotCalled(a => a.GetCustomerByGuid(Arg<Guid>.Is.Anything));
        }
    }
}

