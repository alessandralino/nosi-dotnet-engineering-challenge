using NOS.Engineering.Challenge.Managers;
using Moq;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssert;
using NOS.Engineering.Challenge.API.Models; 

namespace NOS.Engineering.Challenge.API.Tests
{
    public class ContentControllerTests
    {
        Mock<IContentsManager> _mockContentManager;
        Mock<ILogger<ContentController>> _mockLogger;
        ContentController _contentController;

        public ContentControllerTests()
        {
            _mockContentManager = new Mock<IContentsManager>();
            _mockLogger = new Mock<ILogger<ContentController>>(); 
            _contentController = new ContentController(_mockContentManager.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetManyContents_ReturnsNotFound_WhenContentsExist()
        {
            // Arrange
            _mockContentManager.Setup(m => m.GetManyContents()).ReturnsAsync(new List<Content>());

            // Act
            var result = await _contentController.GetManyContents();

            // Assert
            result.ShouldBeEqualTo(result);
        }

        [Fact]
        public async Task GetManyContents_ReturnsNotFound_WhenNoContentsExist()
        {
            // Arrange
            _mockContentManager.Setup(m => m.GetManyContents()).ReturnsAsync(new List<Content>());

            // Act
            var result = await _contentController.GetManyContents();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetManyContents_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockContentManager.Setup(m => m.GetManyContents()).ThrowsAsync(new Exception());

            // Act
            var result = await _contentController.GetManyContents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetContent_ReturnsOk_WhenContentExists()
        {
            // Arrange
            var contentId = Guid.NewGuid();
           
            var mockContent = new Content(
                id: contentId,
                title: "Title 1",
                subTitle: "The subtitle test",
                description: "Description Test",
                imageUrl: "https://image.com/1",
                duration: 120,
                startTime: DateTime.UtcNow,
                endTime: DateTime.UtcNow,
                genreList: new List<string> { "Genre test 1" });

            _mockContentManager.Setup(m => m.GetContent(contentId)).ReturnsAsync(mockContent); 

            // Act
            var result = await _contentController.GetContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnContent = Assert.IsType<Content>(okResult.Value);
            Assert.Equal("Title 1", returnContent.Title);
        }

        [Fact]
        public async Task GetContent_ReturnsNotFound_WhenContentDoesNotExist()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            _mockContentManager.Setup(m => m.GetContent(contentId)).ReturnsAsync((Content)null!);

            // Act
            var result = await _contentController.GetContent(contentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetContent_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            _mockContentManager.Setup(m => m.GetContent(contentId)).ThrowsAsync(new Exception());

            // Act
            var result = await _contentController.GetContent(contentId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task CreateContent_ReturnsOk_WhenContentIsCreated()
        {
            // Arrange
            var contentInput = new ContentInput
            {
                Title = "Test Title",
                SubTitle = "Test Subtitle",
                Description = "Test Description",
                ImageUrl = "https://exp.com/test__2024_03_09.png",
                Duration = 120,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
            };

            var contentDto = contentInput.ToDto();
            var createdContent = new Content
            (
                Guid.NewGuid(),
                contentDto.Title!,
                contentDto.SubTitle!,
                contentDto.Description!,
                contentDto.ImageUrl!,
                contentDto.Duration.GetValueOrDefault(),
                contentDto.StartTime.GetValueOrDefault(),
                contentDto.EndTime.GetValueOrDefault(),
                contentDto.GenreList.ToList()
            );

            _mockContentManager.Setup(manager => manager.CreateContent(It.IsAny<ContentDto>())).ReturnsAsync(createdContent);

            var controller = new ContentController(_mockContentManager.Object, _mockLogger.Object);

            // Act
            var result = await controller.CreateContent(contentInput) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var resultContent = result.Value as Content;
            Assert.NotNull(resultContent);
            Assert.Equal(createdContent.Id, resultContent.Id);
            Assert.Equal(createdContent.Title, resultContent.Title);
            Assert.Equal(createdContent.SubTitle, resultContent.SubTitle);
            Assert.Equal(createdContent.Description, resultContent.Description);
            Assert.Equal(createdContent.ImageUrl, resultContent.ImageUrl);
            Assert.Equal(createdContent.Duration, resultContent.Duration);
            Assert.Equal(createdContent.StartTime, resultContent.StartTime);
            Assert.Equal(createdContent.EndTime, resultContent.EndTime);
            Assert.Equal(createdContent.GenreList, resultContent.GenreList);

        }

        [Fact]
        public async Task CreateContent_ReturnsProblem_WhenCreationFails()
        {
            // Arrange
            var contentInput = new ContentInput();
            _mockContentManager
                .Setup(m => m.CreateContent(It.IsAny<ContentDto>()))
                .ReturnsAsync((Content)null!);  

            // Act
            var result = await _contentController.CreateContent(contentInput);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
            
        }

        [Fact]
        public async Task CreateContent_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var contentInput = new ContentInput();
            _mockContentManager
                .Setup(m => m.CreateContent(It.IsAny<ContentDto>()))
                .ThrowsAsync(new Exception("Something went wrong"));  

            // Act
            var result = await _contentController.CreateContent(contentInput);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode); 
        }

        [Fact]
        public async Task UpdateContent_ReturnsOk_WhenContentIsUpdated()
        {
            // Arrange
            var idContent = Guid.NewGuid();
            var contentInput = new ContentInput
            {
                Title = "Updated Title",
                SubTitle = "Updated Subtitle",
                Description = "Test Description",
                ImageUrl = "https://exp.com/test__2024_03_09.png",
                Duration = 120,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
            };

            var contentDto = contentInput.ToDto();
            var createdContent = new Content
            (
                idContent,
                contentDto.Title!,
                contentDto.SubTitle!,
                contentDto.Description!,
                contentDto.ImageUrl!,
                contentDto.Duration.GetValueOrDefault(),
                contentDto.StartTime.GetValueOrDefault(),
                contentDto.EndTime.GetValueOrDefault(),
                contentDto.GenreList.ToList()
            );

            _mockContentManager.Setup(manager => manager.UpdateContent(idContent, It.IsAny<ContentDto>())).ReturnsAsync(createdContent);

            // Act
            var result = await _contentController.UpdateContent(idContent, contentInput);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnContent = Assert.IsType<Content>(okResult.Value);
            Assert.Equal("Updated Title", returnContent.Title);
        }

        [Fact]
        public async Task UpdateContent_ReturnsNotFound_WhenContentDoesNotExist()
        {
            // Arrange
            var idContent = Guid.NewGuid();
            var contentInput = new ContentInput
            {
                Title = "Updated Title",
                SubTitle = "Updated Subtitle",
                Description = "Test Description",
                ImageUrl = "https://exp.com/test__2024_03_09.png",
                Duration = 120,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
            };

            var contentDto = contentInput.ToDto();
            var createdContent = new Content
            (
                idContent,
                contentDto.Title!,
                contentDto.SubTitle!,
                contentDto.Description!,
                contentDto.ImageUrl!,
                contentDto.Duration.GetValueOrDefault(),
                contentDto.StartTime.GetValueOrDefault(),
                contentDto.EndTime.GetValueOrDefault(),
                contentDto.GenreList.ToList()
            );

            _mockContentManager.Setup(m => m.UpdateContent(idContent, It.IsAny<ContentDto>())).ReturnsAsync((Content)null!);


            // Act
            var result = await _contentController.UpdateContent(idContent, contentInput);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteContent_ReturnsOk_WhenContentIsDeleted()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            _mockContentManager.Setup(m => m.DeleteContent(contentId)).ReturnsAsync(contentId);

            // Act
            var result = await _contentController.DeleteContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnId = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(contentId, returnId);
        }

    }
}
