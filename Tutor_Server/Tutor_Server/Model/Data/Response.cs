using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor_Server.Model.Data
{
    public class Response
    {
        public ResponseType Type { get; set; }
        public Object Content { get; set; }
        public string Status { get; set; }
    }
}
