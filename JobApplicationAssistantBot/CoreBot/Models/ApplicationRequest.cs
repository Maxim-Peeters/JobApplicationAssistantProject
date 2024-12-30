using static CoreBot.Models.Enums;

namespace CoreBot.Models
{
    public class ApplicationRequest
    {
        public ApplicationStatus Status { get; set; }
        public string ResumeUrl { get; set; }
        public string CoverLetterUrl { get; set; }
        public string Notes { get; set; }
        public int JobId { get; set; }
    }
}
