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
    public class UserController : ApiController
    {
        //前台页面直接获取数据
        public List<Users> GetUser()
        {
            var userList = new List<Users> { 
            new Users{ Id=1,UName="张三",UAge=12,UAddress="海淀区"},
            new Users{Id=2,UName="李四",UAge=23,UAddress="昌平区"},
            new Users{Id=3,UName="王五",UAge=34,UAddress="朝阳区"}
            };

            var temp = (from u in userList
                        select u).ToList();
            return temp;
        }


        //前台发送Get请求，获取一个参数的情况
        //public string GetName(string name)
        //{
        //    return string.Format("姓名：{0}。", name);
        //}

        //前台发送Get请求，获取两个参数的情况
        public string GetName(string name, string age)
        {
            return string.Format("姓名：{0}，年龄：{1}。", name, age);
        }

        //前台发送无参Post请求
        public List<Users> Abc()
        {
            var userList = new List<Users> { 
            new Users{ Id=1,UName="张三1",UAge=12,UAddress="海淀区1"},
            new Users{Id=2,UName="李四2",UAge=23,UAddress="昌平区2"}
            };

            var temp = (from u in userList
                        select u).ToList();
            return temp;
        }


        // 前台发送 一个参数的Post请求
        public string Def([FromBody]string name)
        {
            return string.Format("姓名：{0}。", name);
        }


        public class Student
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        //前台发送 两个参数的Post请求
        public string Hig([FromBody]Student stu)
        {
            return string.Format("姓名：{0}，年龄：{1}。", stu.Name, stu.Age);
        }



        public class Album
        {
            public string AName { get; set; }
            public DateTime ADate { get; set; }
            public int ASize { get; set; }
            public bool ALock { get; set; }
        }

        //前台发送多个参数的Post请求
        public string Xyz([FromBody]Album album)
        {

            return string.Format("AName:{0},ADate:{1},ASzie:{2},ALock:{3}。", album.AName, album.ADate, album.ASize, album.ALock);

        }

        // 前台发送 多个对象的多个属性 post请求
        public string Mno([FromBody]JObject jdata)
        {
            dynamic json = jdata;
            JObject jstu = json.student;
            JObject jalbum = json.album;
            var stu = jstu.ToObject<Student>();
            var album = jalbum.ToObject<Album>();
            return string.Format("stu的name:{0},album的size:{1}。",stu.Name,album.ASize);
        }

        //前台页面指定输出类型的 post请求
        public Student Tolist([FromBody]Student stu)
        {
            return stu;
        }

        /// <summary>
        /// 测试表单提交
        /// HttpResponseMessage 为WebApi的反馈特性，可返回流文件
        /// 在Mvc中，Controller不具备适用HttpResponseMessage提交，指定的Response的Content-Type无效，可使用ActionResult返回来改变Content-Type,默认都是
        /// 
        /// </summary>
        /// <param name="stu"></param>
        /// <returns></returns>
        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public HttpResponseMessage PostTestFormData()
        {
            try
            {
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                HttpRequestBase request = context.Request;//定义传统request对象
                string name = request.Form["name"];                
                FileStream fs = new FileStream(@"E:\123.txt", FileMode.Open);
                MemoryStream ms = new MemoryStream();
                HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StreamContent(fs);
                response.Content.Headers.Add("Content-Disposition", "attachment;filename=123.txt");
                response.Content.Headers.Add("Content-Length", fs.Length.ToString());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = "Wep Api Demo File.zip"
                //};
                return response;
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);
            }
        }

    }
}