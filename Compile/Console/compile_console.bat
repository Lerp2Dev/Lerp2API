@echo off

if "%MAIN_PATH%" == "" ( for %%a in ("%~dp0../..") do set "MAIN_PATH=%%~fa" )
if "%REF_PATH%" == "" ( set "REF_PATH=%MAIN_PATH%\Assemblies" )
::set "msbuildPath=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\amd64"

MSBuild "%MAIN_PATH%\Lerp2API.sln" /t:Lerp2Console /pp "/p:ReferencePath=%REF_PATH%" /p:Platform="Any CPU" /p:OutputPath=../../Build/Console /p:Configuration=Debug /flp:logfile=Output.log;verbosity=diagnostic  

pause