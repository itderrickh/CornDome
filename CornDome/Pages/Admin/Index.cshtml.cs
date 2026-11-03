using CornDome.Models;
using Microsoft.AspNetCore.Authorization;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "editor")]
    public class IndexModel : BasePageModel
    {
        public List<AdminArea> AdminAreas { get; set; } = [];

        private List<AdminArea> SetupAreas()
        {
            var adminAreas = new List<AdminArea>();

            if (User.IsInRole("CardManager") || User.IsInRole("Admin"))
            {
                adminAreas.Add(new AdminArea()
                {
                    Name = "Card Management",
                    Description = "Manage the card database. This feature allows full control of every card hosted on CardDweeb.",
                    Icon = "bi-card-text",
                    Area = "/CardManage/Index",
                    LinkText = "Manage Cards"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Card Feedback",
                    Description = "Review the user submitted feedback about cards. It is important to review this frequently.",
                    Icon = "bi-card-checklist",
                    Area = "/CardManage/Feedback",
                    LinkText = "Review Feedback"
                });
            }

            if (User.IsInRole("RulingManager") || User.IsInRole("Admin"))
            {

            }

            if (User.IsInRole("Admin"))
            {
                adminAreas.Add(new AdminArea()
                {
                    Name = "User Management",
                    Description = "Manage user accounts, review user details, and administer access to the application.",
                    Icon = "bi-people-fill",
                    Area = "/Admin/UserManagement",
                    LinkText = "Manage users"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Role Management",
                    Description = "Manage application roles and permissions by creating new roles or deleting existing ones.",
                    Icon = "bi-shield-lock",
                    Area = "/Admin/RoleManagement",
                    LinkText = "Manage roles"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "App Flags",
                    Description = "Enable or disable features without deploying application code.",
                    Icon = "bi-toggles",
                    Area = "/Admin/AppFlags",
                    LinkText = "Manage Flags"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Set Management",
                    Description = "Add, edit, and delete card sets used throughout the card database.",
                    Icon = "bi-collection",
                    Area = "/Admin/ManageCardSets",
                    LinkText = "Manage Sets"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Bug Management",
                    Description = "Review reported issues, investigate problems, and track bug reports.",
                    Icon = "bi-bug-fill",
                    Area = "/Admin/BugManagement",
                    LinkText = "Review Bugs"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Error Logs",
                    Description = "View error logs from CardDweeb logging.",
                    Icon = "bi-journal-x",
                    Area = "/Admin/ErrorLogs",
                    LinkText = "View Logs"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Database backup",
                    Description = "Create and manage database backups to protect application data.",
                    Icon = "bi-database-fill-check",
                    Area = "/Admin/DatabaseBackup",
                    LinkText = "Backup Now"
                });

                adminAreas.Add(new AdminArea()
                {
                    Name = "Database backup",
                    Description = "Restore application data from a backup.",
                    Icon = "bi-database-fill-gear",
                    Area = "/Admin/DatabaseRestore",
                    LinkText = "Open restore tools"
                });
            }
            

            return adminAreas;
        }

        public async void OnGet()
        {
            AdminAreas = SetupAreas();
        }
    }

    public class AdminArea
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Area { get; set; }
        public string LinkText { get; set; }
    }
}
