using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Core.Service;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.ViewModels;
using Serwis.Persistance.Service;
using Serwis.ShopControllers;

namespace Testy
{
    public class TestService
    {
        private readonly Mock<IUnitOfWork> _repositoryStub = new();
        private readonly Mock<IService> _serviceStub = new();
        private readonly Mock<IOrderPositionRepository> _orderPositionRepositoryStub = new();
        private readonly EmailSender _emailSender; // klasy nie moga byc mockowane to jest tylko na uzytek testu
        private readonly List<OrderPositionViewModel> _orderPositions;
        private IHttpContextAccessor _contextAccessor;

        [Fact]
        public async void GetProductsById_IfPutList_WillReturnProperList()
        {
            //arrange
            var lista = new List<int>
            {
                1,2,3
            };
            var expectedItems = new List<Product>
            {
                CreateRandomProduct(),
                CreateRandomProduct(),
                CreateRandomProduct()
            };

            Product product = CreateRandomProduct();
            _repositoryStub.Setup(x => x.Product.FindProductByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

            var service = new Service(_contextAccessor, _repositoryStub.Object); 

            var result = await service.GetProductsById(lista);

            result.Should().BeEquivalentTo(expectedItems);

        }

        [Fact]
        public async Task GetProductAsync_IfProductIdIsNotValid_ReturnNull()
        {
            //arrange
            Random rnd = new Random();
            

            Product product = CreateRandomProduct();

            _repositoryStub.Setup(x => x.Product.GetProductAsync(It.IsInRange(1,30,Moq.Range.Inclusive))).ReturnsAsync(product);
            //po podaniu jakiegokolwiek id zwroc product

            var service = new Service(_contextAccessor, _repositoryStub.Object); //jedno polaczenie z baza danych

            //act
            var result = await service.GetProductAsync(rnd.Next(31, 100));

            //assert
            result.Should().BeNull("Nieprawid³owe id");

        }

        [Fact]
        public async Task SingleProductPurchase_IfProductIdIsNotValid_ReturnView()
        {
            //arrange
            Random rnd = new Random();


            Product product = CreateRandomProduct();

            _serviceStub.Setup(x => x.GetProductAsync(It.IsInRange(1, 30, Moq.Range.Inclusive))).ReturnsAsync(product);
            //po podaniu jakiegokolwiek id zwroc product
            var service = new ShopController(_serviceStub.Object, _orderPositions); //jedno polaczenie z baza danych

            //act
            var result =  service.SingleProductPurchase(rnd.Next(31, 100));

            //assert
            result.Result.Should().BeOfType<ViewResult>();
        }

        //[Fact]

        //public async Task UpdateOrderPositionAsync_WhenGetOrderPosition_Update()
        //{
        //    _orderPositionRepositoryStub.Setup(x => x.UpdateOrderPositionAsync(It.IsAny<OrderPosition>()));


        //    var service = new Service(_contextAccessor,_repositoryStub.Object);

        //    var result = service.UpdateOrderPositionAsync(CreateRandomOrderPosition());

        //    result.Should().NotBeOfType(System.Threading.Tasks);

        //}

        private Product CreateRandomProduct()
        {
            return new()
            {
                Id = 1,
                Name = "Sos",
                Price = 10.20M,
            };
        }

        private OrderPosition CreateRandomOrderPosition()
        {
            return new()
            {
                Id = 0,
                OrderId = 1,
                ProductId = 1,
                UserId = new Guid("7C57B0DE - C98D - 417A - A842 - 67D77CFCCBE5")
            };
        }
    }
}