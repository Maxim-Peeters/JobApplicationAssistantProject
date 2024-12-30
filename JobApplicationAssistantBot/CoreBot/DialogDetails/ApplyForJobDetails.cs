using CoreBot.Models;

namespace CoreBot.DialogDetails
{
    public class ApplyForJobDetails
    {
        public string ResumeUrl { get; set; }
        public string CoverLetterUrl { get; set; }
        public string Notes { get; set; }
        public Job Job { get; set; }
    }
}
