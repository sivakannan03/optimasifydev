using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace PfizerEntity
{
  public  class Acknowledgement
    {
        public string PKID { get; set; }
        public string AllocatedQty { get; set; }
        public string AcknowledgeQty { get; set; }
        public string Pendingack { get; set; }
        public string AckQty { get; set; }
        public string ReasonForShortfall { get; set; }
        
    }
  
}
