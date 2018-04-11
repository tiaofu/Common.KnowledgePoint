using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citms.IllegalMonitor
{
    public class IllegalMonitorModel
    {
        public string Id { get; set; }
        public string SpottingId { get; set; }
        public string IllegalTypeNo { get; set; }
        public string LegalizeIllegalTypeNo { get; set; }      
        public decimal SvgC { get; set; }
        public decimal SvgH { get; set; }
        public decimal SvgL { get; set; }
        public decimal SvgV { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Remark { get; set; }
    }
}
