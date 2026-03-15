using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AML.Shared.Models
{
    public class ScreeningResult
    {
        public string customerId { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }
}
