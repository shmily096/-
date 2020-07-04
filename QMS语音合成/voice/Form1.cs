using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using Baidu.Aip.Speech;
using System.Runtime.InteropServices;


namespace voice
{
    public partial class Form1 : Form
    {


        string cuid = "15681764";//可以随便写
        static string API_KEY = "n5tuQWPk8x9OBdmmYhx3p10g";
        static string SECRET_KEY = "TUpmNjfGVhyaUz24EneA3vftIv5lKCXD";
        string token = ToolClass.getStrToken(API_KEY, SECRET_KEY);
        //百度语音合成
        string PlayUrl = "http://tsn.baidu.com/text2audio?";

        public Form1()
        {
            InitializeComponent();
            cuid = Guid.NewGuid().ToString();
         
        }



        private const string lan = "zh";//语言
        private const string per = "0";//发音人选择 0位女  1位男  默认 女0为女声，1为男声，3为情感合成-度逍遥，4为情感合成-度丫丫，默认为普通女声
        private const string ctp = "1";//客户端类型选择 web端为1  
        private const string spd = "4";//范围0~9  默认 5   语速
        private const string pit = "5";//范围0~9  默认 5   音调
        private const string vol = "5";//范围0~9  默认 5   音量

        private const string rest = "tex={0}&lan={1}&per={2}&ctp={3}&cuid={4}&tok={5}&spd={6}&pit={7}&vol={8}";
        private const int NULL = 0, ERROR_SUCCESS = NULL;
        [DllImport("WinMm.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback); 
        private void button1_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            mciSendString("close app", "", 0, 0);
            string PlayUrlz = string.Format(rest, txt, lan, per, ctp, cuid, token, spd, pit, vol);

            HttpWebRequest req = ToolClass.getRequest("http://tsn.baidu.com/text2audio");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = Encoding.UTF8.GetByteCount(PlayUrlz);
            using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
            {

                sw.Write(PlayUrlz);
                //sw.Close();
            }


            HttpWebResponse res = req.GetResponse() as HttpWebResponse;
            using (Stream stream = res.GetResponseStream())
            {
                string strFullFileName = Application.StartupPath + "/app.mp3";
                using (FileStream fs = new FileStream(strFullFileName, FileMode.Truncate | FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.CopyTo(fs);
                    //stream.Close();
                }

                if (mciSendString(string.Format("open \"{0}\" alias app", strFullFileName), null, NULL, NULL) == ERROR_SUCCESS)
                {
                    mciSendString("play app", null, NULL, NULL);

                }


            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        Boolean pause = true;
        private void button3_Click(object sender, EventArgs e)
        {

            if (pause)
            {
                mciSendString("pause app", null, NULL, NULL);
                pause = false;
            }
            else {

                mciSendString("resume app", null, NULL, NULL);
                pause = true;
            }
            
        }

      
    }
}