using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Http;

namespace CM.ApiArea.Areas.Test2.Controllers
{
    public class Index2Controller: ApiController
    {
        public string GetInfo()
        {
            return "Test2.Index.GetInfo";
        }
    }
}
