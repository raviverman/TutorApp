using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor_Server.Model.Data
{
    public class ProfileUpdateResponse
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public bool RequiresImageUpdate { get; set; }
    }
}
