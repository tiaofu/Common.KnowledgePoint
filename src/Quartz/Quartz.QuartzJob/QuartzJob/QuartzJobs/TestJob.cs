/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2017-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *********************************************************/
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJob.QuartzJobs
{
    public sealed class TestJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                CopyIllegalDataWork.HandlerData();
            }
            catch (Exception e)
            {
                _logger.Error("处理异常：" + e.ToString());
            }
        }
    }
}
