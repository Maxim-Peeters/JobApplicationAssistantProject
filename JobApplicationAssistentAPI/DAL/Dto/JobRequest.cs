using DAL.Models;

namespace DAL.Dto
{
    public class JobRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Salary { get; set; }
        public string Location { get; set; }
        public JobType Type { get; set; }
        public ExperienceLevel RequiredExperience { get; set; }
        public bool IsActive { get; set; }
        public List<string> RequiredSkills { get; set; }

        public int CompanyId { get; set; }
    }
}
