using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ProjectLibrary.Tests.HomeTests {
    public class HomeControllerLogoutTests : HomeControllerTests {
        [Fact]
        public async Task Logout_DeletesAuthCookieAndRedirectsToIndex() {
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
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            responseCookiesMock.Verify(c => c.Delete("auth-t"), Times.Once);
        }

        [Fact]
        public async Task Logout_Always_RedirectsToIndex() {
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
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }
    }
}
