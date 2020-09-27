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
using System.Windows.Navigation;
using System.Windows.Shapes;

//Http
using System.Net.Http;

//Timer
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.IO;

using Newtonsoft.Json;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace navbat
{
    
    struct Person
    {
        public string name;
        public double aver;
        public int number;
    }



    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        static private string guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        //public personal person;
        //Data data;
        public string[] names;
        public double[] avers;
        public int[] numbers;

        List<string> this_is_a_list_of_strings = new List<string>();

        private DispatcherTimer timer = null;
        private int x;

        private static readonly HttpClient client = new HttpClient();

        //shuhrat
        DispatcherTimer loopTimer;
        int counter_1_1, counter_1_2, counter_1_3;
        int counter_2_1, counter_2_2, counter_2_3;
        int counter_3_1, counter_3_2, counter_3_3;


        private void timerStart()
        {
            timer = new DispatcherTimer();  // если надо, то в скобках указываем приоритет, например DispatcherPriority.Render
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            Console.WriteLine("Timer is Fired!");
            x++;
            makeHttp();
        }

        public void makeHttp()
        {
            string url = "http://192.168.233.96:3000/results";
            using (var wb = new WebClient())
            {
                Console.WriteLine("Http get is fired!");
                var response = wb.DownloadString(url);
                Console.WriteLine("Http get response is received!");
                Console.WriteLine(response);
            }
        }

        private static readonly HttpListener Listener = new HttpListener();

        public MainWindow()
        {
            InitializeComponent();

            //timerStart();

            //data[0].name = "Akbar";

            dynamic stuff = JsonConvert.DeserializeObject("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");

            string name = stuff.Name;
            string address = stuff.Address.City;


            Console.WriteLine(name);
            Console.WriteLine(address);

            dynamic jsonResponce = JsonConvert.DeserializeObject("{ 'results': { 'name': 'Hasan', 'aver': '2.3', 'number': '105' } }");
            //data[0].name = stuff.results.name;
            //data[0].aver = stuff.results.aver;
            //data[0].number = stuff.results.number;

            //Console.WriteLine("this is decoded json");
            //Console.WriteLine(data[0].name);
            //Console.WriteLine(data[0].aver);
            //Console.WriteLine(data[0].number);

            //names[0] = stuff.results.name;
            //avers[0] = stuff.results.aver;
            //numbers[0] = stuff.results.number;

            //shuhrat
            // INIT LOOP TIMER
            loopTimer = new DispatcherTimer();
            loopTimer.Tick += new EventHandler(loopTimer_Tick);
            loopTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            loopTimer.Start();

            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            serverSocket.Listen(1); //just one socket
            serverSocket.BeginAccept(null, 0, OnAccept, null);
            //Console.Read();
        }


        private static void OnAccept(IAsyncResult result)
        {
            byte[] buffer = new byte[1024];
            try
            {
                Socket client = null;
                string headerResponse = "";
                if (serverSocket != null && serverSocket.IsBound)
                {
                    client = serverSocket.EndAccept(result);
                    var i = client.Receive(buffer);
                    headerResponse = (System.Text.Encoding.UTF8.GetString(buffer)).Substring(0, i);
                    // write received data to the console
                    //shu yerdan olsa bo'ladi danniyni
                    Console.WriteLine(headerResponse);
                    Console.WriteLine("=====================");
                }
                if (client != null)
                {
                    /* Handshaking and managing ClientSocket */
                    var key = headerResponse.Replace("ey:", "`")
                              .Split('`')[1]                     // dGhlIHNhbXBsZSBub25jZQ== \r\n .......
                              .Replace("\r", "").Split('\n')[0]  // dGhlIHNhbXBsZSBub25jZQ==
                              .Trim();

                    // key should now equal dGhlIHNhbXBsZSBub25jZQ==
                    var test1 = AcceptKey(ref key);

                    var newLine = "\r\n";

                    var response = "HTTP/1.1 101 Switching Protocols" + newLine
                         + "Upgrade: websocket" + newLine
                         + "Connection: Upgrade" + newLine
                         + "Sec-WebSocket-Accept: " + test1 + newLine + newLine
                         //+ "Sec-WebSocket-Protocol: chat, superchat" + newLine
                         //+ "Sec-WebSocket-Version: 13" + newLine
                         ;

                    client.Send(System.Text.Encoding.UTF8.GetBytes(response));
                    var i = client.Receive(buffer); // wait for client to send a message
                    string browserSent = GetDecodedData(buffer, i);
                    Console.WriteLine("BrowserSent: " + browserSent);

                    Console.WriteLine("=====================");
                    //now send message to client
                    client.Send(GetFrameFromString("This is message from server to client."));
                    System.Threading.Thread.Sleep(10000);//wait for message to be sent
                }
            }
            catch (SocketException exception)
            {
                throw exception;
            }
            finally
            {
                if (serverSocket != null && serverSocket.IsBound)
                {
                    serverSocket.BeginAccept(null, 0, OnAccept, null);
                }
            }
        }

        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static string AcceptKey(ref string key)
        {
            string longKey = key + guid;
            byte[] hashBytes = ComputeHash(longKey);
            return Convert.ToBase64String(hashBytes);
        }

        static SHA1 sha1 = SHA1CryptoServiceProvider.Create();
        private static byte[] ComputeHash(string str)
        {
            return sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
        }

        //Needed to decode frame
        public static string GetDecodedData(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }

            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }

            return Encoding.ASCII.GetString(buffer, dataIndex, dataLength);
        }

        //function to create  frames to send to client 
        /// <summary>
        /// Enum for opcode types
        /// </summary>
        public enum EOpcodeType
        {
            /* Denotes a continuation code */
            Fragment = 0,

            /* Denotes a text code */
            Text = 1,

            /* Denotes a binary code */
            Binary = 2,

            /* Denotes a closed connection */
            ClosedConnection = 8,

            /* Denotes a ping*/
            Ping = 9,

            /* Denotes a pong */
            Pong = 10
        }

        /// <summary>Gets an encoded websocket frame to send to a client from a string</summary>
        /// <param name="Message">The message to encode into the frame</param>
        /// <param name="Opcode">The opcode of the frame</param>
        /// <returns>Byte array in form of a websocket frame</returns>
        public static byte[] GetFrameFromString(string Message, EOpcodeType Opcode = EOpcodeType.Text)
        {
            byte[] response;
            byte[] bytesRaw = Encoding.Default.GetBytes(Message);
            byte[] frame = new byte[10];

            int indexStartRawData = -1;
            int length = bytesRaw.Length;

            frame[0] = (byte)(128 + (int)Opcode);
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new byte[indexStartRawData + length];

            int i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }


        //shuhrat
        private void loopTimer_Tick(object sender, EventArgs e)
        {
            counter_1_1++; //bu joyi test uchun

            operator_1(counter_1_1, counter_1_2, counter_1_3);
            operator_2(counter_2_1, counter_2_2, counter_2_3);
            operator_3(counter_3_1, counter_3_2, counter_3_3);
        }

        private void operator_1(int counter_1, int counter_2, int counter_3)
        {
            str_1_1.Text = counter_1.ToString();
            str_1_2.Text = counter_2.ToString();
            str_1_3.Text = counter_3.ToString();
        }

        private void operator_2(int counter_1, int counter_2, int counter_3)
        {
            str_2_1.Text = counter_1.ToString();
            str_2_2.Text = counter_2.ToString();
            str_2_3.Text = counter_3.ToString();
        }

        private void operator_3(int counter_1, int counter_2, int counter_3)
        {
            str_3_1.Text = counter_1.ToString();
            str_3_2.Text = counter_2.ToString();
            str_3_3.Text = counter_3.ToString();
        }

        public async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }


    }

}
