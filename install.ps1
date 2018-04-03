$ProgressPreference='SilentlyContinue'

$temp = $env:temp
if ($isLinux) {
	$temp = '/tmp'
}

$zipPath = Join-Path $temp 'secure-file.zip'
(New-Object Net.WebClient).DownloadFile('https://github.com/appveyor/secure-file/releases/download/1.0.0/secure-file.zip', $zipPath)
Expand-Archive $zipPath -DestinationPath (Join-Path (pwd).path "appveyor-tools")
if ($isLinux) {
	chmod +x ./appveyor-tools/secure-file
}
del $zipPath