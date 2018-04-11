using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citms.DailyStatistics
{
    public sealed class DailyStatisticsJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DailyStatisticsJob));    
        public void Execute(IJobExecutionContext context)
        {
            DailyStatisticsWork.HandlerData();
        }
    }
}
