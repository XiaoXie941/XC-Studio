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
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Net;

namespace XC_Studio
{
    /// <summary>
    /// NetworkToolsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkToolsWindow : Window
    {
        private bool _isDragging = false;
        
        public NetworkToolsWindow()
        {
            InitializeComponent();
            LoadNetworkInterfaces();
            
            // 监听主窗口关闭事件
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Closing += MainWindow_Closing;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        // 当主窗口关闭时，也关闭此窗口
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 取消订阅事件，防止内存泄漏
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Closing -= MainWindow_Closing;
            }
            this.Close();
        }

        // 窗口拖动相关事件
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                this.DragMove();
            }
        }
        
        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }
        
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        
        // 网络工具页面功能
        private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            string url = NetworkTestUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("请输入要测试的网址", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            // 在后台线程执行网络测试
            Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (!_isDragging)
                        {
                            ConnectionStatusText.Text = "测试中...";
                            ConnectionStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // 橙色
                        }
                    });
                    
                    var stopwatch = Stopwatch.StartNew();
                    var uri = new Uri(url);
                    var host = uri.Host;
                    
                    // 获取IP地址
                    var hostEntry = Dns.GetHostEntry(host);
                    var ipAddress = hostEntry.AddressList[0].ToString();
                    
                    // 测试连接
                    var ping = new Ping();
                    var reply = ping.Send(host, 3000);
                    
                    stopwatch.Stop();
                    
                    Dispatcher.Invoke(() =>
                    {
                        if (!_isDragging)
                        {
                            if (reply.Status == IPStatus.Success)
                            {
                                ConnectionStatusText.Text = "连接成功";
                                ConnectionStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 201, 176)); // 绿色
                                ConnectionTimeText.Text = $"{stopwatch.ElapsedMilliseconds} ms";
                                IpAddressText.Text = ipAddress;
                            }
                            else
                            {
                                ConnectionStatusText.Text = "连接失败";
                                ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Red);
                                ConnectionTimeText.Text = "--";
                                IpAddressText.Text = "--";
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (!_isDragging)
                        {
                            ConnectionStatusText.Text = "连接失败";
                            ConnectionStatusText.Foreground = new SolidColorBrush(Colors.Red);
                            ConnectionTimeText.Text = "--";
                            IpAddressText.Text = "--";
                        }
                    });
                    
                    Debug.WriteLine($"网络测试失败: {ex.Message}");
                }
            });
        }
        
        private void LoadNetworkInterfaces()
        {
            Task.Run(() =>
            {
                try
                {
                    var interfaces = new List<NetworkInterfaceInfo>();
                    
                    foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        var info = new NetworkInterfaceInfo
                        {
                            Name = ni.Name,
                            Type = GetNetworkInterfaceType(ni.NetworkInterfaceType),
                            Status = ni.OperationalStatus == OperationalStatus.Up ? "已连接" : "已断开",
                            Speed = ni.Speed > 0 ? $"{ni.Speed / 1000000} Mbps" : "未知",
                            IpAddress = GetIpAddress(ni)
                        };
                        
                        interfaces.Add(info);
                    }
                    
                    Dispatcher.Invoke(() =>
                    {
                        if (!_isDragging)
                        {
                            NetworkInterfacesList.ItemsSource = interfaces;
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"加载网络接口失败: {ex.Message}");
                }
            });
        }
        
        private string GetNetworkInterfaceType(NetworkInterfaceType type)
        {
            switch (type)
            {
                case NetworkInterfaceType.Ethernet: return "以太网";
                case NetworkInterfaceType.Wireless80211: return "Wi-Fi";
                case NetworkInterfaceType.Loopback: return "回环";
                case NetworkInterfaceType.Ppp: return "PPP";
                case NetworkInterfaceType.Tunnel: return "隧道";
                default: return "其他";
            }
        }
        
        private string GetIpAddress(NetworkInterface ni)
        {
            try
            {
                var ipProps = ni.GetIPProperties();
                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && 
                        !IPAddress.IsLoopback(ip.Address))
                    {
                        return ip.Address.ToString();
                    }
                }
            }
            catch
            {
                // 忽略错误
            }
            
            return "无";
        }
    }
    
    // 网络接口信息类
    public class NetworkInterfaceInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Speed { get; set; }
        public string IpAddress { get; set; }
    }
}