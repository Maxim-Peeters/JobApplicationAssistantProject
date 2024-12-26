using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            var applications = await _unitOfWork.ApplicationRepository.GetAllAsync(query =>
                query.Include(a => a.Job)
                     .ThenInclude(j => j.Company));
            return Ok(applications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplication(int id)
        {
            var application = await _unitOfWork.ApplicationRepository.FindAsync(
                a => a.Id == id,
                include => include.Include(a => a.Job)
                                .ThenInclude(j => j.Company));

            if (application == null) return NotFound();
            return Ok(application);
        }
        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplication(Application application)
        {
            application.ApplicationDate = DateTime.Now;
            application.Status = ApplicationStatus.Submitted;

            await _unitOfWork.ApplicationRepository.InsertAsync(application);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication(int id, Application application)
        {
            if (id != application.Id) return BadRequest();
            await _unitOfWork.ApplicationRepository.UpdateAsync(application);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            await _unitOfWork.ApplicationRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

    }
}
