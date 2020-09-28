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
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = null;

        //shuhrat
        DispatcherTimer loopTimer;
        public static int counter_1_1, counter_1_2, counter_1_3;
        public static int counter_2_1, counter_2_2, counter_2_3;
        public static int counter_3_1, counter_3_2, counter_3_3;

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
        }

        public MainWindow()
        {
            InitializeComponent();
            timerStart();

            //shuhrat
            // INIT LOOP TIMER
            loopTimer = new DispatcherTimer();
            loopTimer.Tick += new EventHandler(loopTimer_Tick);
            loopTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            loopTimer.Start();


            // tcp
            thread.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            operator_1(0,0,0);
            operator_2(0,0,0);
            operator_3(0,0,0);
        }

        //shuhrat
        private void loopTimer_Tick(object sender, EventArgs e)
        {
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

        Thread thread = new Thread(() =>
        {
            // put the code here that you want to be executed in a new thread
            mytcpserver();
        });

        public static void mytcpserver()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 4567;
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


                        switch(data[0])
                        {
                            case '1':

                                switch (data[9])
                                {
                                    case '0': counter_1_1++; break;
                                    case '5': counter_2_1++; break;
                                    case '9': counter_3_1++; break;
                                }

                                break;

                            case '2':

                                switch (data[9])
                                {
                                    case '0': counter_1_2++; break;
                                    case '5': counter_2_2++; break;
                                    case '9': counter_3_2++; break;
                                }

                                break;

                            case '3':

                                switch (data[9])
                                {
                                    case '0': counter_1_3++; break;
                                    case '5': counter_2_3++; break;
                                    case '9': counter_3_3++; break;
                                }

                                break;


                        }
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
        }
    }
}
