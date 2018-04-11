using Nancy;
using Nancy.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NancyConsole.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            //主页
            Get["/"] = r =>
            {
                return Response.AsRedirect("/Home/Index");
            };

            //主页
            Get["/Home/Index"] = r =>
            {
                return View["index", "测试站点"];
            };

            ///桌面
            Get["/DestTop"] = r =>
            {
                return View["DestTop"];
            };

            Get["/Home/Download"] = r =>
            {
                string path = AppDomain.CurrentDomain.BaseDirectory+@"\Content\UpFile\使用说明.docx";
                if (!File.Exists(path))
                {
                    return Response.AsJson("文件不存在,可能已经被删除！");
                }
                var msbyte = default(byte[]);
                using (var memstream = new MemoryStream())
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        sr.BaseStream.CopyTo(memstream);
                    }
                    msbyte = memstream.ToArray();
                }

                return new Response()
                {
                    Contents = stream => { stream.Write(msbyte, 0, msbyte.Length); },
                    ContentType = "application/msword",
                    StatusCode = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string> {
                        { "Content-Disposition", string.Format("attachment;filename={0}", HttpUtility.UrlPathEncode(Path.GetFileName(path))) },
                        {"Content-Length",  msbyte.Length.ToString()}
                    }
                };
            };
        }
    }
}