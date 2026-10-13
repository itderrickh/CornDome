using CornDome.Helpers;
using CornDome.Models;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CornDome.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequireUserAgent]
    public class CardsController(ICardRepository cardRepository) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetCards([FromQuery] int limit = ApiPagination.DefaultLimit, [FromQuery] int offset = ApiPagination.DefaultOffset)
        {
            if (limit < 1 || limit > ApiPagination.MaxLimit)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid limit",
                    Detail = $"Limit must be between 1 and {ApiPagination.MaxLimit}.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (offset < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid offset",
                    Detail = "Offset must be zero or greater.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var stopwatch = Stopwatch.StartNew();
            var total = await cardRepository.GetCount();
            var cards = await cardRepository.GetCards(limit, offset);
            stopwatch.Stop();

            var response = new ApiCollectionResponse<object>
            {
                Data = cards.Cast<object>().ToList(),
                Elapsed = stopwatch.Elapsed.TotalMilliseconds,
                Next = ApiPagination.GetNextUrl(
                    Request, total, limit, offset),
                Total = total,
                Count = cards.Count(),
                Limit = limit,
                Offset = offset
            };

            return Ok(response);
        }

        [HttpGet("sets")]
        public IActionResult GetSets()
        {
            var stopwatch = Stopwatch.StartNew();
            var sets = cardRepository.GetAllSets();
            stopwatch.Stop();

            var response = new ApiCollectionResponse<object>
            {
                Data = sets.Cast<object>().ToList(),
                Elapsed = stopwatch.Elapsed.TotalMilliseconds,
                Next = null,
                Total = sets.Count(),
                Count = sets.Count(),
                Limit = sets.Count(),
                Offset = 0
            };

            return Ok(response);
        }

        [HttpGet("landscapes")]
        public IActionResult GetLandscapes()
        {
            var stopwatch = Stopwatch.StartNew();
            var landscapes = cardRepository.GetAllLandscapes();
            stopwatch.Stop();

            var response = new ApiCollectionResponse<object>
            {
                Data = landscapes.Cast<object>().ToList(),
                Elapsed = stopwatch.Elapsed.TotalMilliseconds,
                Next = null,
                Total = landscapes.Count(),
                Count = landscapes.Count(),
                Limit = landscapes.Count(),
                Offset = 0
            };

            return Ok(response);
        }

        [HttpGet("cardtypes")]
        public IActionResult GetCardTypes()
        {
            var stopwatch = Stopwatch.StartNew();
            var cardTypes = cardRepository.GetAllCardTypes();
            stopwatch.Stop();

            var response = new ApiCollectionResponse<object>
            {
                Data = cardTypes.Cast<object>().ToList(),
                Elapsed = stopwatch.Elapsed.TotalMilliseconds,
                Next = null,
                Total = cardTypes.Count(),
                Count = cardTypes.Count(),
                Limit = cardTypes.Count(),
                Offset = 0
            };

            return Ok(response);
        }
    }
}
