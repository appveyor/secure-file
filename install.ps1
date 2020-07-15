$ProgressPreference='SilentlyContinue'

$tempdir = Join-Path $([System.IO.Path]::GetTempPath()) $([System.Guid]::NewGuid())
New-Item -ItemType Directory -Path $tempdir

$zipPath = Join-Path $tempdir 'secure-file.zip'
[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls"
(New-Object Net.WebClient).DownloadFile('https://github.com/appveyor/secure-file/releases/download/1.0.1/secure-file.zip', $zipPath)
Expand-Archive $zipPath -DestinationPath (Join-Path (pwd).path "appveyor-tools") -Force
if (-Not ($isWindows)) {
	chmod +x ./appveyor-tools/secure-file
}
del $tempdir -Recurse -Force
