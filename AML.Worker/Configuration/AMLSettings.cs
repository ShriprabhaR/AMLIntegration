using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AML.Worker.Configuration
{
    public class AMLSettings
    {
        public bool UseMockAuth { get; set; }
        public string AuthUrl { get; set; } = string.Empty;
        public string ScreeningUrl { get; set; }= string.Empty; 
        public string EmpID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TenantName { get; set; }=string.Empty;
        public string CsrfToken { get; set; }= string.Empty;    
    }
}
