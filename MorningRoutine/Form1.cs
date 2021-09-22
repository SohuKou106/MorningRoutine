using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MorningRoutine
{
    public partial class Form1 : Form
    {
        const int dataPoints = 10000;
        const int brightness = 2400;    //画像の輝度。大きくすると明るく、小さくすると暗くなります。
        Stopwatch sw = new Stopwatch();
        Sensor sensor = new Sensor();
        StreamWriter writer;
        int data_num = 0;

        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.MaximizeBox = false;
            this.MinimizeBox = false;

            if (!File.Exists("Result"))
            {
                new DirectoryInfo("Result").Create();
            }

            labelX.Text = "接続しています...";

            bool connected = sensor.Connect();
            if (!connected)
            {
                this.Close();
            }
            labelX.Text = "ボタンを押してください";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data_num = 0;
            sensor.sendData("d");
            writer = new StreamWriter("Result/res.csv", false, Encoding.UTF8);
            sw.Restart();
            timer1.Start();
            button1.Enabled = false;

            bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = bmp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sensor.ReadData();
            writer.Write(sw.ElapsedMilliseconds + ",");
            writer.WriteLine(sensor.str.Split('\r')[0]);
            labelX.Text = $"計測中：{data_num * 100 / dataPoints} %";

            data_num++;
            if(data_num % (dataPoints / 20) == 0)
            {
                sensor.sendData(data_num.ToString());
            }

            if(data_num == dataPoints)
            {
                labelX.Text = "計測終了";

                writer.Close();
                timer1.Stop();
                CreateDayImage();
                button1.Enabled = true;
            }
        }

        private void CreateDayImage()
        {
            using (StreamReader reader = new StreamReader("Result/res.csv"))
            {
                int pixelnum = 0;
                while (!reader.EndOfStream)
                {
                    if(pixelnum > dataPoints)
                    {
                        break;
                    }
                    double[] color = new double[3];
                    int correctnum = 0;
                    
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    foreach(string value in values)
                    {
                        if (correctnum == 0)
                        {
                            correctnum++;
                            continue;
                        }

                        try
                        {
                            double c = (double.Parse(value) / brightness * 127 ) + 128;
                            if (c > 255) c = 255;
                            if (c < 0) c = 0;
                            color[correctnum - 1] = c;
                        }
                        catch
                        {
                            //センサーの動作不良による不正な値を弾く
                            Console.WriteLine($"{value}が異常値です");
                            continue;
                        }
                        correctnum++;
                    }

                    if (correctnum - 1 == 3)
                    {
                        drawPixel(color, pixelnum % (int)Math.Sqrt(dataPoints), pixelnum / (int)Math.Sqrt(dataPoints));
                        pixelnum++;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            sensor.sendData("e");
            DateTime dt = DateTime.Now;
            Image img = pictureBox1.Image;
            int duplicateNum = 1;
            while (File.Exists($"Result/{dt.Year}_{dt.Month:d2}_{dt.Day:d2}_{duplicateNum}.png"))
            {
                duplicateNum++;
            }
            img.Save($"Result/{dt.Year}_{dt.Month:d2}_{dt.Day:d2}_{duplicateNum}.png");
        }

        void drawPixel(double[] color, int x, int y) 
        { 
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            Pen p = new Pen(Color.FromArgb(255, (int)color[0], (int)color[1], (int)color[2]), 2);
            SolidBrush br = new SolidBrush(Color.FromArgb(255, (int)color[0], (int)color[1], (int)color[2]));
            g.FillRectangle(br, x * 3, y * 3, 3, 3);
            pictureBox1.Refresh();
        }
    }
}
