/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2018-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 测试各种web api的提交方式
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//引入命名空间
using System.Web.Http;
using WebApiSubmit.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiSubmit.Controller
{
    public class FormTestController : System.Web.Mvc.Controller
    {

        /// <summary>
        /// 测试表单提交
        /// </summary>
        /// <param name="stu"></param>
        /// <returns></returns>
        [System.Web.Mvc.ValidateInput(false)]
        public HttpResponseBase PostTestFormData(string illexcelD)
        {
            try
            {                                            
                Response.Headers.Add("Content-Disposition", "attachment;filename=123.txt");                
                Response.Headers.Add("Content-Type", "application/octet-stream");
                FileStream fs = new FileStream(@"E:\123.txt", FileMode.Open);
                byte[] by = new byte[fs.Length];
                fs.Read(by, 0, by.Length);
                Response.Headers.Add("Content-Length", by.Length.ToString());
                Response.BinaryWrite(by);
                Response.Flush();
                Response.End();
                return Response;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}