using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Salary { get; set; }
        public string Location { get; set; }
        public JobType Type { get; set; }
        public ExperienceLevel RequiredExperience { get; set; }
        public int CompanyId { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsActive { get; set; }
        public List<string> RequiredSkills { get; set; }
        public Company Company { get; set; }
    }

}
