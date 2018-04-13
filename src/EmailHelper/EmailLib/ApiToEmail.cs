/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/13 16:39:48
 * Modify: 2018/4/13 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *     Email send by api
 *     邮件服务器提供相关的api方式，这里使用的是 sendcloud 平台的，当然支持Smtp方式
 *     支持匿名发送邮件 XX 当然这个功能可以通过自己搭建邮件服务器或者自己实现smtp来实现
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.IO;

namespace EmailLib
{
    public class ApiToEmail
    {
        private static StreamContent createStream(String filePath, String paramKey, String fileName)
        {
            FileStream fs = File.OpenRead(filePath);
            StreamContent streamContent = new StreamContent(fs);
            streamContent.Headers.Add("Content-Type", "application/octet-stream");
            String headerValue = "form-data; name=\"" + paramKey + "\"; filename=\"" + fileName + "\"";
            byte[] bytes1 = Encoding.UTF8.GetBytes(headerValue);
            headerValue = "";
            foreach (byte b1 in bytes1)
            {
                headerValue += (Char)b1;
            }
            streamContent.Headers.Add("Content-Disposition", headerValue);
            return streamContent;
        }

        public static MessageCode Send(string tos)
        {
            var result = MessageCode.Exception;
            String url = "http://api.sendcloud.net/apiv2/mail/send";
            HttpClient client = null;
            HttpResponseMessage response = null;
            try
            {
                client = new HttpClient();
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("apiUser", "xxx"));
                paramList.Add(new KeyValuePair<string, string>("apiKey", "xxx"));
                paramList.Add(new KeyValuePair<string, string>("from", "123456@163.com"));
                paramList.Add(new KeyValuePair<string, string>("fromName", "张三"));
                paramList.Add(new KeyValuePair<string, string>("to", tos));
                paramList.Add(new KeyValuePair<string, string>("subject", "测试邮件"));
                paramList.Add(new KeyValuePair<string, string>("html", "测试邮件"));

                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var keyValuePair in paramList)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value), String.Format("\"{0}\"", keyValuePair.Key));
                }

                multipartFormDataContent.Add(createStream(@"I:\Email\attachments\1.txt", "attachments", "附件名称2.txt"));

                multipartFormDataContent.Add(createStream(@"I:\Email\attachments\2.txt", "attachments", "附件名称1.txt"));

                response = client.PostAsync(url, multipartFormDataContent).Result;
                string apiresult = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = MessageCode.Success;
                }
            }
            catch (Exception e)
            {
                result = MessageCode.Exception;
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
            return result;
        }
    }
}
