using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Snap.Hutao.Installer;

internal class Program
{
    private const string AppxKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock";
    private const string ValueName = "AllowDevelopmentWithoutDevLicense";

    public static async Task Main(string[] args)
    {
        _ = args;
        string ps1File = Path.Combine(AppContext.BaseDirectory, "Install.ps1");

        if (!File.Exists(ps1File))
        {
            Console.WriteLine("未检测到 Install.ps1 文件");
            Console.WriteLine("请勿移动该安装程序，按下任意键退出...");
            Console.ReadKey();
            return;
        }

        try
        {
            //以管理策略打开开发者模式
            Registry.SetValue(AppxKey, ValueName, 1, RegistryValueKind.DWord);
        }
        catch (Exception)
        {
            Console.WriteLine("开发者模式未开启，请手动开启，参阅下方链接");
            Console.WriteLine("https://learn.microsoft.com/zh-CN/windows/apps/get-started/developer-mode-features-and-debugging");
        }

        await InstallAsync(ps1File).ConfigureAwait(false);

        Console.WriteLine();
        Console.WriteLine("官方文档与使用教程");
        Console.WriteLine("https://hut.ao");
        Console.WriteLine();
        Console.WriteLine("在开始菜单中启动 Snap.Hutao ，按下任意键退出...");
        Console.ReadKey();
    }

    private static async Task InstallAsync(string ps1File)
    {
        Console.WriteLine("请注意 PowerShell 中的提示");
        Process ps = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Unrestricted \"{ps1File}\"",
                UseShellExecute = true,
            }
        };

        try
        {
            ps.Start();
            await ps.WaitForExitAsync();
            Console.WriteLine("安装脚本运行完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

    }
}
