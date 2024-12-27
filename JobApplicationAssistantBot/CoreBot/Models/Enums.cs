namespace CoreBot.Models
{
    public class Enums
    {
        public enum JobType
        {
            FullTime,
            PartTime,
            Contract,
            Internship,
            Temporary
        }

        public enum ExperienceLevel
        {
            Entry,
            Junior,
            MidLevel,
            Senior,
            Executive
        }

        public enum ApplicationStatus
        {
            Submitted,
            UnderReview,
            InterviewScheduled,
            InterviewCompleted,
            Offered,
            Accepted,
            Rejected,
            Withdrawn
        }
    }
}
