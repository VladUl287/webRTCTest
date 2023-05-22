dotnet ef migrations add "InitialMigration" --project Web.Hub.Infrastructure --startup-project Web.Hub.Api --output-dir Migrations

dotnet ef database update --project Web.Hub.Infrastructure --startup-project Web.Hub.Api

dotnet ef migrations remove --project Web.Hub.Infrastructure --startup-project Web.Hub.Api