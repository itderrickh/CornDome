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
