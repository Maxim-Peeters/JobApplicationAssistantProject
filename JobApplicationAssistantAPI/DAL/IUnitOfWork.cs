using DAL.Models;
using DAL.Repositories;


namespace DAL
{
    public interface IUnitOfWork
    {
        IRepository<Job> JobRepository { get; }
        IRepository<Company> CompanyRepository { get; }
        IRepository<Application> ApplicationRepository { get; }
        Task SaveAsync();
    }
}
