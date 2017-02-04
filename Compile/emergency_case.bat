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
<<<<<<< HEAD
	for /f "delims=" %%f in (project_path.txt) do if "%%f" NEQ "" call :callcompile "%%f"
) else (
echo bbb
	if /i %opt% == N (
		echo.
		set /p cpath="Path: "
		goto :callcompile %cpath%
=======
	set /p path=<path.txt
	goto :callcompile
) else (
	if /i %opt% == N (
		echo.
		set /p path="Path: "
		goto :callcompile
>>>>>>> 5930cae50477d76e26446a942dc57ab486c1fbe7
	) else (
		goto :str
	)
)
<<<<<<< HEAD
goto :EOF

:callcompile
cmd /c compile.bat "%~1"
=======

:callcompile
compile.bat "%path%"
>>>>>>> 5930cae50477d76e26446a942dc57ab486c1fbe7
pause