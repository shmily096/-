using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace voice
{
   public static class ToolClass
    {
       
       public static HttpWebRequest getAccessRequest;
       //解决广达域问题并获取request
       public static HttpWebRequest getRequest(string getAccessUrl)
       {
           Uri url = new Uri(getAccessUrl);
           WebProxy proxy = WebProxy.GetDefaultProxy();
           proxy.UseDefaultCredentials = true;

           getAccessRequest = WebRequest.Create(url) as HttpWebRequest;
           getAccessRequest.AllowAutoRedirect = false;
           getAccessRequest.Proxy = proxy;
           return getAccessRequest;
       }
       //获取token值
       public static string getStrToken(string para_API_key, string para_API_secret_key)
       {

           //方法参数说明:
           //para_API_key:API_key(你的KEY)
           //para_API_secret_key(你的SECRRET_KEY)

           //方法返回值说明:
           //百度认证口令码,access_token
           string access_html = null;
           string access_token = null;
           string getAccessUrl = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials" +
          "&client_id=" + para_API_key + "&client_secret=" + para_API_secret_key;

           
           
           try
           {
               HttpWebRequest getAccessRequest = ToolClass.getRequest(getAccessUrl);
               getAccessRequest.Timeout = 30000;//30秒连接不成功就中断 
               getAccessRequest.Method = "post";

               HttpWebResponse response = getAccessRequest.GetResponse() as HttpWebResponse;
               using (StreamReader strHttpComback = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
               {
                   access_html = strHttpComback.ReadToEnd();
               }
           }
           catch (WebException ex)
           {
               MessageBox.Show(ex.ToString());
           }

           JObject jo = JObject.Parse(access_html);
           access_token = jo["access_token"].ToString();//得到返回的toke
           return access_token;
       }
       
       //比对解析出来的文字，浏览器打开对应的URL
       public static string openBrowser(string voiceStr)
       {
           //Boolean flag = false;
           string flag = "";
           string voiceStrL = voiceStr.ToLower();
           string xml = Application.StartupPath + "\\mapping.xml";
           XmlDocument xmlDoc = new XmlDocument();
           XmlReaderSettings settings = new XmlReaderSettings();
           settings.IgnoreComments = true;//忽略文档里面的注释
           XmlReader reader = XmlReader.Create(xml, settings);
           xmlDoc.Load(reader);
           XmlNode xn = xmlDoc.SelectSingleNode("voiceMap");
           XmlNodeList xnl = xn.ChildNodes;
           List<MapTable> _list = new List<MapTable>();
           foreach (XmlNode xnmode in xnl)
           {
               MapTable t = new MapTable();
               XmlElement xe = (XmlElement)xnmode;
               if (xe.Name == "type")
               {
                   t.key = xe.GetAttribute("key").ToString();
                   string getV = xe.GetAttribute("value").ToString();
                  string getv2= getV.Replace("+", "=");
                 string getv3=getv2.Replace("#", "&");
                 t.value = getv3;
                   _list.Add(t);
               }
           }
           foreach( MapTable m in _list){

               if (voiceStr != "" && voiceStrL.Contains(m.key.ToString().ToLower()))
               {
                   flag = m.key;

                   //if (voiceStrL.Contains("关闭"))
                   //{
                       
                   //    Application.Exit();
                   //}
                   //打开浏览器
                   Process.Start(m.value.ToString());
                   return flag;
               }
               

           }
           return flag;
       }

       
    }
}
