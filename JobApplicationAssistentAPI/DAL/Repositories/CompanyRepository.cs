using DAL.Data;
using DAL.Models;


namespace DAL.Repositories
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
