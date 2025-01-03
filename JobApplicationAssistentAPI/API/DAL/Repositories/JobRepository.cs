using API.DAL.Data;
using API.DAL.Models;


namespace API.DAL.Repositories
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

