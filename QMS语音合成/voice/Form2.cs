using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace voice
{
    public partial class Form2 : Form
    {
        string cuid = "15681764";//可以随便写
      static  string API_KEY = "n5tuQWPk8x9OBdmmYhx3p10g";
      static string SECRET_KEY = "TUpmNjfGVhyaUz24EneA3vftIv5lKCXD";



        string token = ToolClass.getStrToken(API_KEY, SECRET_KEY);
        //百度语音识别
        string serverURL = "http://vop.baidu.com/server_api";
        //百度语音合成
        string PlayUrl = "http://tsn.baidu.com/text2audio?";
        public Form2()
        {
            InitializeComponent();
            cuid = Guid.NewGuid().ToString();
            //Play("广达语音助手为您服务！");
            Play("汉皇重色思倾国,御宇多年求不得.杨家有女初长成,养在深闺人未识.天生丽质难自弃,一朝选在君王侧.回眸一笑百媚生,六宫粉黛无颜色.春寒赐浴华清池,温泉水滑洗凝脂.侍儿扶起娇无力,始是新承恩泽时.云鬓花颜金步摇,芙蓉帐暖度春宵.春宵苦短日高起,从此君王不早朝.");
        }
        private SoundRecord recorder;
       
        private void Btn_Down(object sender, MouseEventArgs e)
        {
            
            button1.Text = "录音中...";
            button1.BackColor=Color.FromArgb(255, 120, 255);
            textBox1.Visible = false;
            recorder = new SoundRecord();
            try
            {
                recorder.InitCaptureDevice();
            }catch(NullReferenceException) { }
            try
            {
                recorder.mWavFormat = recorder.CreateWaveFormat();
            }
            catch (NullReferenceException) { }
            
 
            //  
            // 录音设置  
            //  
            string wavfile;
            wavfile = "test.wav";
            //if (recorder == null)
            //{
            //    recorder = new SoundRecord();               
            //}
            
            recorder.SetFileName(wavfile);
            recorder.RecStart();

        }

        private void Btn_Up(object sender, MouseEventArgs e)
        {

            try
            {
                recorder.RecStop();
            }catch(NullReferenceException)
            {}
            
            recorder = null;
            //Boolean flag=false;
            string voiceStr = Post(Application.StartupPath + "\\test.wav");
            if (!voiceStr.Contains("3301"))
            {
                if (voiceStr.Contains("关闭")) {

                    Play("欢迎下次使用！");
                    
                    //主程停止1秒
                    Thread.Sleep(1500);
                    //该方法会强制进程关闭，并给操作系统一个退出代码。 
                    Environment.Exit(0);
                    //调用 Application.Exit() 并不一定能让程序立即退出，程序会等待所有的前台线程终止后才能真正退出。
                    //Application.Exit();
                }
              string  flag = ToolClass.openBrowser(voiceStr);
                if (flag=="")
                {

                    textBox1.Visible = true;
                    //textBox1.Text = voiceStr + ":未包含定义的字符！";
                    textBox1.Text = "指令未包含定义的字符！";

                }
                else {
                    textBox1.Text = "已打开"+flag;
                }
                
            }
            else {
                textBox1.Visible = true;
                textBox1.Text = voiceStr;
                
            }
            
            
            button1.Text = "按住说话";
            button1.BackColor = Color.FromArgb(255, 255, 255);
            string text = textBox1.Text;
            
            Play(text);
        }

        private string Post(string audioFilePath)
        {
            //string token = "24.4025c0457c09f7176ac73466f1c22cbe.2592000.1554354708.282335-15681764";dev_pid=1536
            //serverURL += "?lan=zh&cuid=QMS_Sway&token=" + token;
            serverURL += "?dev_pid=1536&cuid=QMS_Sway&token=" + token;
            FileStream fs = new FileStream(audioFilePath, FileMode.Open);
            byte[] voice = new byte[fs.Length];
            fs.Read(voice, 0, voice.Length);
            fs.Close();
            fs.Dispose();

            HttpWebRequest request = ToolClass.getRequest(serverURL);
            //var data = File.ReadAllBytes(audioFilePath);
            request.Method = "POST";
            request.Timeout = 10000;

            request.ContentType = "audio/wav; rate=16000";
            try
            {
                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(voice, 0, voice.Length);
                    writeStream.Close();
                    writeStream.Dispose();
                }
            }
            catch
            {
                return "出错了";
            }
            string result = string.Empty;
            string result_final = string.Empty;
         
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string line = string.Empty;
                        StringBuilder sb = new StringBuilder();
                        while (!readStream.EndOfStream)
                        {
                            line = readStream.ReadLine();
                            sb.Append(line);
                            sb.Append("\r");
                        }
                        readStream.Close();
                        readStream.Dispose();
                        result = sb.ToString();
                        string[] indexs = result.Split(',');
                        foreach (string index in indexs)
                        {

                            string[] _indexs = index.Split('"');

                            if (_indexs[2] == ":[")
                            {
                                result_final = _indexs[3];
                            }
                            //{"err_msg":"speech quality error.","err_no":3301,"sn":"33021332371551857418"}

                            if (_indexs[2] == ":3301")
                            {
                                return "3301:声音不清晰！";
                            }

                        }
                    }
                    responseStream.Close();
                    responseStream.Dispose();
                }
                response.Close();
            }
            return result_final;
        }

        private const string lan = "zh";//语言
        private const string per = "0";//发音人选择 0位女  1位男  默认 女0为女声，1为男声，3为情感合成-度逍遥，4为情感合成-度丫丫，默认为普通女声
        private const string ctp = "1";//客户端类型选择 web端为1  
        private const string spd = "5";//范围0~9  默认 5   语速
        private const string pit = "5";//范围0~9  默认 5   音调
        private const string vol = "5";//范围0~9  默认 5   音量
       
        private const string rest = "tex={0}&lan={1}&per={2}&ctp={3}&cuid={4}&tok={5}&spd={6}&pit={7}&vol={8}";
        private const int NULL = 0, ERROR_SUCCESS = NULL;  
        [DllImport("WinMm.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback); 
        private void Play(string txt) {

            mciSendString("close app", "", 0, 0);
           string PlayUrlz= string.Format(rest, txt, lan, per, ctp, cuid, token, spd, pit, vol);
           
            HttpWebRequest req = ToolClass.getRequest("http://tsn.baidu.com/text2audio");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = Encoding.UTF8.GetByteCount(PlayUrlz);
            using (StreamWriter sw = new StreamWriter(req.GetRequestStream())) {

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

        //System.Timers.Timer time = new System.Timers.Timer();
        //private void con_select()
        //{
        //    //到时间的时候执行事件  
        //    //time.Elapsed += new System.Timers.ElapsedEventHandler(Btn_Down);
        //    time.Interval = 500;
        //    time.AutoReset = true;//执行一次 false，一直执行true  
        //    //是否执行System.Timers.Timer.Elapsed事件  
        //    time.Enabled = true;   
        //}

    }
}
