using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    class Request
    {
        public string Username { get; set; }
        public string SessionId { get; set; }
        public RequestType Type { get; set; }
        public Object Content { get; set; }
    }
}
