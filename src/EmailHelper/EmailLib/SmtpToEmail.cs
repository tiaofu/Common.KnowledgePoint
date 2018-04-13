/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/13 16:39:48
 * Modify: 2018/4/13 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *     Email send by api  
 *     smtp 方式发送邮件，比较普遍的就是163，qq,提供常规操作
 *     smmtp方式只要支持smtp的邮箱服务器都可以
 *     区别在于163，qq目前开启了授权码，不再使用密码登录，还有就是邮件内容的检查更严格了,发送普通邮件没有问题
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailLib
{
    public class SmtpToEmail
    {
        /// <summary>
        /// SMTP客户端实例
        /// </summary>
        private static System.Net.Mail.SmtpClient client = null;

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="Receiver">邮件接收人</param>
        /// <param name="Subject">邮件主题</param>
        /// <param name="content">邮件内容</param>
        /// <returns>发送状态</returns>
        public static MessageCode SendMessage(string Receiver, string Subject, string content)
        {
            if (string.IsNullOrEmpty(Receiver) || string.IsNullOrEmpty(Subject)
                || string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("SendMessage参数空异常！");
            }
            if (client == null)
            {
                try
                {
                    //163发送配置 还是采用原始的邮箱账号密码方式，所有考虑这里才会UseDefaultCredentials配置为true
                    //UseDefaultCredentials 取得或設定 Boolean 值，控制 DefaultCredentials 與要求一起傳送。         
                    client = new System.Net.Mail.SmtpClient();
                    client.Host = "smtp.21cn.com";
                    client.Port = 25;
                    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;


                    //qq发送配置的参数
                    //qq邮箱现在使用了授权码的方式进行验证，而不再是邮箱密码，所有考虑这里的UseDefaultCredentials需要设置为false
                    //切EnableSsl必须设置为true  
                    //client = new System.Net.Mail.SmtpClient();
                    //client.Host = "smtp.qq.com";
                    //client.Port = 25;
                    //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    //client.EnableSsl = true;
                    //client.UseDefaultCredentials = false;

                    client.Credentials = new System.Net.NetworkCredential("xxxx@21cn.com", "xxxx");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            try
            {
                System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
                Message.SubjectEncoding = System.Text.Encoding.UTF8;
                Message.BodyEncoding = System.Text.Encoding.UTF8;
                Message.Priority = System.Net.Mail.MailPriority.High;

                Message.From = new System.Net.Mail.MailAddress("xxxx@21cn.com", "张三");
                //添加邮件接收人地址
                string[] receivers = Receiver.Split(new char[] { ',' });
                Array.ForEach(receivers.ToArray(), ToMail => { Message.To.Add(ToMail); });
                
                Message.Subject = Subject;
                Message.Body = content;
                Message.IsBodyHtml = true;
                client.Send(Message);
                return MessageCode.Success;
            }
            catch (Exception ex)
            {
                return MessageCode.Exception;
            }
        }
    }
}
