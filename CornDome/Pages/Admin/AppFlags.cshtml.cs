using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class AppFlagsModel : PageModel
    {
        private readonly IAppFlagRepository _appFlagRepository;

        public AppFlagsModel(IAppFlagRepository appFlagRepository)
        {
            _appFlagRepository = appFlagRepository;
        }

        public List<AppFlag> Flags { get; set; } = new();

        public void OnGet()
        {
            Flags = _appFlagRepository.GetAll();
        }

        public IActionResult OnPostToggleAsync(int id)
        {
            var flags = _appFlagRepository.GetAll();
            var flag = flags.FirstOrDefault(x => x.Id == id);

            if (flag == null)
            {
                return NotFound();
            }

            flag.Active = !flag.Active;

            _appFlagRepository.Update(flag);

            return RedirectToPage();
        }
    }
}