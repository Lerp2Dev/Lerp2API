@echo off

::Use this in case, you haven't VS installed, or you wan't to open it!
::Maybe you need to install this, if you don't have VS 2015 or MSBuild packages: https://www.microsoft.com/en-us/download/details.aspx?id=48159

::Main

if "%MAIN_PATH%" == "" ( for %%a in ("%~dp0..") do set "MAIN_PATH=%%~fa" )
if "%REF_PATH%" == "" ( set "REF_PATH=%MAIN_PATH%\Assemblies" )

set MSBuildEmitSolution=1

set "CompileOrder=%~2"
if "%CompileOrder%" NEQ "" (
	FOR /f "tokens=1,2,3 delims= " %%a IN ("%CompileOrder%") do (
		call :build%%a
		call :build%%b
		call :build%%c
	)
) else (
	FOR /L %%i IN (1,1,3) DO (
		call :build%%i
	)
)

::Build Console

CMD.exe /C "%MAIN_PATH%/Compile/Console/compile_console.bat"

set "ProjectPath=%~1"
if "%ProjectPath%" NEQ "" call :copycontent "%ProjectPath%"

pause

goto :EOF

::Build API

:build1
MSBuild "%MAIN_PATH%\Lerp2API.sln" /t:Lerp2API /pp "/p:ReferencePath=%REF_PATH%" /p:Platform="Any CPU" /p:OutputPath=../Build /p:Configuration=Debug

if exist "%MAIN_PATH%\Build\Lerp2API.dll" ( xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.dll" "%MAIN_PATH%\Assemblies\Lerp2API.dll" )

if exist "%MAIN_PATH%\Build\Lerp2APIEditor.dll" ( del "%MAIN_PATH%\Build\Lerp2APIEditor.dll" )
if exist "%MAIN_PATH%\Build\Lerp2APIEditor.pdb" ( del "%MAIN_PATH%\Build\Lerp2APIEditor.pdb" )
if exist "%MAIN_PATH%\Build\Lerp2APIEditor.xml" ( del "%MAIN_PATH%\Build\Lerp2APIEditor.xml" )
if exist "%MAIN_PATH%\Build\UnityEngine.dll" ( del "%MAIN_PATH%\Build\UnityEngine.dll" )
if exist "%MAIN_PATH%\Build\UnityEngine.xml" ( del "%MAIN_PATH%\Build\UnityEngine.xml" )
if exist "%MAIN_PATH%\Build\UnityEditor.dll" ( del "%MAIN_PATH%\Build\UnityEditor.dll" )
if exist "%MAIN_PATH%\Build\UnityEditor.xml" ( del "%MAIN_PATH%\Build\UnityEditor.xml" )
if exist "%MAIN_PATH%\Build\UnityEngine.UI.dll" ( del "%MAIN_PATH%\Build\UnityEngine.UI.dll" )
if exist "%MAIN_PATH%\Build\UnityEngine.UI.xml" ( del "%MAIN_PATH%\Build\UnityEngine.UI.xml" )
if exist "%MAIN_PATH%\Build\Lerp2Raw.dll" ( del "%MAIN_PATH%\Build\Lerp2Raw.dll" )
if exist "%MAIN_PATH%\Build\Lerp2Raw.pdb" ( del "%MAIN_PATH%\Build\Lerp2Raw.pdb" )
if exist "%MAIN_PATH%\Build\Lerp2Raw.xml" ( del "%MAIN_PATH%\Build\Lerp2Raw.xml" )
goto :EOF

::Build RAW

:build2
MSBuild "%MAIN_PATH%\Lerp2API.sln" /t:Lerp2Raw /pp "/p:ReferencePath=%REF_PATH%" /p:Platform="Any CPU" /p:OutputPath=../Build/Raw /p:Configuration=Debug

if exist "%MAIN_PATH%\Build\Raw\Lerp2APIRaw.dll" ( xcopy /e /y "%MAIN_PATH%\Build\Raw\Lerp2Raw.dll" "%MAIN_PATH%\Assemblies\Lerp2Raw.dll" )

if exist "%MAIN_PATH%\Build\Raw\Lerp2API.dll" ( del "%MAIN_PATH%\Build\Raw\Lerp2API.dll" )
if exist "%MAIN_PATH%\Build\Raw\Lerp2API.pdb" ( del "%MAIN_PATH%\Build\Raw\Lerp2API.pdb" )
if exist "%MAIN_PATH%\Build\Raw\Lerp2API.xml" ( del "%MAIN_PATH%\Build\Raw\Lerp2API.xml" )
if exist "%MAIN_PATH%\Build\Raw\Lerp2APIEditor.dll" ( del "%MAIN_PATH%\Build\Raw\Lerp2APIEditor.dll" )
if exist "%MAIN_PATH%\Build\Raw\Lerp2APIEditor.pdb" ( del "%MAIN_PATH%\Build\Raw\Lerp2APIEditor.pdb" )
if exist "%MAIN_PATH%\Build\Raw\Lerp2APIEditor.xml" ( del "%MAIN_PATH%\Build\Raw\Lerp2APIEditor.xml" )
if exist "%MAIN_PATH%\Build\Raw\UnityEngine.dll" ( del "%MAIN_PATH%\Build\Raw\UnityEngine.dll" )
if exist "%MAIN_PATH%\Build\Raw\UnityEngine.xml" ( del "%MAIN_PATH%\Build\Raw\UnityEngine.xml" )
if exist "%MAIN_PATH%\Build\Raw\UnityEditor.dll" ( del "%MAIN_PATH%\Build\Raw\UnityEditor.dll" )
if exist "%MAIN_PATH%\Build\Raw\UnityEditor.xml" ( del "%MAIN_PATH%\Build\Raw\UnityEditor.xml" )
if exist "%MAIN_PATH%\Build\Raw\UnityEngine.UI.dll" ( del "%MAIN_PATH%\Build\Raw\UnityEngine.UI.dll" )
if exist "%MAIN_PATH%\Build\Raw\UnityEngine.UI.xml" ( del "%MAIN_PATH%\Build\Raw\UnityEngine.UI.xml" )
goto :EOF

::Build Editor

:build3
MSBuild "%MAIN_PATH%\Project\Editor\Lerp2APIEditor.csproj" "/p:ReferencePath=%REF_PATH%" /p:Platform="Any CPU" /p:OutputPath=../../Build/Editor /p:Configuration=Debug

if exist "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" ( xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" "%MAIN_PATH%\Assemblies\Lerp2APIEditor.dll" )

if exist "%MAIN_PATH%\Build\Editor\Lerp2API.dll" ( del "%MAIN_PATH%\Build\Editor\Lerp2API.dll" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2API.pdb" ( del "%MAIN_PATH%\Build\Editor\Lerp2API.pdb" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2API.xml" ( del "%MAIN_PATH%\Build\Editor\Lerp2API.xml" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2Raw.dll" ( del "%MAIN_PATH%\Build\Editor\Lerp2Raw.dll" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2Raw.pdb" ( del "%MAIN_PATH%\Build\Editor\Lerp2Raw.pdb" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2Raw.xml" ( del "%MAIN_PATH%\Build\Editor\Lerp2Raw.xml" )
if exist "%MAIN_PATH%\Build\Editor\UnityEngine.dll" ( del "%MAIN_PATH%\Build\Editor\UnityEngine.dll" )
if exist "%MAIN_PATH%\Build\Editor\UnityEngine.xml" ( del "%MAIN_PATH%\Build\Editor\UnityEngine.xml" )
if exist "%MAIN_PATH%\Build\Editor\UnityEditor.dll" ( del "%MAIN_PATH%\Build\Editor\UnityEditor.dll" )
if exist "%MAIN_PATH%\Build\Editor\UnityEditor.xml" ( del "%MAIN_PATH%\Build\Editor\UnityEditor.xml" )
if exist "%MAIN_PATH%\Build\Editor\UnityEngine.UI.dll" ( del "%MAIN_PATH%\Build\Editor\UnityEngine.UI.dll" )
if exist "%MAIN_PATH%\Build\Editor\UnityEngine.UI.xml" ( del "%MAIN_PATH%\Build\Editor\UnityEngine.UI.xml" )
goto :EOF

::Copy everything

:copycontent
xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.dll" "%ProjectPath%\Lerp2API.dll"
xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.pdb" "%ProjectPath%\Lerp2API.pdb"
xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.xml" "%ProjectPath%\Lerp2API.xml"

xcopy /e /y "%MAIN_PATH%\Build\Console\Lerp2Console.exe" "%ProjectPath%\Console\Lerp2Console.exe"
xcopy /e /y "%MAIN_PATH%\Build\Console\Lerp2Console.exe.config" "%ProjectPath%\Console\Lerp2Console.exe.config"
xcopy /e /y "%MAIN_PATH%\Build\Console\Lerp2Console.xml" "%ProjectPath%\Console\Lerp2Console.xml"
xcopy /e /y "%MAIN_PATH%\Build\Console\Lerp2Console.pdb" "%ProjectPath%\Console\Lerp2Console.pdb"

xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" "%ProjectPath%\Editor\Lerp2APIEditor.dll"
xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.pdb" "%ProjectPath%\Editor\Lerp2APIEditor.pdb"
xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.xml" "%ProjectPath%\Editor\Lerp2APIEditor.xml"

xcopy /e /y "%MAIN_PATH%\Project\Lerp2Raw\*.cs" "%ProjectPath%\AttachedScripts\"

goto:EOF

::Other params that you will need if you modify this code and put new dlls, or maybe, if the dlls aren't found in your hdd
::/t:Compile /p:Configuration=Release "/p:ReferencePath=%REF_PATH%"