using API.DAL.Models;

namespace API.DAL.Dto
{
    public class ApplicationResponse
    {
        public int Id { get; set; }
        public DateTime ApplicationDate { get; set; }
        public ApplicationStatus Status { get; set; }
        public string ResumeUrl { get; set; }
        public string CoverLetterUrl { get; set; }
        public string Notes { get; set; }
        public int JobId { get; set; }
    }
}
