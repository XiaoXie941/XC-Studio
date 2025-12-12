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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace XC_Studio
{
    /// <summary>
    /// SplashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SplashWindow : Window
    {
        private DispatcherTimer _timer;
        private string[] _statusMessages = {
            "正在初始化组件...",
            "正在加载配置文件...",
            "正在检查更新...",
            "正在准备用户界面...",
            "即将完成..."
        };
        
        private int _currentMessageIndex = 0;

        public SplashWindow()
        {
            InitializeComponent();
            InitializeTimer();
            
            // 设置淡入动画
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");
            fadeIn.Begin(this);
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(400);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 更新状态消息
            if (_currentMessageIndex < _statusMessages.Length)
            {
                StatusText.Text = _statusMessages[_currentMessageIndex];
                _currentMessageIndex++;
            }
            
            // 更新进度条宽度 (总宽度640，总共约3秒，每400ms增加约85)
            if (_currentMessageIndex <= _statusMessages.Length)
            {
                double progressWidth = (_currentMessageIndex * 640.0) / _statusMessages.Length;
                ProgressBar.Width = progressWidth;
            }
            
            // 所有消息显示完后，再等待2个周期后关闭
            if (_currentMessageIndex >= _statusMessages.Length + 2)
            {
                _timer.Stop();
                CloseSplashWindow();
            }
        }

        private void CloseSplashWindow()
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            fadeOut.Completed += (s, e) => {
                this.Close();
            };
            fadeOut.Begin(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // 按ESC键可以跳过启动界面
            if (e.Key == Key.Escape)
            {
                _timer.Stop();
                CloseSplashWindow();
            }
            base.OnKeyDown(e);
        }
    }
}