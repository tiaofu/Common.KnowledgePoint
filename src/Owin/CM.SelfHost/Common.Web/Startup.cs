using System.IO;
using Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Web
{
    class Startup
    {

        private static string _siteDir = @"Plugins\\Common.Web\\Html\\";       
        public void Configuration(IAppBuilder app)
        {
            // web api 接口  
            //HttpConfiguration config = InitWebApiConfig();
            //app.UseWebApi(config);

            //静态文件托管  
            app.Use((context, fun) =>
            {
                return myhandle(context, fun);
            });
        }
        /// <summary>  
        /// 路由初始化  
        /// </summary>  
        //public HttpConfiguration InitWebApiConfig()
        //{
        //    HttpConfiguration config = new HttpConfiguration();
        //    config.Routes.MapHttpRoute(
        //        name: "Default",
        //        routeTemplate: "api/{controller}/{action}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );
        //    config.Formatters
        //       .XmlFormatter.SupportedMediaTypes.Clear();
        //    //默认返回 json  
        //    config.Formatters
        //        .JsonFormatter.MediaTypeMappings.Add(
        //        new QueryStringMapping("datatype", "json", "application/json"));
        //    //返回格式选择  
        //    config.Formatters
        //        .XmlFormatter.MediaTypeMappings.Add(
        //        new QueryStringMapping("datatype", "xml", "application/xml"));
        //    //json 序列化设置  
        //    config.Formatters
        //        .JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
        //        {
        //            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        //        };
        //    return config;
        //}


        public Task myhandle(IOwinContext context, Func<Task> next)
        {
            //获取物理文件路径  
            var path = GetFilePath(context.Request.Path.Value);

            //验证路径是否存在  
            if (File.Exists(path))
            {
                return SetResponse(context, path);
            }            
            //不存在返回下一个请求  
            return next();
        }
        public static string GetFilePath(string relPath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,_siteDir,relPath.TrimStart('/').Replace('/', '\\'));
        }

        public Task SetResponse(IOwinContext context, string path)
        {
            var perfix = Path.GetExtension(path);
            if (perfix == ".html")
                context.Response.ContentType = "text/html; charset=utf-8";
            else if (perfix == ".js")
                context.Response.ContentType = "application/x-javascript";
            else if (perfix == ".css")
                context.Response.ContentType = "text/css";
            return context.Response.WriteAsync(File.ReadAllText(path));
        }
    }
}