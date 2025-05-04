# Card Dweeb website

README.md in progress. Please check back later

## Deployment Steps

For the website:

`cd /app/CornDome`

`git pull`

`systemctl stop carddweeb.service`

`dotnet publish`

`systemctl start carddweeb.service`

For the database:

`cd /appdata/CardWarsData`

`git pull`

For the google login:

`export Authentication__Google__ClientId="your-client-id"`

`export Authentication__Google__ClientSecret="your-client-secret"`

For the migrations:

`dotnet ef migrations add [Name] --context TournamentContext --project CornDome.Repository --startup-project CornDome`
`dotnet ef database update --context TournamentContext --project CornDome.Repository --startup-project CornDome`

`dotnet ef database update --context CardDatabaseContext --project CornDome.Repository --startup-project CornDome`