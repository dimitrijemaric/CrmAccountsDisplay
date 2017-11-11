using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace AccountInfo.Models
{
    public class CrmContactInfoViewModel
    {
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public Guid ContactId { get; set; }
        public bool isPrimary { get; set; }

    }
}
