using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Model.Data
{
    public class CreateVideoResponse
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public string VideoID { get; set; }
        public string Port { get; set; }
    }
}
