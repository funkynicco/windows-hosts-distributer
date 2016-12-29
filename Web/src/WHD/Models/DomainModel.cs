using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WHD.Models
{
    public class DomainModel
    {
        [Required]
        public int Store { get; set; }

        [Required]
        [RegularExpression(@"^([a-z0-9-_]+\.)*[a-z0-9-_]+$")]
        public string Domain { get; set; }

        [Required]
        [RegularExpression(@"^\d+\.\d+\.\d+\.\d+$")]
        public string IP { get; set; }

        [StringLength(512)]
        public string Description { get; set; }
    }
}
