using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using Inventory_Management_Sustem.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IReviewsService _reviewsService;

        public ReviewsController(AppDbContext context, IReviewsService reviewsService)
        {
            _context = context;
            _reviewsService = reviewsService;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            return await _reviewsService.GetReviews();
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _reviewsService.GetReviewByIDAsync(id);

            if (review == null) return NotFound();

            return review;
        }

        // POST: api/Reviews
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> PostReview([FromBody] ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList()
                });
            }

            await _reviewsService.AddNewReviewAsync(model);

            return CreatedAtAction(nameof(GetReview), new { id = model.Id });
        }

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> PutReview(int id, ReviewUpdateViewModel model)
        {
            if (id != model.Id) return BadRequest();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            await _reviewsService.UpdateReviewAsync(id, model);
            return NoContent();
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            await _reviewsService.DeleteReviewAsync(id);

            return NoContent();
        }
    }
}

