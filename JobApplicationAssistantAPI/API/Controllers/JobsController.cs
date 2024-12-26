using DAL.Models;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DAL.Dto;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: http://.../Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobResponse>>> GetJobs()
        {
            var jobs = await _unitOfWork.JobRepository.GetAllAsync(query =>
                query.Include(j => j.Company));

            var jobsDtos = _mapper.Map<IEnumerable<JobResponse>>(jobs);
            return Ok(jobsDtos);
        }

        // GET: http://.../Jobs/by-id/{id}
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<Job>> GetJobById(int id)
        {
            var job = await _unitOfWork.JobRepository.FindAsync(
                j => j.Id == id,
                include => include.Include(j => j.Company));

            if (job == null) return NotFound();

            var jobDto = _mapper.Map<Job>(job);
            return Ok(jobDto);
        }

        // POST: http://.../Jobs/create
        [HttpPost("create")]
        public async Task<ActionResult<JobResponse>> CreateJob(JobRequest jobRequest)
        {
            var job = _mapper.Map<Job>(jobRequest);

            job.PostedDate = DateTime.Now;

            await _unitOfWork.JobRepository.InsertAsync(job);
            await _unitOfWork.SaveAsync();

            var jobResponse = _mapper.Map<JobResponse>(job);
            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, jobResponse);

        }

        // PUT: http://.../Jobs/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobRequest jobRequest)
        {
            var existingJob = await _unitOfWork.JobRepository.GetByIDAsync(id);
            if (existingJob == null) return NotFound("Job not found.");

            _mapper.Map(jobRequest, existingJob);

            await _unitOfWork.JobRepository.UpdateAsync(existingJob);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: http://.../Jobs/remove/{id}
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveJob(int id)
        {
            var existingJob = await _unitOfWork.JobRepository.GetByIDAsync(id);
            if (existingJob == null) return NotFound("Job not found.");

            await _unitOfWork.JobRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

    }
}
