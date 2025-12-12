# XC Studio

<div align="center">
  <img src="icon/xc.ico" alt="XC Studio Logo" width="128">
  
  <h3>专业应用集成工具平台</h3>
  
  <p>
    <strong>XC Studio</strong> 是一个由谢晨工作室开发的专业级系统工具集成平台，提供系统监控、网络诊断和多种实用工具的统一访问入口。
  </p>
</div>

## ✨ 主要功能

### 🏠 系统概览
- **实时系统监控**：CPU使用率、内存占用、磁盘空间和网络状态
- **高性能刷新**：0.5秒刷新间隔，提供实时性能数据

### 🌐 网络工具
- **连接测试**：测试网站连接性和响应时间
- **接口信息**：显示所有网络接口的详细状态
- **IP解析**：自动获取并显示目标网站的IP地址

### 🎨 现代化界面
- **深色主题**：专业的暗色界面设计，减少眼部疲劳
- **无边框设计**：自定义窗口样式，提供更沉浸的体验
- **流畅动画**：精心设计的启动动画和过渡效果

## 🚀 快速开始

### 系统要求
- Windows 10/11 (x64)
- .NET Framework 4.8 或更高版本
- 至少 4GB 内存
- 50MB 可用磁盘空间

### 安装步骤
1. 从 [发布](releases) 页面下载最新版本的安装包
2. 解压到任意文件夹
3. 运行 `XC_Studio.exe`
4. 首次运行时会显示启动动画，加载完成后自动进入主界面

## 📸 截图

<div align="center">
  <img src="docs/images/main_interface.png" alt="主界面" width="800">
  <p><em>主界面 - 系统概览</em></p>
</div>

<div align="center">
  <img src="docs/images/network_tools.png" alt="网络工具" width="800">
  <p><em>网络工具 - 连接测试和接口信息</em></p>
</div>

## 🛠️ 技术架构

- **框架**：.NET Framework 4.8
- **UI框架**：WPF (Windows Presentation Foundation)
- **架构模式**：MVVM (Model-View-ViewModel)
- **语言**：C# 7.3

## 📦 项目结构

```
XC_Studio/
├── XC_Studio.csproj          # 项目文件
├── App.xaml                  # 应用程序入口
├── MainWindow.xaml            # 主界面
├── NetworkToolsWindow.xaml    # 网络工具窗口
├── SplashWindow.xaml          # 启动画面
├── AnnouncementWindow.xaml    # 公告窗口
├── SystemInfo.cs             # 系统信息获取
├── icon/                    # 应用图标资源
├── Properties/              # 应用程序属性
└── docs/                   # 文档
    └── images/              # 截图和文档图片
```

## 🔧 开发计划

### 近期更新 (v1.1)
- [ ] 文件管理器模块
- [ ] 数据库管理工具
- [ ] 安全扫描功能
- [ ] 配置文件导入/导出

### 长期规划 (v2.0)
- [ ] 插件系统
- [ ] 自定义主题
- [ ] 性能历史记录
- [ ] 远程监控功能

## 🤝 贡献指南

我们欢迎任何形式的贡献！

### 提交问题
1. 使用 [议题](issues) 页面报告bug
2. 提供详细的复现步骤
3. 附上系统信息和错误日志

## 📄 许可证

本项目基于 [MIT License](LICENSE) 开源，详见 [LICENSE](LICENSE) 文件。

## 🙏 致谢

- 感谢所有贡献者和用户的支持
- 特别感谢 .NET 和 WPF 社区提供的优秀框架
- UI 图标来自 [Flaticon](https://www.flaticon.com/)

## 📞 联系我们

- **邮箱**：3899032251@qq.com
- **QQ群**：901526593

---

<div align="center">
  <p>© 2025 谢晨工作室. 保留所有权利.</p>
  <p>Made with ❤️ by XC Studio Team</p>
</div>
