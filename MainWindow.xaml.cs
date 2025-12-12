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
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

namespace XC_Studio
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging = false;
        
        public MainWindow()
        {
            InitializeComponent();
            UpdateSystemInfo();
            
            // 设置定时器，每0.5秒更新一次系统信息
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += Timer_Tick;
            timer.Start();
            
            // 监听窗口关闭事件，确保所有子窗口也被关闭
            this.Closing += MainWindow_Closing;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _isDragging = true;
                    this.DragMove();
                }
            }
            catch
            {
                // 忽略拖动错误
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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 关闭所有子窗口
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }
            this.Close();
        }

        // 快速访问功能
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
        }

        private void PerformanceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("性能监视器", "实时监控系统性能和资源使用情况");
        }

        private void SystemToolsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("系统工具", "系统维护和优化工具集");
        }

        private void DataAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("数据分析", "数据处理和可视化分析工具");
        }

        // 集成应用功能
        private void NetworkToolsButton_Click(object sender, RoutedEventArgs e)
        {
            NetworkToolsWindow networkTools = new NetworkToolsWindow();
            networkTools.Show();
        }

        private void FileManagerButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("文件管理器", "高效的文件和目录管理工具");
        }

        private void DatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("数据库管理", "数据库连接、查询和管理工具");
        }

        private void SecurityButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("安全工具", "系统安全扫描和防护工具");
        }

        private void ImageProcessingButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("图像处理", "图像编辑、转换和批处理工具");
        }

        private void DocumentEditorButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("文档编辑器", "多格式文档编辑和转换工具");
        }

        // 开发工具功能
        private void UIDesignerButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("UI设计器", "可视化用户界面设计工具");
        }

        private void TestToolsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("测试工具", "自动化测试和质量保证工具");
        }

        private void PackageManagerButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("包管理器", "软件包安装、更新和管理");
        }

        // 系统设置功能
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("设置", "应用程序和系统配置管理");
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage("关于 XC Studio", "版本信息和帮助文档");
        }

        // 页面显示管理
        private void ShowHomePage()
        {
            PageTitle.Text = "XC Studio 主页";
            PageDescription.Text = "专业应用集成工具平台";
            HomePageContent.Visibility = Visibility.Visible;
            OtherPageContent.Visibility = Visibility.Collapsed;
        }

        private void ShowPage(string title, string description)
        {
            PageTitle.Text = title;
            PageDescription.Text = description;
            HomePageContent.Visibility = Visibility.Collapsed;
            OtherPageContent.Visibility = Visibility.Visible;
        }

        // 定时器更新系统信息
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 即使在拖动时也更新系统信息，但只在非拖动状态下更新UI
            UpdateSystemInfo();
        }

        // 更新系统信息 - 使用SystemInfo类
        private async void UpdateSystemInfo()
        {
            try
            {
                // 更新CPU使用率
                var cpuUsage = await SystemInfo.CPU.GetUsagePercentage();
                Dispatcher.Invoke(() =>
                {
                    if (!_isDragging)
                    {
                        CpuUsageText.Text = $"{cpuUsage:F0}%";
                        CpuDetailsText.Text = "使用率";
                        
                        if (cpuUsage > 80)
                        {
                            CpuUsageText.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else if (cpuUsage > 60)
                        {
                            CpuUsageText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                        }
                        else
                        {
                            CpuUsageText.Foreground = new SolidColorBrush(Colors.White);
                        }
                    }
                });
                
                // 更新内存信息
                var memoryInfo = SystemInfo.Memory.GetPhysicalMemory();
                Dispatcher.Invoke(() =>
                {
                    if (!_isDragging)
                    {
                        MemoryUsageText.Text = $"{memoryInfo.UsedGB:F1}GB";
                        MemoryDetailsText.Text = $"已用 / {memoryInfo.TotalGB:F1}GB";
                        
                        if (memoryInfo.UsagePercent > 85)
                        {
                            MemoryUsageText.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else if (memoryInfo.UsagePercent > 70)
                        {
                            MemoryUsageText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                        }
                        else
                        {
                            MemoryUsageText.Foreground = new SolidColorBrush(Colors.White);
                        }
                    }
                });
                
                // 更新磁盘信息 - 显示所有硬盘的总容量
                var allDisksInfo = SystemInfo.Disk.GetAllDisksInfo();
                Dispatcher.Invoke(() =>
                {
                    if (!_isDragging)
                    {
                        DiskUsageText.Text = $"{allDisksInfo.TotalGB:F0}GB";
                        DiskDetailsText.Text = $"总容量 / {allDisksInfo.FreeGB:F0}GB可用 ({allDisksInfo.DriveCount}个硬盘)";
                        
                        var usagePercent = allDisksInfo.TotalGB > 0 ? (allDisksInfo.UsedGB / allDisksInfo.TotalGB) * 100 : 0;
                        if (usagePercent > 90)
                        {
                            DiskUsageText.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else if (usagePercent > 80)
                        {
                            DiskUsageText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                        }
                        else
                        {
                            DiskUsageText.Foreground = new SolidColorBrush(Colors.White);
                        }
                    }
                });
                
                // 更新网络状态
                var networkInfo = SystemInfo.Network.GetNetworkStatus();
                Dispatcher.Invoke(() =>
                {
                    if (!_isDragging)
                    {
                        NetworkStatusText.Text = networkInfo.Status;
                        NetworkDetailsText.Text = GetNetworkDetailsText(networkInfo);
                        
                        if (networkInfo.Status == "在线")
                        {
                            NetworkStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 201, 176));
                        }
                        else if (networkInfo.Status == "受限")
                        {
                            NetworkStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                        }
                        else
                        {
                            NetworkStatusText.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                });
                
                // 尝试获取CPU温度
                var cpuTemp = SystemInfo.Temperature.GetCPUTemperature();
                if (cpuTemp.HasValue)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (!_isDragging)
                        {
                            CpuDetailsText.Text = $"使用率 • {cpuTemp:F0}°C";
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新系统信息时出错: {ex.Message}");
            }
        }

        private string GetNetworkDetailsText((bool IsAvailable, bool HasInternet, string Status) networkInfo)
        {
            switch (networkInfo.Status)
            {
                case "在线":
                    return "网络正常";
                case "受限":
                    return "网络受限";
                case "离线":
                    return "网络断开";
                default:
                    return "检查失败";
            }
        }

        // 更新CPU使用率 - 改进版
        private void UpdateCPUInfo()
        {
            Task.Run(() =>
            {
                try
                {
                    float cpuUsage = 0;
                    
                    // 尝试多种方法获取CPU使用率
                    try
                    {
                        using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                        {
                            cpuCounter.NextValue();
                            System.Threading.Thread.Sleep(500);
                            cpuUsage = cpuCounter.NextValue();
                        }
                    }
                    catch
                    {
                        // 备用方法
                        cpuUsage = GetCPUUsageAlternative();
                    }
                    
                    Dispatcher.Invoke(() =>
                    {
                        CpuUsageText.Text = $"{cpuUsage:F0}%";
                        CpuDetailsText.Text = "使用率";
                        
                        // 根据CPU使用率设置颜色
                        if (cpuUsage > 80)
                        {
                            CpuUsageText.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else if (cpuUsage > 60)
                        {
                            CpuUsageText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // 橙色
                        }
                        else
                        {
                            CpuUsageText.Foreground = new SolidColorBrush(Colors.White);
                        }
                    });
                }
                catch
                {
                    Dispatcher.Invoke(() =>
                    {
                        CpuUsageText.Text = "N/A";
                        CpuDetailsText.Text = "获取失败";
                    });
                }
            });
        }

        // 备用CPU获取方法
        private float GetCPUUsageAlternative()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
                
                System.Threading.Thread.Sleep(500);
                
                var endTime = DateTime.UtcNow;
                var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
                
                var cpuUsedMs = endCpuUsage - startCpuUsage;
                var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
                
                return (float)(cpuUsageTotal * 100);
            }
            catch
            {
                return 0;
            }
        }

        // 更新内存信息 - 改进版
        private void UpdateMemoryInfo()
        {
            Task.Run(() =>
            {
                try
                {
                    // 获取物理内存信息
                    var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                    var totalMemory = computerInfo.TotalPhysicalMemory;
                    var availableMemory = computerInfo.AvailablePhysicalMemory;
                    var usedMemory = totalMemory - availableMemory;
                    
                    var totalGB = Math.Round(totalMemory / 1024.0 / 1024.0 / 1024.0, 1);
                    var usedGB = Math.Round(usedMemory / 1024.0 / 1024.0 / 1024.0, 1);
                    var availableGB = Math.Round(availableMemory / 1024.0 / 1024.0 / 1024.0, 1);
                    
                    Dispatcher.Invoke(() =>
                    {
                        MemoryUsageText.Text = $"{usedGB}GB";
                        MemoryDetailsText.Text = $"已用 / {totalGB}GB";
                        
                        // 根据内存使用率设置颜色
                        var memoryUsagePercent = (usedMemory / totalMemory) * 100;
                        if (memoryUsagePercent > 85)
                        {
                            MemoryUsageText.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else if (memoryUsagePercent > 70)
                        {
                            MemoryUsageText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // 橙色
                        }
                        else
                        {
                            MemoryUsageText.Foreground = new SolidColorBrush(Colors.White);
                        }
                    });
                }
                catch
                {
                    // 备用方法
                    try
                    {
                        var gc = GC.GetTotalMemory(false);
                        var workingSet = Environment.WorkingSet;
                        var usedGB = Math.Round(workingSet / 1024.0 / 1024.0 / 1024.0, 1);
                        
                        Dispatcher.Invoke(() =>
                        {
                            MemoryUsageText.Text = $"{usedGB}GB";
                            MemoryDetailsText.Text = "应用内存使用";
                        });
                    }
                    catch
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MemoryUsageText.Text = "N/A";
                            MemoryDetailsText.Text = "获取失败";
                        });
                    }
                }
            });
        }

        // 更新磁盘信息 - 改进版
        private void UpdateDiskInfo()
        {
            Task.Run(() =>
            {
                try
                {
                    // 获取所有驱动器信息，显示主系统盘
                    var systemDrive = System.IO.Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C";
                    var drive = System.IO.DriveInfo.GetDrives()
                        .FirstOrDefault(d => d.Name.StartsWith(systemDrive, StringComparison.OrdinalIgnoreCase));
                    
                    if (drive != null && drive.IsReady)
                    {
                        var freeBytes = drive.AvailableFreeSpace;
                        var totalBytes = drive.TotalSize;
                        var usedBytes = totalBytes - freeBytes;
                        
                        var freeGB = Math.Round(freeBytes / 1024.0 / 1024.0 / 1024.0, 1);
                        var usedGB = Math.Round(usedBytes / 1024.0 / 1024.0 / 1024.0, 1);
                        var totalGB = Math.Round(totalBytes / 1024.0 / 1024.0 / 1024.0, 1);
                        
                        Dispatcher.Invoke(() =>
                        {
                            DiskUsageText.Text = $"{freeGB}GB";
                            DiskDetailsText.Text = $"可用 / {totalGB}GB ({drive.Name})";
                            
                            // 根据磁盘使用率设置颜色
                            var diskUsagePercent = (usedBytes / totalBytes) * 100;
                            if (diskUsagePercent > 90)
                            {
                                DiskUsageText.Foreground = new SolidColorBrush(Colors.Red);
                            }
                            else if (diskUsagePercent > 80)
                            {
                                DiskUsageText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // 橙色
                            }
                            else
                            {
                                DiskUsageText.Foreground = new SolidColorBrush(Colors.White);
                            }
                        });
                    }
                    else
                    {
                        throw new Exception("无法获取磁盘信息");
                    }
                }
                catch
                {
                    Dispatcher.Invoke(() =>
                    {
                        DiskUsageText.Text = "N/A";
                        DiskDetailsText.Text = "获取失败";
                    });
                }
            });
        }

        // 更新网络状态
        private void UpdateNetworkInfo()
        {
            try
            {
                bool isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
                bool hasInternetConnection = CheckInternetConnection();
                
                Dispatcher.Invoke(() =>
                {
                    if (hasInternetConnection)
                    {
                        NetworkStatusText.Text = "在线";
                        NetworkStatusText.Foreground = new SolidColorBrush(Color.FromRgb(78, 201, 176)); // 绿色 #4EC9B0
                        NetworkDetailsText.Text = "网络正常";
                    }
                    else if (isNetworkAvailable)
                    {
                        NetworkStatusText.Text = "受限";
                        NetworkStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)); // 橙色
                        NetworkDetailsText.Text = "网络受限";
                    }
                    else
                    {
                        NetworkStatusText.Text = "离线";
                        NetworkStatusText.Foreground = new SolidColorBrush(Colors.Red);
                        NetworkDetailsText.Text = "网络断开";
                    }
                });
            }
            catch
            {
                Dispatcher.Invoke(() =>
                {
                    NetworkStatusText.Text = "N/A";
                    NetworkDetailsText.Text = "检查失败";
                });
            }
        }

        // 检查网络连接
        private bool CheckInternetConnection()
        {
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var reply = ping.Send("8.8.8.8", 1000); // ping Google DNS
                    return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }
        
        // 主窗口关闭事件处理
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 关闭所有子窗口
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }
        }
    }
}
