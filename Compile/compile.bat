@echo off

::Use this in case, you haven't VS installed, or you wan't to open it!
::Maybe you need to install this, if you don't have VS 2015 or MSBuild packages: https://www.microsoft.com/en-us/download/details.aspx?id=48159

if "%MAIN_PATH%" == "" ( for %%a in ("%~dp0..") do set "MAIN_PATH=%%~fa" )
if "%REF_PATH%" == "" ( set "REF_PATH=%MAIN_PATH%\Assemblies" )

set MSBuildEmitSolution=1
"%~dp0\MSBuild.exe" "%MAIN_PATH%\Lerp2API.sln" /t:Lerp2API /pp "/p:ReferencePath=%REF_PATH%" /p:Platform="Any CPU" /p:OutputPath=../Build /p:Configuration=Debug

if exist "%MAIN_PATH%\Build\Lerp2API.dll" ( xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.dll" "%MAIN_PATH%\Assemblies\Lerp2API.dll" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" ( xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" "%MAIN_PATH%\Assemblies\Lerp2APIEditor.dll" )

if exist "%MAIN_PATH%\Build\Lerp2APIEditor.dll" ( del "%MAIN_PATH%\Build\Lerp2APIEditor.dll" )
if exist "%MAIN_PATH%\Build\Lerp2APIEditor.pdb" ( del "%MAIN_PATH%\Build\Lerp2APIEditor.pdb" )
if exist "%MAIN_PATH%\Build\Lerp2APIEditor.xml" ( del "%MAIN_PATH%\Build\Lerp2APIEditor.xml" )
if exist "%MAIN_PATH%\Build\UnityEditor.dll" ( del "%MAIN_PATH%\Build\UnityEditor.dll" )
if exist "%MAIN_PATH%\Build\UnityEngine.dll" ( del "%MAIN_PATH%\Build\UnityEngine.dll" )
if exist "%MAIN_PATH%\Build\UnityEngine.UI.dll" ( del "%MAIN_PATH%\Build\UnityEngine.UI.dll" )

"%~dp0\MSBuild.exe" "%MAIN_PATH%\Project\Editor\Lerp2APIEditor.csproj" "/p:ReferencePath=%REF_PATH%" /p:Platform="Any CPU" /p:OutputPath=../../Build/Editor /p:Configuration=Debug

if exist "%MAIN_PATH%\Build\Editor\Lerp2API.dll" ( del "%MAIN_PATH%\Build\Editor\Lerp2API.dll" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2API.pdb" ( del "%MAIN_PATH%\Build\Editor\Lerp2API.pdb" )
if exist "%MAIN_PATH%\Build\Editor\Lerp2API.xml" ( del "%MAIN_PATH%\Build\Editor\Lerp2API.xml" )
if exist "%MAIN_PATH%\Build\Editor\UnityEditor.dll" ( del "%MAIN_PATH%\Build\Editor\UnityEditor.dll" )
if exist "%MAIN_PATH%\Build\Editor\UnityEngine.dll" ( del "%MAIN_PATH%\Build\Editor\UnityEngine.dll" )
if exist "%MAIN_PATH%\Build\Editor\UnityEngine.UI.dll" ( del "%MAIN_PATH%\Build\Editor\UnityEngine.UI.dll" )

set "ProjectPath=%~1"
<<<<<<< HEAD
if "%ProjectPath%" NEQ "" call :copycontent "%ProjectPath%"
goto:EOF

:copycontent
xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.dll" "%ProjectPath%\Lerp2API.dll"
xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.pdb" "%ProjectPath%\Lerp2API.pdb"
xcopy /e /y "%MAIN_PATH%\Build\Lerp2API.xml" "%ProjectPath%\Lerp2API.xml"
xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" "%ProjectPath%\Editor\Lerp2APIEditor.dll"
xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.pdb" "%ProjectPath%\Editor\Lerp2APIEditor.pdb"
xcopy /e /y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.xml" "%ProjectPath%\Editor\Lerp2APIEditor.xml"
goto:EOF
=======
if "%ProjectPath%" NEQ "" goto :copycontent 

:copycontent
xcopy /E /Y "%MAIN_PATH%\Build\Lerp2API.dll" "%ProjectPath%\Lerp2API.dll"
xcopy /E /Y "%MAIN_PATH%\Build\Lerp2API.pdb" "%ProjectPath%\Lerp2API.pdb"
xcopy /E /Y "%MAIN_PATH%\Build\Lerp2API.xml" "%ProjectPath%\Lerp2API.xml"
xcopy /E /Y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.dll" "%ProjectPath%\Editor\Lerp2APIEditor.dll"
xcopy /E /Y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.pdb" "%ProjectPath%\Editor\Lerp2APIEditor.pdb"
xcopy /E /Y "%MAIN_PATH%\Build\Editor\Lerp2APIEditor.xml" "%ProjectPath%\Editor\Lerp2APIEditor.xml"
pause
>>>>>>> 5930cae50477d76e26446a942dc57ab486c1fbe7

::Other params that you will need if you modify this code and put new dlls, or maybe, if the dlls aren't found in your hdd
::/t:Compile /p:Configuration=Release "/p:ReferencePath=%REF_PATH%"