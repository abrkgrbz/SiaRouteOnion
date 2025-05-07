using Application.Interfaces.SSIServices;
using Microsoft.Extensions.Configuration;
using Moq;

namespace SiaRouteTest
{
    public class SSIDataServiceTests
    {
        private readonly Mock<ISSIDataService> _ssiDataService;

        public SSIDataServiceTests()
        {
            _ssiDataService=new Mock<ISSIDataService>();
        }


        [Fact]
        public async Task GetMapList_ShouldNotNull()
        {
            _ssiDataService.Setup(x => x.GetMapList("S23P2764B")).ReturnsAsync(new List<string> { "Map1", "Map2" });
            var result = await _ssiDataService.Object.GetMapList("S23P2764B");
            Assert.NotNull(result);
        }

    }

   
}