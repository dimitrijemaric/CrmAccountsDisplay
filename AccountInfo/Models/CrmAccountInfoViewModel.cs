using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountInfo.Models
{
    public class CrmAccountInfoViewModel
    {

        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountAddress { get; set; }
        public int ContactsCount { get; set; }
        public bool NoChildContacts { get; set; }
        public bool NoContactData { get; set; }

    }
}
