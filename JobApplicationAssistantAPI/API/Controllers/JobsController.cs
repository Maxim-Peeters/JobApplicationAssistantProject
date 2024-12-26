using DAL.Models;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            var jobs = await _unitOfWork.JobRepository.GetAllAsync(query =>
                query.Include(j => j.Company));
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _unitOfWork.JobRepository.FindAsync(
                j => j.Id == id,
                include => include.Include(j => j.Company));

            if (job == null) return NotFound();
            return Ok(job);
        }

        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob(Job job)
        {
            await _unitOfWork.JobRepository.InsertAsync(job);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, Job job)
        {
            if (id != job.Id) return BadRequest();
            await _unitOfWork.JobRepository.UpdateAsync(job);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            await _unitOfWork.JobRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
