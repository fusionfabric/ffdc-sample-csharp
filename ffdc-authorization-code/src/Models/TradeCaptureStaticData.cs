using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffdc_authorization_code.Models
{
    public class TradeCaptureStaticData
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public string[] ApplicableEntities { get; set; }

    }
}
