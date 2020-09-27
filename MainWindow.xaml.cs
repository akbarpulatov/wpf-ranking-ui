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

namespace navbat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = null;
        private int x;

        private static readonly HttpClient client = new HttpClient();


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

        public MainWindow()
        {
            InitializeComponent();
            //timerStart();


            dynamic stuff = JsonConvert.DeserializeObject("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");

            string name = stuff.Name;
            string address = stuff.Address.City;
            Console.WriteLine(name);
            Console.WriteLine(address);


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
