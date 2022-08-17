using FluentAssertions;
using Moq;
using Serwis.Core;
using Serwis.Models.Domains;
using Serwis.Persistance.Service;

namespace Testy
{
    public class TestService
    {
        private readonly Mock<IUnitOfWork> _repositoryStub = new();

        [Fact]
        public async void GetProductsById_WillReturnProperList()
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

            var service = new Service(_repositoryStub.Object); //jedno polaczenie z baza danych

            var result = await service.GetProductsById(lista);

            result.Should().BeEquivalentTo(expectedItems);

        }

        private Product CreateRandomProduct()
        {
            return new()
            {
                Id = 1,
                Name = "Sos",
                Price = 10.20M,
            };
        }
    }
}