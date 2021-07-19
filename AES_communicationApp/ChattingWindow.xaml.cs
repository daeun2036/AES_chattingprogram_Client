using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Quobject.SocketIoClientDotNet.Client; //socket.io for .NET (Client)
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using EngineIOSharp.Common.Enum;
using EngineIOSharp.Client;
using EngineIOSharp.Common.Packet;
using SocketIOSharp.Client;
using SocketIOSharp.Common;
using SocketIOSharp.Common.Abstract.Connection;
using System.Threading;
using System.IO;

namespace AES_communicationApp
{
    /// <summary>
    /// ChattingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChattingWindow : Window
    {
        
        private List<string> messageList = new List<string>();
        private SocketIOClient client = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, "localhost", 3000));
        
        public ChattingWindow()
        {
            InitializeComponent();
            initdata();
            socketIoManager();
        }

        private void initdata()
        { 
            textbox_name.Clear();
            textbox_room.Clear();
            textbox_send.Clear();
            //messageListView.ItemsSource = messageList;
        }

        private void socketIoManager()
        {
            // socket connect
            client.Connect();
            Console.WriteLine("connected!!");

            // 소켓 받음
            client.On("rcv",(Data) =>
            {
                if (Data != null && Data.Length > 0 && Data[0] != null)
                {
                    string name, room, msg = null;

                    name = Data[0].Value<string>("name");
                    room = Data[0].Value<string>("room");
                    msg = Data[0].Value<string>("msg");

                    Message incoming = new Message(name, room, msg);
                    incoming.msg = Encryption_Decryption(msg, 'D');

                    // 비동기
                    Dispatcher.Invoke(() =>
                    {
                        updateChatWindow(incoming.name,incoming.msg);
                    });
                }

            });
        }
        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            // check IsNullOrEmpty
            if (string.IsNullOrEmpty(this.textbox_name.Text) || string.IsNullOrEmpty(this.textbox_room.Text) || string.IsNullOrEmpty(this.textbox_send.Text))
            {
                MessageBox.Show("please enter your name, room and message");
                return;
            }
                
            Message send = new Message(this.textbox_name.Text, this.textbox_room.Text, this.textbox_send.Text);
            send.msg = Encryption_Decryption(send.msg, 'E');
            client.Emit("message", send);
        }

        private void updateChatWindow(string name,string msg)
        {
            // for (int i = 0; i < messageList.Count; i++)
            //     Console.WriteLine(messageList[i]);

            
            messageList.Add(name + ": " + msg);
            textbox_msglistview.Text += name + ": " + msg +"\r\n";
            //ScrollToBot();
            //messageListView.ItemsSource = messageList;
            this.textbox_send.Clear();
        }
        /*
        private void ScrollToBot()
        {
            if (VisualTreeHelper.GetChildrenCount(messageListView) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(messageListView, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
        */
        private string Encryption_Decryption(string text,char type)
        {
            System.Diagnostics.ProcessStartInfo proInfo = new System.Diagnostics.ProcessStartInfo();
            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            proInfo.FileName = @"cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardError = true;

            pro.StartInfo = proInfo;
            pro.Start();

            //CMD에 보낼 명령어를 입력
            pro.StandardInput.Write(@"cd C:\\Users\\82103\\source\\repos\\AES\\x64\\Debug" + Environment.NewLine);

            // Encryption
            if (type == 'E')
                pro.StandardInput.Write(@"AES.exe E " + "\"" + text + "\"" + Environment.NewLine);
            // Decryption
            else if (type == 'D')
                pro.StandardInput.Write(@"AES.exe D " + "\"" + text + "\"" + Environment.NewLine);
            pro.StandardInput.Close();

            string resultValue = pro.StandardOutput.ReadToEnd();
            //Console.WriteLine("this is result : "+resultValue);
            pro.WaitForExit();
            pro.Close();

            //데이터 파싱
            string[] result = resultValue.Split(new char[] { '@' });
            //string afterTxt = result[1];

            return result[1];
        }

        private void key_down(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
                btn_send_Click(this, e);
        }
    }
}