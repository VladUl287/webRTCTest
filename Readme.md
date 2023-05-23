dotnet tool install [--global] dotnet-ef --version 7.*

dotnet ef migrations add "InitialMigration" --project Web.Hubs.Infrastructure --startup-project Web.Hubs.Api --output-dir Migrations

dotnet ef database update --project Web.Hubs.Infrastructure --startup-project Web.Hubs.Api

dotnet ef migrations remove --project Web.Hubs.Infrastructure --startup-project Web.Hubs.Api