using DAL.Data;
using DAL.Models;


namespace DAL.Repositories
{
    public class JobRepository : GenericRepository<Job>
    {
        public readonly JobApplicationContext _context;
        public JobRepository(JobApplicationContext context) : base(context)
        {
            _context = context;
        }
    }

}

