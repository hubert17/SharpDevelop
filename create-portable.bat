@echo off
setlocal enabledelayedexpansion

echo ===================================================
echo   SharpDevelop Quick Portable Build Packager
echo ===================================================
echo.

:: Find MSBuild
for /f "usebackq tokens=*" %%i in (`src\Tools\VSWhere\vswhere.exe -latest -products * -prerelease -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
  set msbuild=%%i
)

if not defined msbuild (
  echo Error: MSBuild.exe could not be found via vswhere.exe.
  echo Please make sure Build Tools or Visual Studio is installed.
  pause
  exit /b 1
)

echo Found MSBuild at: "%msbuild%"
echo.

:: Compile ZipFromMsi helper if needed
if not exist src\Tools\ZipFromMsi\bin\Debug\ZipFromMsi.exe (
  echo Compiling ZipFromMsi helper...
  "%msbuild%" src\Tools\ZipFromMsi\ZipFromMsi.csproj /p:Configuration=Debug
  if !ERRORLEVEL! NEQ 0 (
    echo Error: Failed to compile ZipFromMsi.
    pause
    exit /b !ERRORLEVEL!
  )
)

:: Run ZipFromMsi to package current bin directory
echo Packaging portable zip...
pushd src\Tools\ZipFromMsi\bin\Debug
ZipFromMsi.exe
popd

:: Move the zip file to the dist directory
if exist src\Tools\ZipFromMsi\bin\Debug\SharpDevelopStandalone.zip (
  if not exist dist (
    mkdir dist
  )
  if exist dist\SharpDevelopPortable.zip (
    del dist\SharpDevelopPortable.zip
  )
  move src\Tools\ZipFromMsi\bin\Debug\SharpDevelopStandalone.zip dist\SharpDevelopPortable.zip
  echo.
  echo ===================================================
  echo   Portable build created successfully!
  echo   Output: dist\SharpDevelopPortable.zip
  echo ===================================================
) else (
  echo Error: Portable zip could not be generated.
)

echo.
pause
