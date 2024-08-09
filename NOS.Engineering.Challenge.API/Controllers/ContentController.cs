using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;

namespace NOS.Engineering.Challenge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContentController : Controller
    {
        private readonly IContentsManager _manager;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentsManager manager, ILogger<ContentController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [Obsolete("This endpoint is deprecated. Please use GET /api/v1/Content/filter instead.")]
        [HttpGet]
        public async Task<IActionResult> GetManyContents()
        {
            _logger.LogInformation("Fetching all contents at {Timestamp}.", DateTime.UtcNow);

            try
            {
                var contents = await _manager.GetManyContents().ConfigureAwait(false);

                if (!contents.Any())
                {
                    _logger.LogWarning("No contents found at {Timestamp}.", DateTime.UtcNow);
                    return NotFound();
                }

                _logger.LogInformation("Fetched {Count} contents at {Timestamp}.", contents.Count(), DateTime.UtcNow);
                return Ok(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching contents at {Timestamp}.", DateTime.UtcNow);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredContents([FromQuery] string? title, [FromQuery] string? genre)
        {
            _logger.LogInformation("Fetching filtered contents with Title: {Title} and Genre: {Genre} at {Timestamp}.", title, genre, DateTime.UtcNow);

            try
            {
                var contents = await _manager.GetManyContents().ConfigureAwait(false);

                if (!string.IsNullOrEmpty(title))
                {
                    contents = contents.Where(c => c.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    contents = contents.Where(c => c.GenreList.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase))).ToList();
                }

                if (!contents.Any())
                {
                    _logger.LogWarning("No contents found matching the filters at {Timestamp}.", DateTime.UtcNow);
                    return NotFound();
                }

                _logger.LogInformation("Fetched {Count} filtered contents at {Timestamp}.", contents.Count(), DateTime.UtcNow);
                return Ok(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching filtered contents at {Timestamp}.", DateTime.UtcNow);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContent(Guid id)
        {
            _logger.LogInformation("Fetching content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);

            try
            {
                var content = await _manager.GetContent(id).ConfigureAwait(false);

                if (content == null)
                {
                    _logger.LogWarning("Content with ID {Id} not found at {Timestamp}.", id, DateTime.UtcNow);
                    return NotFound();
                }

                _logger.LogInformation("Fetched content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] ContentInput content)
        {
            _logger.LogInformation("Creating new content at {Timestamp}.", DateTime.UtcNow);

            try
            {
                var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

                if (createdContent == null)
                {
                    _logger.LogError("Failed to create content at {Timestamp}. Request: {@ContentInput}", DateTime.UtcNow, content);
                    return Problem();
                }

                _logger.LogInformation("Created content with ID {Id} at {Timestamp}.", createdContent.Id, DateTime.UtcNow);
                return Ok(createdContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating content at {Timestamp}. Request: {@ContentInput}", DateTime.UtcNow, content);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] ContentInput content)
        {
            _logger.LogInformation("Updating content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);

            try
            {
                var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

                if (updatedContent == null)
                {
                    _logger.LogWarning("Failed to update content with ID {Id} at {Timestamp}. Request: {@ContentInput}", id, DateTime.UtcNow, content);
                    return NotFound();
                }

                _logger.LogInformation("Updated content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return Ok(updatedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating content with ID {Id} at {Timestamp}. Request: {@ContentInput}", id, DateTime.UtcNow, content);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(Guid id)
        {
            _logger.LogInformation("Deleting content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);

            try
            {
                var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);

                _logger.LogInformation("Deleted content with ID {Id} at {Timestamp}.", deletedId, DateTime.UtcNow);
                return Ok(deletedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("{id}/genre")]
        public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genre)
        {
            _logger.LogInformation("Adding genres to content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);

            try
            {
                var updatedContent = await _manager.AddGenres(id, genre).ConfigureAwait(false);

                if (updatedContent == null)
                {
                    _logger.LogWarning("Content with ID {Id} not found for adding genres at {Timestamp}.", id, DateTime.UtcNow);
                    return NotFound();
                }

                _logger.LogInformation("Added genres to content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return Ok(updatedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding genres to content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id}/genre")]
        public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genre)
        {
            _logger.LogInformation("Removing genres from content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);

            try
            {
                var updatedContent = await _manager.RemoveGenres(id, genre).ConfigureAwait(false);

                if (updatedContent == null)
                {
                    _logger.LogWarning("Content with ID {Id} not found for removing genres at {Timestamp}.", id, DateTime.UtcNow);
                    return NotFound();
                }

                _logger.LogInformation("Removed genres from content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return Ok(updatedContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing genres from content with ID {Id} at {Timestamp}.", id, DateTime.UtcNow);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
