using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Http;
using AutoMock;
using Newtonsoft.Json;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Models.ShoppingCartsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ShoppingCartItems
{
    [TestFixture]
    public class ShoppingCartItemsControllerTests_GetShoppingCartItemsByCustomerId
    {

        [Test]
        public void WhenReturnedShouldCalculateSubTotals_ShouldReturnValidRepsonse()
        {
            var parameters = new ShoppingCartItemsForCustomerParametersModel();
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>(new JsonFieldsSerializer());

            var item1 = new ShoppingCartItem() {Id = 1};
            var item2 = new ShoppingCartItem() {Id = 2};

            autoMocker.Get<IShoppingCartItemApiService>()
                .Stub(a => a.GetShoppingCartItems(Arg<int>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<DateTime>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                .Return(new List<ShoppingCartItem>() {item1,item2 });

            autoMocker.Get<IPriceCalculationService>()
                .Stub(a => a.GetUnitPrice(Arg<ShoppingCartItem>.Is.Equal(item1),Arg<bool>.Is.Anything))
                .Return(9.99m);

            autoMocker.Get<IPriceCalculationService>()
              .Stub(a => a.GetUnitPrice(Arg<ShoppingCartItem>.Is.Equal(item2), Arg<bool>.Is.Anything))
              .Return(1.99m);

            IHttpActionResult httpActionResult = autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(2, parameters);
            var result = httpActionResult.ExecuteAsync(new CancellationToken()).Result;
            var str = result.Content.ReadAsStringAsync().Result;// .ReadAsStringAsync().Result;
           
            var cart = JsonConvert.DeserializeObject<ShoppingCartItemsRootObject>(str);

            Assert.AreEqual(cart.ShoppingCartItems.Count,2);
            Assert.AreEqual(cart.ShoppingCartItems[0].SubTotal,9.99m);
            Assert.AreEqual(cart.ShoppingCartItems[1].SubTotal, 1.99m);
            Assert.AreEqual(cart.SubTotal,11.98m);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturnBadRequest(int nonPositiveCustomerId)
        {
            // Arange
            var parameters = new ShoppingCartItemsForCustomerParametersModel();
            
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                       .IgnoreArguments()
                                                       .Return(string.Empty);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(nonPositiveCustomerId, parameters);

            // Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallShoppingCartItemsApiService(int negativeShoppingCartItemsId)
        {
            // Arange
            var parameters = new ShoppingCartItemsForCustomerParametersModel();
            
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

            // Act
            autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(negativeShoppingCartItemsId, parameters);

            // Assert
            autoMocker.Get<IShoppingCartItemApiService>().AssertWasNotCalled(x => x.GetShoppingCartItems(negativeShoppingCartItemsId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchShoppingCartItemsExists_ShouldReturn404NotFound()
        {
            int nonExistingShoppingCartItemId = 5;
            var parameters = new ShoppingCartItemsForCustomerParametersModel();

            // Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems(nonExistingShoppingCartItemId)).Return(null);

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Is.Anything, Arg<string>.Is.Anything))
                                                       .IgnoreArguments()
                                                       .Return(string.Empty);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(nonExistingShoppingCartItemId, parameters);

            // Assert
            var statusCode = result.ExecuteAsync(new CancellationToken()).Result.StatusCode;

            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }

        [Test]
        public void WhenIdEqualsToExistingShoppingCartItemId_ShouldSerializeThatShoppingCartItem()
        {
            MappingExtensions.Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            int existingShoppingCartItemId = 5;
            var existingShoppingCartItems = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() {Id = existingShoppingCartItemId}
            };

            var parameters = new ShoppingCartItemsForCustomerParametersModel();

            // Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems(existingShoppingCartItemId)).Return(existingShoppingCartItems);

            // Act
            autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(existingShoppingCartItemId, parameters);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<ShoppingCartItemsRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.ShoppingCartItems.Count == 1 &&
                               objectToSerialize.ShoppingCartItems[0].Id == existingShoppingCartItemId.ToString()),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingShoppingCartItemIdAndFieldsSet_ShouldReturnJsonForThatShoppingCartItemWithSpecifiedFields()
        {
            MappingExtensions.Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            int existingShoppingCartItemId = 5;
            var existingShoppingCartItems = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() {Id = existingShoppingCartItemId}
            };
            
            var parameters = new ShoppingCartItemsForCustomerParametersModel()
            {
                Fields = "id,quantity"
            };

            // Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems(existingShoppingCartItemId)).Return(existingShoppingCartItems);

            // Act
            autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(existingShoppingCartItemId, parameters);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<ShoppingCartItemsRootObject>.Matches(objectToSerialize => objectToSerialize.ShoppingCartItems[0].Id == existingShoppingCartItemId.ToString()),
                    Arg<string>.Matches(fieldsParameter => fieldsParameter == parameters.Fields)));
        }
    }
}