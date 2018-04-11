using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Security.Cryptography;

namespace Citms.DailyStatistics
{
    public class ConfigManage
    {
        static string Path = AppDomain.CurrentDomain.BaseDirectory + "\\config.js";
        public static SysConfig _sysConfig;
        public static SysConfig SysConfig
        {
            get
            {
                if (_sysConfig != null)
                {
                    return _sysConfig;
                }
                else
                {
                    if (!File.Exists(Path))
                        return null;
                    string result = File.ReadAllText(Path);
                    return JsonConvert.DeserializeObject<SysConfig>(result);
                }
            }
            set
            {
                _sysConfig = value;
                string result = JsonConvert.SerializeObject(value, Formatting.Indented);
                File.WriteAllText(Path, result);
            }
        }

        public static void Reload()
        {
            try
            {
                string result = File.ReadAllText(Path);
                _sysConfig = JsonConvert.DeserializeObject<SysConfig>(result);
                LogHelper.WriteInfo(string.Format("应用新的配置文件成功"));
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("应用新的配置文件异常，{0}", e.ToString()));
            }
        }
    }
    public class SysConfig
    {
        public string DbConn { get; set; }
        public string ApiAddress { get; set; }
        public decimal NoticePercent { get; set; }
        public decimal FloatingRange { get; set; }
    }
    public class UserTokenHelper
    {
        public static string GetToken()
        {
            string _imsToken = string.Empty;
            try
            {
                string sql = "SELECT APPKEY,UserGUID FROM Sys_Token WHERE  APPNAME='IMS' AND DISABLED=0";
                DataTable table = new DataTable();
                using (OracleConnection conn = new OracleConnection(ConfigManage.SysConfig.DbConn))
                {
                    OracleDataAdapter sda = new OracleDataAdapter(sql, conn);
                    sda.Fill(table);
                }
                if (table.Rows.Count > 0)
                {
                    string userGuid = table.Rows[0]["UserGUID"].ToString();
                    string appKey = table.Rows[0]["APPKEY"].ToString();
                    string token = JsonConvert.SerializeObject(new UserToken(userGuid, appKey));
                    return DESEncrypt.Encrypt(token);
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(
                           string.Format("违法检测-IMS Token获取异常，异常:{0}", e.ToString()));
            }
            return _imsToken;
        }
    }
    public class CommonInfo
    {
        public static List<Spotting> spottingList;
        public static Spotting GetSpottingName(string spottingId)
        {
            Spotting spo = null;
            if (spottingList == null)
            {
                try
                {
                    string sql = "select SpottingId,SpottingNo,SpottingName from common_spotting";
                    DataTable table = new DataTable();
                    using (OracleConnection conn = new OracleConnection(ConfigManage.SysConfig.DbConn))
                    {
                        OracleDataAdapter sda = new OracleDataAdapter(sql, conn);
                        sda.Fill(table);
                    }
                    spottingList = DataTableListHelper.ToList<Spotting>(table);
                }
                catch (Exception e)
                {
                    LogHelper.Error(string.Format("违法检测-获取路口异常，异常:{0}", e.ToString()));
                }
            }
            if (spottingList != null)
            {
                spo = spottingList.Where(p => p.SpottingId == spottingId).FirstOrDefault();
            }
            return spo;
        }
    }
    public class Spotting
    {
        public string SpottingId { get; set; }
        public string SpottingNo { get; set; }
        public string SpottingName { get; set; }
    }
    public class UserToken
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public string UserGUID { get; set; }

        /// <summary>
        /// 分配的密钥key
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 令牌生成时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="UserGUID">当前用户</param>
        public UserToken(string UserGUID, string AppKey)
        {
            this.UserGUID = UserGUID;
            Time = DateTime.Now;
            this.AppKey = AppKey;
        }

        public UserToken()
        {
        }
    }
    public class DESEncrypt
    {
        private static readonly string KEY = "Citms";

        #region ========加密======== 

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, KEY);
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            byte[] bKey = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.Key = bKey;
            des.IV = bKey;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ========解密======== 


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, KEY);
        }

        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            byte[] bKey = ASCIIEncoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.Key = bKey;
            des.IV = bKey;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        //// <summary>
        /// 取得MD5加密串
        /// </summary>
        /// <param name="input">源明文字符串</param>
        /// <returns>密文字符串</returns>
        public static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            string password = s.ToString();
            return password;
        }
    }
}