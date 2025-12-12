using System;
using System.Windows;

namespace XC_Studio
{
    /// <summary>
    /// AnnouncementWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AnnouncementWindow : Window
    {
        public AnnouncementWindow()
        {
            InitializeComponent();
            
            // 简化窗口加载逻辑
            this.Loaded += (s, e) =>
            {
                try
                {
                    if (Application.Current?.MainWindow != null)
                    {
                        this.Owner = Application.Current.MainWindow;
                        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    }
                }
                catch
                {
                    // 如果设置所有者失败，保持默认居中
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
                // 如果拖动失败，忽略错误
            }
        }

        private void DontShowAgainButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 使用简单的本地存储，避免配置文件问题
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                Properties.Settings.Default.DontShowAnnouncementToday = true;
                Properties.Settings.Default.LastAnnouncementDate = DateTime.Today;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                // 记录错误但不影响程序运行
                System.Diagnostics.Debug.WriteLine($"保存设置时出错: {ex.Message}");
            }
            
            this.Close();
        }

        // 简化公告显示检查逻辑
        public static bool ShouldShowAnnouncement()
        {
            try
            {
                // 检查设置是否存在和可用
                if (Properties.Settings.Default == null)
                    return true;

                // 检查今日是否已设置不显示
                if (Properties.Settings.Default.DontShowAnnouncementToday)
                {
                    var lastDate = Properties.Settings.Default.LastAnnouncementDate;
                    if (lastDate.Date == DateTime.Today.Date)
                        return false;
                }

                return true;
            }
            catch
            {
                // 出现任何错误都显示公告
                return true;
            }
        }
    }
}