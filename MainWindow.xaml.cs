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
using System.Net.Sockets;

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
            //makeHttp();
            //DoAcceptTcpClientCallback();
            //MyTcpListener.mymain();

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

        Thread thread = new Thread(() =>
        {
            // put the code here that you want to be executed in a new thread
            MyTcpListener.mymain();
        });

        public MainWindow()
        {
            InitializeComponent();

            timerStart();

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


            // tcp
            thread.Start();
            // MyTcpListener.mymain();
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

    class MyTcpListener
    {
        public static void mymain()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 80;
                IPAddress localAddr = IPAddress.Parse("0.0.0.0");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        //data = data.ToUpper();

                        //byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        //// Send back a response.
                        //stream.Write(msg, 0, msg.Length);
                        //Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }

}
