using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WHD.Models
{
    public class AddOrUpdateStoreModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ,.!#()\[\]?-]{1,128}$")]
        public string Name { get; set; }
    }
}
