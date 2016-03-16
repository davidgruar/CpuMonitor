# Adapted from https://www.devtxt.com/blog/install-a-windows-service-using-powershell

$serviceName = "CPUMonitorService"
$displayName = "CPU Monitor Service"
$bin = "$PSScriptRoot\CpuMonitor.WindowsService\bin\debug"
$outputDir = "$PSScriptRoot\CpuMonitor.WindowsService\bin\service"
$exeName = "CpuMonitor.WindowsService.exe"

$user = [Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()
if (-not $user.IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Warning "You must run this script as an administrator!"
    break
}

if (-not (Test-Path "$bin\$exeName"))
{
	"$bin\$exeName not found. Have you compiled the project yet?"
}

$existingService = Get-WmiObject -Class Win32_Service -Filter "Name='$serviceName'"

if ($existingService) 
{
  "'$serviceName' exists already. Stopping."
  Stop-Service $serviceName
  "Waiting 3 seconds to allow existing service to stop."
  Start-Sleep -s 3

  $existingService.Delete()
  "Waiting 5 seconds to allow service to be uninstalled."
  Start-Sleep -s 5  
}

if (-not (Test-Path $outputDir))
{
	Write-Host "Creating directory $outputDir"
	New-Item $outputDir -ItemType Directory
}
Write-Host "Copying binaries"
Remove-Item -Path "$outputDir\*.*" -Recurse
Copy-Item -Path "$bin\*.*" -Destination $outputDir

"Installing the service."
New-Service -BinaryPathName "$outputDir\$exeName" -Name $serviceName -DisplayName $displayName -StartupType Automatic
"Installed the service."

services.msc

#$ShouldStartService = Read-Host "Would you like the '$serviceName ' service started? Y or N"
#if($ShouldStartService -eq "Y")
#{
#    "Starting the service."
#    Start-Service $serviceName
#}
#"Completed."