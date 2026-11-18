using CornDome.Models;
using Microsoft.AspNetCore.Authorization;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class QueryTesterModel : BasePageModel
    {
        public void OnGet()
        {
        }
    }
}
