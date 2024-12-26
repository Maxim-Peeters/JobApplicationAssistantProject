
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompaniesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync(query =>
                query.Include(c => c.Jobs));
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _unitOfWork.CompanyRepository.FindAsync(
                c => c.Id == id,
                include => include.Include(c => c.Jobs));

            if (company == null) return NotFound();
            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany(Company company)
        {
            await _unitOfWork.CompanyRepository.InsertAsync(company);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, Company company)
        {
            if (id != company.Id) return BadRequest();
            await _unitOfWork.CompanyRepository.UpdateAsync(company);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            await _unitOfWork.CompanyRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
