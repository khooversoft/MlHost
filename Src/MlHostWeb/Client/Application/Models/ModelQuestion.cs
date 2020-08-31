using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Application.Models
{
    public class ModelQuestion
    {
        [Required]
        public string Question { get; set; }
        public string Result { get; set; }
    }
}
