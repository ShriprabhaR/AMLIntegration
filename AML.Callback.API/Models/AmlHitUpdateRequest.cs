using System.ComponentModel.DataAnnotations;

namespace AML.Callback.API.Models
{
    public class AmlHitUpdateRequest
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public int AlertId { get; set; }

        [Required]
        public int ProscribedStatus { get; set; }

        [Required]
        public string EmpUsername { get; set; }

        public string FinalComments { get; set; }

        [Required]
        public string ModuleType { get; set; }
    }
}
