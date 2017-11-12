using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountInfo.Models
{
   
    public class CrmConnectionException : Exception
    {
        public CrmConnectionException(string message) : base(message)
        {

        }
    }
}
