using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AML.Shared.Models
{
    public class RelatedParty
    {
        public string relatedPartyKey { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int entityId { get; set; }
        public string category { get; set; } = string.Empty;
        public int yearOfBirth { get; set; }

    }
}
