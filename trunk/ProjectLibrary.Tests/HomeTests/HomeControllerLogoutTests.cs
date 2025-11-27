using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ProjectLibrary.Tests.HomeTests {
    public class HomeControllerLogoutTests : HomeControllerTests {
        [Fact]
        public async Task Logout_DeletesAuthCookieAndRedirectsToHome() {
            var responseCookiesMock = new Mock<IResponseCookies>();
            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();

            responseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(responseMock.Object);

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = httpContextMock.Object
            };

            var result = await _controller.Logout();

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            responseCookiesMock.Verify(c => c.Delete("auth-t"), Times.Once);
        }

        [Fact]
        public async Task Logout_Always_RedirectsToHomeIndex() {
            var responseCookiesMock = new Mock<IResponseCookies>();
            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();

            responseMock.Setup(r => r.Cookies).Returns(responseCookiesMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(responseMock.Object);

            _controller.ControllerContext = new ControllerContext() {
                HttpContext = httpContextMock.Object
            };

            var result = await _controller.Logout();

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}
