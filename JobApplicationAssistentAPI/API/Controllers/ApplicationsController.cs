using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DAL.Dto;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApplicationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: http://.../Applications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetApplications()
        {
            var applications = await _unitOfWork.ApplicationRepository.GetAllAsync(
                query => query.Include(a => a.Job)
                              .ThenInclude(j => j.Company));

            var applicationsDtos = _mapper.Map<IEnumerable<ApplicationResponse>>(applications);
            return Ok(applicationsDtos);
        }

        // GET: http://.../Applications/by-id/{id}
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<Application>> GetApplicationById(int id)
        {
            var application = await _unitOfWork.ApplicationRepository.FindAsync(
                a => a.Id == id,
                include => include.Include(a => a.Job)
                                .ThenInclude(j => j.Company));

            if (application == null) return NotFound();

            var applicationDto = _mapper.Map<ApplicationResponse>(application);
            return Ok(applicationDto);
        }

        // POST: http://.../Applications/create
        [HttpPost("create")]
        public async Task<ActionResult<ApplicationRequest>> CreateApplication(ApplicationRequest applicationRequest)
        {
            applicationRequest.Status = ApplicationStatus.Submitted;

            var application = _mapper.Map<Application>(applicationRequest);
            application.ApplicationDate = DateTime.Now;

            await _unitOfWork.ApplicationRepository.InsertAsync(application);
            await _unitOfWork.SaveAsync();

            var appplicationResponse = _mapper.Map<ApplicationResponse>(application);
            return CreatedAtAction(nameof(GetApplicationById), new { id = application.Id }, application);
        }

        // PUT: http://.../Applications/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] ApplicationRequest applicationRequest)
        {
            var existingApplication = await _unitOfWork.ApplicationRepository.GetByIDAsync(id);
            if (existingApplication == null) return NotFound("Application not found.");

            _mapper.Map(applicationRequest, existingApplication);

            await _unitOfWork.ApplicationRepository.UpdateAsync(existingApplication);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: http://.../Applications/remove/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveApplication(int id)
        {
            var existingApplication = await _unitOfWork.ApplicationRepository.GetByIDAsync(id);
            if (existingApplication == null) return NotFound("Application not found.");

            await _unitOfWork.ApplicationRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

    }
}
