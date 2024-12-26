
using AutoMapper;
using DAL;
using DAL.Dto;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompaniesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: http://.../Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyResponse>>> GetCompanies()
        {
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync(query =>
                query.Include(c => c.Jobs));

            var companiesDto = _mapper.Map<IEnumerable<CompanyResponse>>(companies);
            return Ok(companies);
        }

        // GET: http://.../Companies/{id}
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<CompanyResponse>> GetCompanyById(int id)
        {
            var company = await _unitOfWork.CompanyRepository.FindAsync(
                c => c.Id == id,
                include => include.Include(c => c.Jobs));

            if (company == null) return NotFound();

            var companyResponse = _mapper.Map<CompanyResponse>(company);
            return Ok(companyResponse);
        }

        // POST: http://.../Companies
        [HttpPost("create")]
        public async Task<ActionResult<CompanyResponse>> CreateCompany(CompanyRequest companyRequest)
        {
            var company = _mapper.Map<Company>(companyRequest);

            await _unitOfWork.CompanyRepository.InsertAsync(company);
            await _unitOfWork.SaveAsync();

            var companyResponse = _mapper.Map<CompanyResponse>(company);
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, companyResponse);
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
