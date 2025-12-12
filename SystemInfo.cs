using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace XC_Studio
{
    public class SystemInfo
    {
        // Windows API调用获取准确的内存信息
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        // 获取磁盘信息
        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_SPACE_INFORMATION
        {
            public ulong TotalBytes;
            public ulong FreeBytes;
            public ulong AvailableBytes;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
            out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes,
            out ulong lpTotalNumberOfFreeBytes);

        public static class Memory
        {
            public static (double TotalGB, double UsedGB, double AvailableGB, double UsagePercent) GetPhysicalMemory()
            {
                try
                {
                    var memStatus = new MEMORYSTATUSEX();
                    memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                    
                    if (GlobalMemoryStatusEx(ref memStatus))
                    {
                        var totalGB = memStatus.ullTotalPhys / 1024.0 / 1024.0 / 1024.0;
                        var availableGB = memStatus.ullAvailPhys / 1024.0 / 1024.0 / 1024.0;
                        var usedGB = totalGB - availableGB;
                        var usagePercent = (usedGB / totalGB) * 100;
                        
                        return (totalGB, usedGB, availableGB, usagePercent);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"获取内存信息失败: {ex.Message}");
                }
                
                return (0, 0, 0, 0);
            }
        }

        public static class Disk
        {
            public static (double TotalGB, double FreeGB, double UsedGB, double UsagePercent, string DriveName) GetSystemDiskInfo()
            {
                try
                {
                    var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)?.TrimEnd('\\') ?? "C";
                    
                    if (GetDiskFreeSpaceEx(systemDrive, out var freeBytes, out var totalBytes, out var _))
                    {
                        var totalGB = totalBytes / 1024.0 / 1024.0 / 1024.0;
                        var freeGB = freeBytes / 1024.0 / 1024.0 / 1024.0;
                        var usedGB = totalGB - freeGB;
                        var usagePercent = (usedGB / totalGB) * 100;
                        
                        return (totalGB, freeGB, usedGB, usagePercent, $"{systemDrive}:\\");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"获取磁盘信息失败: {ex.Message}");
                }
                
                return (0, 0, 0, 0, "N/A");
            }
            
            public static (double TotalGB, double FreeGB, double UsedGB, int DriveCount) GetAllDisksInfo()
            {
                try
                {
                    var allDrives = DriveInfo.GetDrives()
                        .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                        .ToList();
                    
                    if (allDrives.Count == 0)
                    {
                        return (0, 0, 0, 0);
                    }
                    
                    double totalAllGB = 0;
                    double freeAllGB = 0;
                    double usedAllGB = 0;
                    
                    foreach (var drive in allDrives)
                    {
                        if (drive.IsReady)
                        {
                            var totalBytes = drive.TotalSize;
                            var freeBytes = drive.AvailableFreeSpace;
                            
                            totalAllGB += totalBytes / 1024.0 / 1024.0 / 1024.0;
                            freeAllGB += freeBytes / 1024.0 / 1024.0 / 1024.0;
                            usedAllGB += (totalBytes - freeBytes) / 1024.0 / 1024.0 / 1024.0;
                        }
                    }
                    
                    return (totalAllGB, freeAllGB, usedAllGB, allDrives.Count);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"获取所有磁盘信息失败: {ex.Message}");
                    return (0, 0, 0, 0);
                }
            }
        }

        public static class CPU
        {
            private static PerformanceCounter _cpuCounter;
            private static bool _initialized = false;

            public static async Task<float> GetUsagePercentage()
            {
                if (!_initialized)
                {
                    try
                    {
                        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                        _cpuCounter.NextValue(); // 第一次调用
                        await Task.Delay(1000); // 等待1秒
                        _initialized = true;
                    }
                    catch
                    {
                        return 0;
                    }
                }

                try
                {
                    return _cpuCounter.NextValue();
                }
                catch
                {
                    // 备用方法
                    return GetCPUUsageByProcess();
                }
            }

            private static float GetCPUUsageByProcess()
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
        }

        public static class Network
        {
            public static (bool IsAvailable, bool HasInternet, string Status) GetNetworkStatus()
            {
                try
                {
                    var isAvailable = NetworkInterface.GetIsNetworkAvailable();
                    
                    if (!isAvailable)
                    {
                        return (false, false, "离线");
                    }

                    // 检测互联网连接
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var reply = ping.Send("8.8.8.8", 2000);
                            if (reply.Status == IPStatus.Success)
                            {
                                return (true, true, "在线");
                            }
                        }
                    }
                    catch
                    {
                        // 尝试其他DNS
                        try
                        {
                            using (var ping = new Ping())
                            {
                                var reply = ping.Send("114.114.114.114", 2000);
                                if (reply.Status == IPStatus.Success)
                                {
                                    return (true, true, "在线");
                                }
                            }
                        }
                        catch
                        {
                            return (true, false, "受限");
                        }
                    }

                    return (true, false, "受限");
                }
                catch
                {
                    return (false, false, "未知");
                }
            }
        }

        public static class Temperature
        {
            // 尝试获取CPU温度（需要管理员权限）
            public static double? GetCPUTemperature()
            {
                try
                {
                    // 使用WMI查询CPU温度
                    var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                    
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var temp = Convert.ToDouble(obj["CurrentTemperature"]);
                        // 从开尔文转换为摄氏度
                        return (temp - 273.15) / 10;
                    }
                }
                catch
                {
                    // WMI可能不可用或权限不足
                    return null;
                }

                return null;
            }
        }
    }
}