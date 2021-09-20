﻿using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MorningRoutine
{
    class Sensor
    {
        private SerialPort sensorPort;                 //M5Stickポート
        private string COMPort = "";
        private bool connectflag = false;
        public double[] a = new double[3];
        public string str;

        public bool Connect()
        {
            COMPort = "COM4";       //適宜変更してください
            connectflag = true;
            Console.WriteLine("接続中");
            sensorPort = new SerialPort(COMPort, 115200, Parity.None, 8, StopBits.One);
            sensorPort.Open();

            return connectflag;
        }


        public void ReadData()
        {
            if (connectflag)
            {
                str = sensorPort.ReadLine();
                str = str.Split('\r')[0];
            }
        }

        public void sendData(string dat)
        {
            if (connectflag)
            {
                sensorPort.Write(dat);
            }
        }
    }
}
