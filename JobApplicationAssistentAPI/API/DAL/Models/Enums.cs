using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Models
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