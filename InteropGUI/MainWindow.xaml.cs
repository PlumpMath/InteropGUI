using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;

namespace InteropGUI
{
    public partial class MainWindow : Window
    {
        System.Windows.Point currentPoint = new System.Windows.Point();
        volatile List<Location> stationary_obs = new List<Location>();
        volatile List<Location> moving_obs = new List<Location>();
        volatile List<Location> waypoints = new List<Location>();
        Obstacles obs = new Obstacles();
        volatile string connect_server_string = "Connect";
        volatile string connect_plane_string = "Connect";
        volatile string server_response_string = "No response yet";
        volatile string plane_response_string = "No response yet";
        volatile Stopwatch ping_timer = new Stopwatch();
        volatile string monitor_string = "";
        volatile string server_ip = "None";
        volatile string client_ip = "None";
        double airspeed = 0.0;
        double direction = 0.0;
        int size = 0;
        long latency = 0;
        volatile Location plane_loc = new Location();
        volatile string mode = "Unknown";
        Location tl = new Location(38.15288, -76.43819, 15);
        Location br = new Location(38.14001, -76.42077, 15);
        public MainWindow()
        {
            //Method generated behind the scenes by the xml code
            InitializeComponent();
            Location.setup(map_surface.Width, map_surface.Height, tl, br);
            //DispatcherTimer is synced with the apartment of the UI.
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1000 / 60), DispatcherPriority.Normal, new EventHandler(repaint), this.Dispatcher);
            timer.Start();
            Thread plane_networker_tcp = new Thread(new ThreadStart(plane_server_tcp));
            plane_networker_tcp.Start();
            Thread plane_networker_udp = new Thread(new ThreadStart(plane_server_udp));
            plane_networker_udp.Start();
            Thread competition_networker = new Thread(new ThreadStart(competition_client));
            competition_networker.Start();
        }
        private void plane_server_tcp()
        {
            //TODO: handle exceptions, impement waypoints
            //connect to plane
            //receive ping
            //receive plane location, airspeed, direction
            //receive waypoints
            while (true)
            {
                try
                {


                    while (client_ip.Equals("None"))
                    {
                        Thread.Sleep(100);
                    }
                    connect_plane_string = "Connecting";
                    IPAddress ipAd = IPAddress.Parse(client_ip);
                    TcpListener tcp = new TcpListener(ipAd, 25000);
                    tcp.Start();
                    Socket s = tcp.AcceptSocket();
                    connect_plane_string = "Connected";
                    byte[] home_bytes = new byte[32];
                    s.Receive(home_bytes);
                    while (true)
                    {
                        byte[] b = new byte[4];
                        char[] c = new char[4];
                        s.Receive(b);
                        for (int go = 0; go < 4; go++)
                        {
                            c[go] = Convert.ToChar(b[go]);
                        }
                        int msg_len = Convert.ToInt32(Convert.ToString(c)) - 1;
                        byte[] b2 = new byte[1];
                        s.Receive(b2);
                        char id = Convert.ToChar(b2[0]);
                        byte[] b3 = new byte[msg_len];
                        s.Receive(b3);
                        switch (id)
                        {
                            case 't'://time and location
                                char[] loc_chars = new char[msg_len];
                                for (int i = 0; i < msg_len; i++)
                                {
                                    loc_chars[i] = Convert.ToChar(b3[i]);
                                }
                                string loc_str = new string(loc_chars);
                                string[] loc_params = loc_str.Split(',');
                                //splice and use
                                plane_loc.lat = Convert.ToDouble(loc_params[2]);
                                plane_loc.lon = Convert.ToDouble(loc_params[3]);
                                plane_loc.alt = Convert.ToDouble(loc_params[4]);
                                direction = Convert.ToDouble(loc_params[5]);
                                airspeed = Convert.ToDouble(loc_params[7]);

                                break;
                            case 'w'://waypoints
                                     //convert to char
                                     //convert to string
                                     //convert to int
                                     //read in that many sets of waypoints
                                break;
                            case 'm'://mode
                                char[] mode_chars = new char[msg_len];
                                for (int i = 0; i < msg_len; i++)
                                {
                                    mode_chars[i] = Convert.ToChar(b3[i]);
                                }
                                mode = Convert.ToString(mode_chars);
                                break;
                            case 'd'://plane response
                                char[] response_chars = new char[msg_len];
                                for (int i = 0; i < msg_len; i++)
                                {
                                    response_chars[i] = Convert.ToChar(b3[i]);
                                }
                                plane_response_string = Convert.ToString(response_chars);
                                break;
                            case 'p':
                                latency = ping_timer.ElapsedMilliseconds;
                                ping_timer.Reset();
                                ping_timer.Stop();
                                break;
                            default:
                                break;
                        }

                    }

                }
                catch (Exception e)
                {
                    connect_plane_string = "Connect";
                    client_ip = "None";
                }
            }

        }
        private void plane_server_udp()
        {
            //connect to plane
            //send obstacles
            try
            {
                while (client_ip.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                IPAddress ipAd = IPAddress.Parse(client_ip);
                TcpListener tcp = new TcpListener(ipAd, 25001);
                tcp.Start();
                Socket s = tcp.AcceptSocket();
            
                while (true)
                {

                    if (!(ping_timer.IsRunning))
                    {
                        Byte[] bytes = Encoding.ASCII.GetBytes("0004ping");
                        s.Send(bytes);
                        ping_timer.Start();
                    }

                    //send obstacles
                    //
                    Thread.Sleep(500);
                }
            }
            catch(Exception e)
            {
                plane_response_string = e.ToString();
            }
        }//TODO
        private void competition_client()
        {
            while (server_ip.Equals("None"))
            {
                Thread.Sleep(100);
            }
            connect_server_string = "Connecting";

            Uri site = new Uri("http://" + server_ip + ":8080/api/login");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(site);
            CookieContainer cc = new CookieContainer();//for login, not eating
            wr.CookieContainer = cc;
            string login = "username=utuav&password=utuav";//CHANGE FOR COMPETITION
            byte[] bytes = Encoding.UTF8.GetBytes(login);
            wr.Method = WebRequestMethods.Http.Post;
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.ContentLength = bytes.Length;
            try
            {
                using (var stream = wr.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
                var response = (HttpWebResponse)wr.GetResponse();
                server_response_string = new StreamReader(response.GetResponseStream()).ReadToEnd();

                wr.Method = WebRequestMethods.Http.Get;
            }
            catch (Exception e)
            {
                server_response_string = "u fucked up mate: " + e.Message;
            }
            Stopwatch stopwatch = new Stopwatch();
            connect_server_string = "Connected";
            long a = 0;
            while (true)
            {
                stopwatch.Start();
                //Get obstacles
                if (a % 10 == 0)
                {
                    site = new Uri("http://" + server_ip + "/api/obstacles");
                    wr = (HttpWebRequest)WebRequest.Create(site);
                    wr.CookieContainer = cc;
                    wr.ContentType = "application/json";
                    string mobs = "";
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                        mobs = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        response.Close();
                    }
                    catch (Exception e)
                    {
                        server_response_string = "u fucked up mate: " + e.Message;
                    }
                    //parse obstacles
                    obs = JsonConvert.DeserializeObject<Obstacles>(mobs);
                }


                //post uas telemetry
                site = new Uri("http://" + server_ip + "/api/telemetry");
                wr = (HttpWebRequest)WebRequest.Create(site);
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.CookieContainer = cc;
                string info = "latitude=" + plane_loc.lat + "&longitude=" + plane_loc.lon + "&altitude_msl=" + plane_loc.alt + "&uas_heading=" + direction;
                bytes = Encoding.UTF8.GetBytes(info);
                wr.Method = WebRequestMethods.Http.Post;
                wr.ContentLength = bytes.Length;
                try
                {
                    using (var stream = wr.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Close();
                    }
                    var response = (HttpWebResponse)wr.GetResponse();
                    server_response_string = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    wr.Method = WebRequestMethods.Http.Get;
                }
                catch (Exception e)
                {
                    server_response_string = "u fucked up mate: " + e.Message;
                }
                int towait = 150 - (int)stopwatch.ElapsedMilliseconds;
                if (towait > 0)
                {
                    Thread.Sleep(towait);
                }
                stopwatch.Reset();
                a++;
            }
        }
        private void Canvas_Initialized(object sender, EventArgs e)
        {
            Bitmap bm = Properties.Resources.auvsi_map_2;//start by using project resource
            IntPtr hbm = bm.GetHbitmap();
            BitmapSource bms;
            try//convert the bitmap to a bitmapsource
            {
                bms = (Imaging.CreateBitmapSourceFromHBitmap(hbm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            }
            finally { }
            image.Source = bms;
            image.Stretch = Stretch.UniformToFill;
            //map.Source = bms;//provide the image control with the source
            ///*useful for if/when fill is removed
            //map.MinWidth = map_surface.Width;
            //map.MinHeight = map_surface.Height;
            //map.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //map.VerticalAlignment = VerticalAlignment.Top;
            //*/
            //map.Stretch = Stretch.Fill;//Map fills the window
            //grid.Children.Insert(0,map);//puts map in back position (drawn first)
        }
        private void repaint(object sender, EventArgs e)
        {
            update();
            while (map_surface.Children.Count > 0)
            {
                map_surface.Children.RemoveAt(0);
            }
            if (obs.stationary_obstacles != null)
            {
                monitor_string = "" + obs.stationary_obstacles.Count + " obstacles\t";
                foreach (StationaryObstacle so in obs.stationary_obstacles)
                {
                    Ellipse circle = new Ellipse();
                    double radx = so.cylinder_radius / 10;
                    double rady = so.cylinder_radius / 10;
                    circle.Height = radx * 2;
                    circle.Width = rady * 2;
                    System.Windows.Media.Color color = new System.Windows.Media.Color();
                    color.B = 0;
                    color.G = 0;
                    color.R = 255;
                    color.A = 255;
                    circle.Stroke = new SolidColorBrush(color);
                    circle.Fill = new SolidColorBrush(color);
                    System.Windows.Point loc = Location.convert(so.longitude, so.latitude);
                    circle.Margin = new Thickness(loc.X - radx, loc.Y - rady, 0, 0);
                    monitor_string += Location.width_ratio + "," + Location.height_ratio;
                    map_surface.Children.Add(circle);
                }
            }
            if (obs.moving_obstacles != null)
            {
                monitor_string = "" + obs.moving_obstacles.Count + " obstacles\t";
                foreach (MovingObstacle mo in obs.moving_obstacles)
                {
                    Ellipse circle = new Ellipse();
                    double radx = mo.sphere_radius / 10;
                    double rady = mo.sphere_radius / 10;
                    circle.Height = radx * 2;
                    circle.Width = rady * 2;
                    System.Windows.Media.Color color = new System.Windows.Media.Color();
                    color.B = 255;
                    color.G = 0;
                    color.R = 255;
                    color.A = 255;
                    circle.Stroke = new SolidColorBrush(color);
                    circle.Fill = new SolidColorBrush(color);
                    System.Windows.Point loc = Location.convert(mo.longitude, mo.latitude);
                    circle.Margin = new Thickness(loc.X - radx, loc.Y - rady, 0, 0);
                    map_surface.Children.Add(circle);
                }
            }

        }
        private void update()
        {
            latitude_textbox.Text = "" + plane_loc.lat;
            longitude_textbox.Text = "" + plane_loc.lon;
            altitude_textbox.Text = "" + plane_loc.alt;
            airspeed_textbox.Text = "" + airspeed;
            plane_connect_button.Content = connect_plane_string;
            server_connect_button.Content = connect_server_string;
            mode_textbox.Text = mode;
            server_response_textbox.Text = server_response_string;
            plane_response_textbox.Text = plane_response_string;
            latency_textbox.Text = "" + latency;
        }


        //Vestigial, allows for map drawing.
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
        }

        private void Canvas_MouseMoved(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();

                line.Stroke = System.Windows.SystemColors.WindowFrameBrush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(map_surface).X;
                line.Y2 = e.GetPosition(map_surface).Y;

                currentPoint = e.GetPosition(map_surface);

                map_surface.Children.Add(line);
            }
        }

        private void plane_connect_button_Click(object sender, RoutedEventArgs e)
        {
            client_ip = plane_ip_textbox.Text;
        }

        private void server_connect_button_Click(object sender, RoutedEventArgs e)
        {
            server_ip = server_ip_textbox.Text;
        }
    }
    public class MovingObstacle
    {
        public double altitude_msl { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double sphere_radius { get; set; }
    }

    public class StationaryObstacle
    {
        public double cylinder_height { get; set; }
        public double cylinder_radius { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Obstacles
    {
        public List<MovingObstacle> moving_obstacles { get; set; }
        public List<StationaryObstacle> stationary_obstacles { get; set; }
    }
    public partial class Location
    {
        public double lat;
        public double lon;
        public double alt;//in MSL, feet
        public static double width_ratio;
        public static double height_ratio;
        public static Location tl;
        public static Location br;
        public Location()
        {
            lat = 0;
            lon = 0;
            alt = 0;
        }
        public Location(double lat, double lon, double alt)
        {
            this.lat = lat;
            this.lon = lon;
            this.alt = alt;
        }
        public static System.Windows.Point convert(double gps_x, double gps_y)//lon,lat
        {
            return new System.Windows.Point(-(tl.lon - gps_x) * width_ratio, (tl.lat - gps_y) * height_ratio);
        }
        public static void setup(double mw, double mh, Location bot_right, Location top_left)
        {
            tl = top_left;
            br = bot_right;
            width_ratio = -mw / (tl.lon - br.lon);//map width in pixels / longitude, negative because it increases right to left in our hemisphere
            height_ratio = mh / (tl.lat - br.lat);//map height in pixels / latitude

        }
    }
}
