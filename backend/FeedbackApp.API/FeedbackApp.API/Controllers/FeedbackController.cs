using FeedbackApp.Application.DTOs;
using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var form = await _feedbackService.GetAllAsync();
            return Ok(form);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null)
            {
                return NotFound(new { message = "Geri bildirim bulunamadý." });
            }
            return Ok(feedback);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var feedback = new Feedback
            {
                Name = feedbackDto.Name,
                Email = feedbackDto.Email,
                Message = feedbackDto.Message
            };

            await _feedbackService.CreateAsync(feedback);
            return CreatedAtAction(nameof(GetById), new { id = feedback.Id }, feedback);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _feedbackService.UpdateAsync(id, feedback);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _feedbackService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}

