using CornDome.Repository.Main;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoticesController(INoticeRepository noticeRepository) : Controller
    {
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var notice = await noticeRepository.GetActiveNoticeAsync();

            return Ok(notice);
        }
    }
}
