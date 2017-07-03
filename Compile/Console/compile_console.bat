@echo off

if "%MAIN_PATH%" == "" ( for %%a in ("%~dp0../..") do set "MAIN_PATH=%%~fa" )
if "%REF_PATH%" == "" ( set "REF_PATH=%MAIN_PATH%\Assemblies" )
::set "msbuildPath=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64"
set "MSBuild=%MAIN_PATH%\Apps\MSBuild\4.0"

"%MSBuild%\MSBuild.exe" "%MAIN_PATH%\Lerp2API.sln" /p:ReferencePath="%REF_PATH%\Lerp2API.dll; %REF_PATH%\UnityEngine.dll" /t:Lerp2Console /p:OutputPath=../../Build/Console
::/p:ReferencePath="%REF_PATH%\Lerp2API.dll; %MAIN_PATH%\Compile\Console\Assemblies"

if exist "%MAIN_PATH%\Build\Console\Lerp2API.dll" ( del "%MAIN_PATH%\Build\Console\Lerp2API.dll" )
if exist "%MAIN_PATH%\Build\Console\Lerp2API.pdb" ( del "%MAIN_PATH%\Build\Console\Lerp2API.pdb" )
if exist "%MAIN_PATH%\Build\Console\Lerp2API.xml" ( del "%MAIN_PATH%\Build\Console\Lerp2API.xml" )
if exist "%MAIN_PATH%\Build\Console\UnityEngine.dll" ( del "%MAIN_PATH%\Build\Console\UnityEngine.dll" )
if exist "%MAIN_PATH%\Build\Console\UnityEditor.dll" ( del "%MAIN_PATH%\Build\Console\UnityEditor.dll" )
if exist "%MAIN_PATH%\Build\Console\UnityEngine.UI.dll" ( del "%MAIN_PATH%\Build\Console\UnityEngine.UI.dll" )
if exist "%MAIN_PATH%\Build\Console\Lerp2Raw.dll" ( del "%MAIN_PATH%\Build\Console\Lerp2Raw.dll" )