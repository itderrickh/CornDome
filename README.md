# CardDweeb

CardDweeb is a web application for Adventure Time Card Wars with many features including a card database, building and sharing decks, and more.

## Features

### Card Database
CardDweeb has all the cards featured in the latest Card Wars Kickstarter releases. With ease of sorting and filtering you can find ANY card released to date.

### Deck Building & Management
Build decks using cards from the site's card collection and share with others.

### Community & Integrations
Connect with the Card Wars discord, view decks from other users, and arrange games with other Card Wars players!


## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Application language |
| ASP.NET Core Razor Pages | Web application and UI |
| Entity Framework Core | Database access and ORM |
| Relational database | SQLite for ease of hosting |
| Bootstrap | Responsive UI styling |
| JavaScript | Client-side interactions and deck utilities |
| Google Authentication | External sign-in |
| Nginx | Production reverse proxy |
| Linux / systemd | Production hosting and service management |

## Project Structure

The application is organized around Razor Pages, models, repositories, and shared application infrastructure.

```text
CornDome/
├── Pages/          # Razor Pages and page models
├── Controllers/    # The ongoing API development for dev access
├── Models/         # Entities and application models
├── Repository/     # Data access and repository implementations
├── Data/           # Database context and related data code
├── wwwroot/        # Static assets, CSS, and JavaScript
├── Program.cs      # Application configuration and startup
└── appsettings.json
```

*The tree is illustrative; exact directory names may differ in the repository.*

## Getting Started

### Prerequisites

- .NET SDK compatible with the application's target framework.
- Git.
- Google OAuth credentials if Google sign-in is enabled.
- Discord OAuth credentials if Discord sign-in is enabled.

### 1. Clone the repository

```bash
git clone <repository-url>
cd CornDome
```

Open the solution or project in your preferred IDE, such as JetBrains Rider or Visual Studio.

### 2. Configure the application

Configure the required settings for your local environment, including:

- Database connection string.
- Authentication credentials and Google OAuth settings, if applicable.
- Any external service credentials required by enabled integrations.

Use User Secrets or environment variables for development secrets rather than committing credentials to source control.

Example development configuration:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

The configuration keys above are examples. Use the actual keys expected by the application's configuration and `DbContext` setup.

### 3. Apply database migrations

From the directory containing the project file, run:

```bash
`dotnet ef database update --context TournamentContext --project CornDome.Repository --startup-project CornDome`
`dotnet ef database update --context MainContext --project CornDome.Repository --startup-project CornDome`
`dotnet ef database update --context CardDatabaseContext --project CornDome.Repository --startup-project CornDome`
```

If the Entity Framework Core tools are not installed, install them using:

```bash
dotnet tool install --global dotnet-ef
```

Use a tool version compatible with the project's EF Core dependencies.

### 4. Run the application

```bash
dotnet restore
dotnet build
dotnet run
```

Open the local URL printed in the console output.

## Architecture

CardDweeb uses a repository-oriented architecture with dependency injection.

- **Razor Pages** handle page rendering and page-specific request processing.
- **Repositories** encapsulate database operations and data access.
- **Entity Framework Core** manages database queries, entity tracking, and persistence.
- **Dependency injection** provides repositories and shared services to page models and other application components.
- **Authentication and authorization** protect user-specific functionality and restrict administrative operations.
- **Feature flags** allow selected functionality to be enabled or disabled without removing its implementation.

The application also supports API endpoints for functionality that benefits from HTTP-based requests.

## Configuration & Security

- Keep secrets and API credentials out of source control.
- Use HTTPS in production.
- Configure forwarded headers correctly when hosting behind Nginx.
- Protect state-changing requests against cross-site request forgery (CSRF).
- Enforce authorization on administrative pages and endpoints.
- Persist ASP.NET Core Data Protection keys across application restarts when using cookie-based authentication.
- Use appropriate database backups and access controls in production.

## Production Deployment

The production deployment uses a Linux host with Nginx acting as a reverse proxy to the ASP.NET Core application. The application runs as a systemd-managed service.

At a high level, deployment involves:

1. Publishing the application for the target runtime.
2. Deploying the published files to the server.
3. Configuring production environment variables and secrets.
4. Applying any required database migrations.
5. Configuring the systemd service.
6. Configuring Nginx to proxy requests to the application.
7. Verifying authentication, application health, and error logging.

Production-specific paths, service settings, database configuration, and reverse-proxy details should be maintained in deployment documentation rather than embedded as development defaults.

## Development

Typical development workflow:

```bash
dotnet restore
dotnet build
dotnet run
```

When changing database entities, create and apply an Entity Framework Core migration as appropriate:

```bash
`dotnet ef migrations add [Name] --context TournamentContext --project CornDome.Repository --startup-project CornDome`
`dotnet ef migrations add [Name] --context MainContext --project CornDome.Repository --startup-project CornDome`
```

Before submitting changes, verify that the application builds successfully and test the affected functionality.

## Contributing

Contributions and improvements are welcome.

1. Create a branch for your change.
2. Keep changes focused and follow the existing project conventions.
3. Test affected pages, database operations, and authorization behavior.
4. Submit a pull request describing the change and any required configuration or migration updates.



## TODO: Fill in the image source details later and move the following section
For the database:

`cd /appdata/CardWarsData`

`git pull`

For the google login:

`export Authentication__Google__ClientId="your-client-id"`

`export Authentication__Google__ClientSecret="your-client-secret"`

