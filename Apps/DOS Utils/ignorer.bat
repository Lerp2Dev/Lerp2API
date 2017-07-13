@echo off

:: %~1 ==> TXT file (ignorecom.txt)
:: %~2 ==> Dest Folder

if [%1]==[] goto :Usage
if [%2]==[] goto :Usage

::Main

set "txtfile=%~1"
set "destfolder=%~2"

if exist %1 (
	if exist %2 (
		for /f "usebackq tokens=*" %%x in ("%txtfile%") do (
			call :FDelete %%x
		)
	) else (
		echo aaaa
		echo.
	)
) else (
	echo bbb
	echo.
)
goto:Terminate

:Usage
echo The usage of this file is the following:
echo.
echo You must specify a file to ignore as a 1st parameter.
echo You must specify a folder where to find everything as a 2nd parameter.
echo.
goto:Terminate

:FDelete
set "line=%~1"
setlocal EnableDelayedExpansion
set "string=%line:/=\%"
echo Deleting '%destfolder%%string%'...
RMDIR "%destfolder%%string%" /S /Q
endlocal
goto:EOF

:Terminate
exit