# PowerShell script to capture WPF window screenshot
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

function Capture-Window {
    param(
        [string]$WindowTitle = "TrainMeX Launcher",
        [string]$OutputPath = "screenshot.png"
    )
    
    # Find the window
    Add-Type @"
        using System;
        using System.Runtime.InteropServices;
        public class Win32 {
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            
            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
            
            [DllImport("user32.dll")]
            public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        }
        
        public struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
"@
    
    $hwnd = [Win32]::FindWindow($null, $WindowTitle)
    
    if ($hwnd -eq [IntPtr]::Zero) {
        Write-Host "Window '$WindowTitle' not found. Make sure the app is running."
        return $false
    }
    
    $rect = New-Object RECT
    [Win32]::GetWindowRect($hwnd, [ref]$rect) | Out-Null
    
    $width = $rect.Right - $rect.Left
    $height = $rect.Bottom - $rect.Top
    
    $bitmap = New-Object System.Drawing.Bitmap $width, $height
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $hdc = $graphics.GetHdc()
    
    [Win32]::PrintWindow($hwnd, $hdc, 0) | Out-Null
    
    $graphics.ReleaseHdc($hdc)
    $graphics.Dispose()
    
    $bitmap.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bitmap.Dispose()
    
    Write-Host "Screenshot saved to: $OutputPath"
    return $true
}

# Capture the screenshot
Capture-Window -WindowTitle "TrainMeX Launcher" -OutputPath "trainmex-screenshot.png"
