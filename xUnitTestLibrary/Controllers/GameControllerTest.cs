using System;
using Xunit;

namespace xUnitTestLibrary.Controllers
{
    
    public class GameControllerTest
    {
        
        [Fact]
        public void IndexActionText()
        {
            Assert.Equal(2 + 2, 4);
        }

        [Fact]
        public void SecondTest()
        {
            Assert.NotNull("Hello World");
        }
    }
}
