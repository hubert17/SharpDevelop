@echo off
setlocal enabledelayedexpansion

echo ===================================================
echo   SharpDevelop Installer and Portable Build Generator
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

:: Create dist directory if it doesn't exist
if not exist dist (
  mkdir dist
)

:: Run MSBuild with Automated.proj targets to build and package both MSI installer and Portable Zip
echo Starting build and package process...
"%msbuild%" src\Automated.proj /p:MSBuildExecutable="\"%msbuild%\"" /p:ArtefactsOutputDir="%CD%\dist" /p:BranchName=master /t:clean1;init;build;copyprecompiled;createsetup;createxcopydeployablezip

if %ERRORLEVEL% NEQ 0 (
  echo.
  echo Error: Build failed! Please check the errors above.
  pause
  exit /b %ERRORLEVEL%
)

echo.
echo ===================================================
echo   Build Successful!
echo ===================================================
echo Outputs are located in the "dist" folder:
dir dist
echo.
pause
