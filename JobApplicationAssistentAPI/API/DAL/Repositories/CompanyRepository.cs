using API.DAL.Data;
using API.DAL.Models;


namespace API.DAL.Repositories
{
    internal class CompanyRepository : GenericRepository<Company>
    {
        public readonly JobApplicationContext _context;
        public CompanyRepository(JobApplicationContext context) : base(context)
        {
            _context = context;
        }
    }
}
