using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace XC_Studio
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _mainWindow;
        private bool _windowPreLoaded = false;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 显示启动界面
            SplashWindow splash = new SplashWindow();
            splash.Show();
            
            // 在UI线程中预加载主窗口，但不显示
            Dispatcher.BeginInvoke(new Action(() => {
                _mainWindow = new MainWindow();
                _mainWindow.WindowState = WindowState.Minimized; // 先最小化，不显示
                _mainWindow.ShowInTaskbar = false; // 不在任务栏显示
                _mainWindow.Show();
                _windowPreLoaded = true;
            }), System.Windows.Threading.DispatcherPriority.Background);
            
            // 订阅启动画面关闭事件
            splash.Closed += (sender, args) =>
            {
                // 在启动画面完全关闭后，显示已加载的主窗口
                if (_windowPreLoaded && _mainWindow != null)
                {
                    _mainWindow.WindowState = WindowState.Normal; // 恢复正常窗口状态
                    _mainWindow.ShowInTaskbar = true; // 在任务栏显示
                    _mainWindow.Activate(); // 激活窗口
                    
                    // 使用更简单的方式显示公告窗口
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // 短暂延迟确保主窗口完全显示
                            System.Threading.Thread.Sleep(100);
                           
                            // 检查是否应该显示公告
                            if (AnnouncementWindow.ShouldShowAnnouncement())
                            {
                                var announcement = new AnnouncementWindow();
                                announcement.Show();
                            }
                        }
                        catch (Exception ex)
                        {
                            // 记录错误但不影响程序运行
                            System.Diagnostics.Debug.WriteLine($"显示公告窗口时出错: {ex.Message}");
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            };
        }
    }
}