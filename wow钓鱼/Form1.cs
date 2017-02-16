using myClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wow钓鱼
{
    public partial class Form1 : Form
    {
        public List<settings> settings =new List<settings>();

        public Form1()
        {
            InitializeComponent();
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Shift, Keys.S);
            myClassLibrary.helper.HotKey.RegisterHotKey(Handle, 101, myClassLibrary.helper.HotKey.KeyModifiers.Shift, Keys.D);
            this.textBox1.Text = "342";
            this.textBox2.Text = "0";
            this.textBox3.Text = "1562";
            this.textBox4.Text = "743";

            this.textBox6.Text = "#211948";// "#2A1C42";
            this.textBox7.Text = "7";
            this.textBox8.Text = "55";
            this.textBox9.Text = "新建配置";

            //加载配置文件
            if (!File.Exists(Application.StartupPath + "\\config.txt"))
            {
                //不存在
                FileStream fs1 = new FileStream(Application.StartupPath + "\\config.txt", FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);
                settings.Add(new settings() 
                { 
                    changetime = DateTime.Now,
                    leftx=this.textBox1.Text,
                    lefty = this.textBox2.Text,
                    rightx = this.textBox3.Text,
                    righty = this.textBox4.Text,
                    color =this.textBox6.Text,
                    colorDeviation = this.textBox7.Text,
                    moveRange = this.textBox8.Text,
                    name =this.textBox9.Text,
                });
                sw.WriteLine(JsonConvert.SerializeObject(settings));//开始写入值
                sw.Close();
                fs1.Close();
            }
            StreamReader sr = new StreamReader(Application.StartupPath + "\\config.txt", false);
            settings = JsonConvert.DeserializeObject<List<settings>>(sr.ReadToEnd());
            sr.Close();
            if (settings!=null)
            {

                foreach (var item in settings.OrderByDescending(o => o.changetime))
                {
                    this.comboBox1.Items.Add(item.name);
                }
                this.comboBox1.SelectedItem = settings.OrderByDescending(o => o.changetime).First().name;
                //加载配置
                loadsetting(settings.OrderByDescending(o => o.changetime).First());
            }


        }

        private void loadsetting(settings s)
        {
            this.textBox1.Text = s.leftx;
            this.textBox2.Text = s.lefty;
            this.textBox3.Text = s.rightx;
            this.textBox4.Text = s.righty;
            this.textBox6.Text = s.color;
            this.textBox7.Text = s.colorDeviation;
            this.textBox8.Text = s.moveRange;
            this.textBox9.Text = s.name;
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;//如果m.Msg的值为0x0312那么表示用户按下了热键
            //按快捷键 
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:    //按下的是Shift+S
                            //此处填写快捷键响应代码         
                            //MessageBox.Show("ok");
                            if (isOn)
                            {
                                isOn = false;
                                this.textBox5.Text += "结束钓鱼";
                                if (thread.IsAlive)
                                {
                                    thread.Abort();
                                }
                            }
                            else
                            {
                                isOn = true;
                                this.textBox5.Text += "开始钓鱼";

                                thread = new Thread(fishing);
                                thread.Start();
                            }
                            break;
                        case 101://shift D
                            //获取颜色,保存到颜色输入框
                            this.textBox6.Text = helper.ColorHelper.获取指定坐标的16进制颜色((Cursor.Position.X).ToString(),(Cursor.Position.Y).ToString());
                            this.BackColor = ColorTranslator.FromHtml(this.textBox6.Text);
                            ;break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        bool isOn = false;
        Thread thread;

        private void button1_Click(object sender, EventArgs e)
        {
            if (isOn)
            {
                isOn = false;
                this.textBox5.Text += "结束钓鱼";
                if (thread.IsAlive)
                {
                    thread.Abort();
                }
            }
            else
            {
                isOn = true;
                this.textBox5.Text += "开始钓鱼";

                thread = new Thread(fishing);
                thread.Start();
            }
            
        }

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo); 

        const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标 
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", EntryPoint = "keybd_event")]

        public static extern void keybd_event(

        byte bVk, //虚拟键值

        byte bScan,// 一般为0

        int dwFlags, //这里是整数类型 0 为按下，2为释放

        int dwExtraInfo //这里是整数类型 一般情况下设成为 0

        );

        private void fishing()
        {
            //基本参数
            int delay = 20;
            int x1 = Convert.ToInt32(this.textBox1.Text);
            int y1 = Convert.ToInt32(this.textBox2.Text);
            int x2 = Convert.ToInt32(this.textBox3.Text);
            int y2 = Convert.ToInt32(this.textBox4.Text);
            int stepx = (x2-x1)/10;
            int stepy = (y2 - y1) / 10;
            DateTime dt = DateTime.Now;

            do
            {
                Random r = new Random();
                if (dt.AddMinutes(9)<=DateTime.Now)
                {
                    keybd_event(50, 0, 0, 0);
                    System.Threading.Thread.Sleep(r.Next(150, 380));
                    keybd_event(50, 0, 2, 0);
                    System.Threading.Thread.Sleep(r.Next(3150, 4380));
                    dt = DateTime.Now;
                }
                keybd_event(49, 0, 0, 0);
                System.Threading.Thread.Sleep(r.Next(150, 380));
                keybd_event(49, 0, 2, 0);

                System.Threading.Thread.Sleep(r.Next(1150, 1380));

                Class1 c1 = new Class1();
                //#243C22 
                var p = c1.FindColor(this.textBox6.Text, new Rectangle() { X = x1, Y = y1, Height = y2 - y1, Width = x2 - x1 }, Convert.ToByte(this.textBox7.Text));
                //MessageBox.Show("坐标"+p.X+","+p.Y);
                this.textBox5.Text += "\r\n" + "坐标" + p.X + "," + p.Y;
                if (p.X == -1)
                {
                    continue;
                }

                SetCursorPos(p.X, p.Y);
                System.Threading.Thread.Sleep(2000);
                //循环20秒再找这个颜色,如果位置偏差则右键
                int x = 0;
                for (int i = 0; i < 1800; i++)
                {
                    System.Threading.Thread.Sleep(10);

                    var p2 = c1.FindColor(this.textBox6.Text, new Rectangle() { X = x1, Y = y1, Height = y2 - y1, Width = x2 - x1 }, Convert.ToByte(this.textBox7.Text));


                    int range = Convert.ToInt32( Math.Sqrt(Math.Abs(((p2.X - p.X) * (p2.Y - p.Y)))));
                    //this.textBox5.Text += range + ",";
                    if (range > Convert.ToInt32(this.textBox8.Text))
                    {
                        //x = range;
                        textBox5.Text += "偏差："+range+"收杆";
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                        System.Threading.Thread.Sleep(r.Next(90, 280));
                        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                        break;
                    }

                }
                System.Threading.Thread.Sleep(r.Next(1550, 2380));
            } while (this.checkBox1.Checked && isOn);

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            if (this.textBox9.Text.Trim() == "")
            {
                MessageBox.Show("名称不能为空");
                this.textBox9.Focus();
                return;
            }

            //读取根目录下的config.txt文件,如果有,则加载,如果没有就新建
            //FileStream file = new FileStream(Application.StartupPath+"config.txt", FileMode.OpenOrCreate);

            StreamWriter sw = new StreamWriter(Application.StartupPath + "\\config.txt",false);
            settings s = new settings()
            {
                changetime = DateTime.Now,
                leftx = this.textBox1.Text,
                lefty = this.textBox2.Text,
                rightx = this.textBox3.Text,
                righty = this.textBox4.Text,
                color = this.textBox6.Text,
                colorDeviation = this.textBox7.Text,
                moveRange = this.textBox8.Text,
                name = this.textBox9.Text,
            };

            if (settings == null)
            {
                settings = new List<settings>();
            }

            if ( settings.FirstOrDefault(o => o.name == this.textBox9.Text) != null)
            {
                //找到同名的setting
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).changetime = DateTime.Now;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).leftx = this.textBox1.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).lefty = this.textBox2.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).rightx = this.textBox3.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).righty = this.textBox4.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).color = this.textBox6.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).colorDeviation = this.textBox7.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).moveRange = this.textBox8.Text;
                settings.FirstOrDefault(o => o.name == this.textBox9.Text).remark = "";
                if (settings.FirstOrDefault(o => o.name == this.textBox9.Text).name.Trim() == "")
                {
                    settings.FirstOrDefault(o => o.name == this.textBox9.Text).name = "新建配置";
                }
            }
            else
            {
                settings.Add(s);
            }
            

            string str = JsonConvert.SerializeObject(settings);
            sw.WriteLine(str);
            sw.Close();//写入
        }

        /// <summary>
        /// 选择配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadsetting(settings.First(o=>o.name == comboBox1.SelectedItem));

        }
    }
}
