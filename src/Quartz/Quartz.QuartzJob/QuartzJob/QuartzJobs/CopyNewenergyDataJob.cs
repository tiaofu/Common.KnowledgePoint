/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2017-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Quartz;

namespace QuartzJob.QuartzJobs
{
    /// <summary>
    /// 新能源车赋值服务
    /// </summary>
    public class CopyNewenergyDataJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            CopyNewenergyData.HandlerData();
        }
    }
}
