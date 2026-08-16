using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobProject.Models
{
    public class ContainerModel
    {
        [Required]
        public string Name { get; set; }
    }
}
