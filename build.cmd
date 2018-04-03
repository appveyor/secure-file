rd /s /q build
dotnet publish secure-file -f netcoreapp2.0 -o ..\build -c Release
dotnet publish secure-file -f net462 -o ..\build -c Release