function Get-VsVersion ()
{
    if ($env:VS120COMNTOOLS -ne $null)
    {
        " /property:VisualStudioVersion=12.0"
    }
}

Start-Process -NoNewWindow -Wait -FilePath $env:windir\Microsoft.NET\Framework\v4.0.30319\MSBuild -ArgumentList ('BuildRelease.msbuild', '/property:Configuration=Release', (Get-VsVersion))