using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AML.Shared.Models
{
    public class ScreeningResponse
    {    
            public string message { get; set; } = string.Empty;

            public int statusCode { get; set; }

            public List<ScreeningResult> results { get; set; } = new();
     }
}

