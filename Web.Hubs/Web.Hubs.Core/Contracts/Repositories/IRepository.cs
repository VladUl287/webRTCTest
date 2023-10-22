namespace Web.Hubs.Core.Contracts.Repositories;

public interface IRepository
{
    Task<int> SaveChanges();
}
