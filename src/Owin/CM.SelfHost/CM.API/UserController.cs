using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CM.API
{
    public class UserController: ApiController
    {
        [HttpPost,HttpGet]
        public string PostGetInfo()
        {
            return "API测试地址";
        }
    }
}
