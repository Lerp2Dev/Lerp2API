@echo off
::Call this in case your broke you dll and you cannot start Unity for example

:str
cls
goto :main

:main
echo Hello, well, if you're opening this is because you have done something wrong and you cannot start Unity...
echo Do you want to read your path from the 'path.txt' file or do you want to specify? [Y/N]
echo.
set /p opt="Select option: "

if /i %opt% == Y (
	set /p path=<path.txt
	goto :callcompile
) else (
	if /i %opt% == N (
		echo.
		set /p path="Path: "
		goto :callcompile
	) else (
		goto :str
	)
)

:callcompile
compile.bat "%path%"
pause