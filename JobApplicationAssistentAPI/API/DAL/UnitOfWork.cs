using API.DAL.Data;
using API.DAL.Models;
using API.DAL.Repositories;

namespace API.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly JobApplicationContext _context;
        private IRepository<Job> jobRepository;
        private IRepository<Company> companyRepository;
        private IRepository<Application> applicationRepository;

        public UnitOfWork(JobApplicationContext context)
        {
            _context = context;
        }

        public IRepository<Job> JobRepository
        {
            get
            {
                jobRepository ??= new GenericRepository<Job>(_context);
                return jobRepository;
            }
        }

        public IRepository<Company> CompanyRepository
        {
            get
            {
                companyRepository ??= new GenericRepository<Company>(_context);
                return companyRepository;
            }
        }

        public IRepository<Application> ApplicationRepository
        {
            get
            {
                applicationRepository ??= new GenericRepository<Application>(_context);
                return applicationRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
