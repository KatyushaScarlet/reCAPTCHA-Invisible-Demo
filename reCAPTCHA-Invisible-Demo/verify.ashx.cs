using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace reCAPTCHA_Invisible_Demo
{
    /// <summary>
    /// verify 的摘要说明
    /// </summary>
    public class verify : IHttpHandler
    {
        public class RequestData
        {
            public string secret;
            public string response;
        }

        public void ProcessRequest(HttpContext context)
        {
            RequestData requestData = new RequestData();
            requestData.secret = "6LdaK0UUAAAAAFBdKejyhkMBXGxdxTDvpE2IT41t";//站点secret key
            requestData.response = context.Request.Form["code"];//客户端返回结果
            if (requestData.response != "")
            {
                string postUrl = "https://recaptcha.net/recaptcha/api/siteverify?secret=" + requestData.secret + "&response=" + requestData.response;
                string postResult = HttpPost(postUrl);//发送
                Debug.WriteLine(postResult);
                JObject jObject = JsonConvert.DeserializeObject(postResult) as JObject;//反序列化验证结果
                if (jObject["success"].ToString().ToLower() == "true")
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Write("{\"status\":\"true\"}");
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Write("{\"status\":\"false\"}");
                }
            }
            else
            {
                context.Response.ContentType = "application/json";
                context.Response.Write("{\"status\":\"false\"}");
            }
        }

        public static string HttpPost(string url, string postDataStr = "")//模拟POST请求
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}