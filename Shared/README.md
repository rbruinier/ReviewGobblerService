#

## Database

See: https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

### Create initial database

In Shared folder:

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Upload database to server

In root of solution: ```./uploadDatabase.sh```


### Scrape Pitchfork


