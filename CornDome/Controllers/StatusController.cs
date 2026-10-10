using CornDome.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AppFlag("EnableDeveloperApi")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new {
                status = "Ok"
            });
        }
    }
}
