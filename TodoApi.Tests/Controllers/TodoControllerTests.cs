using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApi.Controllers;
using TodoApi.Models.Dto;
using TodoApi.Services.Abstrations;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new TodoController(_mockTodoService.Object, _mockUserService.Object);
        }

        private void SetUserClaims(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "mock");
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenUserIsAdmin()
        {
            // Arrange
            SetUserClaims("user123");
            _mockUserService.Setup(u => u.IsAdmin(It.IsAny<string>())).ReturnsAsync(true);
            _mockTodoService.Setup(t => t.GetAllTodosAsync()).ReturnsAsync(new List<TodoDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenUserIsNotAdmin()
        {
            // Arrange
            SetUserClaims("user123");
            _mockUserService.Setup(u => u.IsAdmin(It.IsAny<string>())).ReturnsAsync(false);
            _mockTodoService.Setup(t => t.GetUserTodosAsync(It.IsAny<string>())).ReturnsAsync(new List<TodoDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            SetUserClaims("user123");
            _mockTodoService.Setup(t => t.GetTodoByIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync((TodoDto)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenTodoExists()
        {
            // Arrange
            SetUserClaims("user123");
            var todo = new TodoDto { Id = 1, Title = "Test Todo" };
            _mockTodoService.Setup(t => t.GetTodoByIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(todo);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<TodoDto>>(okResult.Value);
            Assert.True(response.Status);
            Assert.Equal("Todo information!", response.Message);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenTodoIsCreated()
        {
            // Arrange
            SetUserClaims("user123");
            var newTodo = new TodoDto { Title = "New Todo" };
            var createdTodo = new TodoDto { Id = 1, Title = "New Todo" };
            _mockTodoService.Setup(t => t.AddTodoAsync(It.IsAny<TodoDto>(), It.IsAny<string>())).ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.Create(newTodo);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenTodoIsUpdated()
        {
            // Arrange
            SetUserClaims("user123");
            var updatedTodo = new TodoDto { Id = 1, Title = "Updated Todo" };
            _mockTodoService.Setup(t => t.UpdateTodoAsync(It.IsAny<int>(), It.IsAny<TodoDto>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(updatedTodo);

            // Act
            var result = await _controller.Update(1, updatedTodo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<TodoDto>>(okResult.Value);
            Assert.True(response.Status);
            Assert.Equal("Todo updated successfully!", response.Message);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenTodoIsDeleted()
        {
            // Arrange
            SetUserClaims("user123");
            _mockTodoService.Setup(t => t.DeleteTodoAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            SetUserClaims("user123");
            _mockTodoService.Setup(t => t.DeleteTodoAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
