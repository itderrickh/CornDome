using CornDome.Models;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class AppFlagsModel(IAppFlagRepository appFlagRepository) : BasePageModel
    {
        public List<AppFlag> Flags { get; set; } = [];

        public void OnGet()
        {
            Flags = appFlagRepository.GetAll();
        }

        public IActionResult OnPostToggleAsync(int id)
        {
            var flags = appFlagRepository.GetAll();
            var flag = flags.FirstOrDefault(x => x.Id == id);

            if (flag == null)
            {
                return NotFound();
            }

            flag.Active = !flag.Active;

            appFlagRepository.Update(flag);

            return RedirectToPage();
        }
    }
}