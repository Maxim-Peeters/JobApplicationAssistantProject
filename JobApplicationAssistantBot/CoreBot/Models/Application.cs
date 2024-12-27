using static CoreBot.Models.Enums;
using System;

namespace CoreBot.Models
{
    public class Application
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