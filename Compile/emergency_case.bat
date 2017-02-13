@echo off
::Call this in case your broke you dll and you cannot start Unity for example

:str
cls
goto :main

:main
echo Hello, well, if you're opening this is because you have done something wrong and you cannot start Unity...
echo.
set /p opt="Do you want to read your path from the 'path.txt' file or do you want to specify? [Y/N] "
echo.
echo Also, this is optional but you can try to establish an order for the compilation.
echo.
echo 1) Build the API
echo 2) Build the RAW Scripts
echo 3) Build the Editor API
echo.
set /p order="Type, for example: [2 1 3], to compile in this order, or the way you want: "

if /i "%opt%" == "Y" (
	for /f "delims=" %%f in ("project_path.txt") do (
		if "%%f" NEQ "" call :callcompile "%%f" "%order%"
	)
) else (
	if /i "%opt%" == "N" (
		echo.
		set /p cpath="Path: "
		goto :callcompile "%cpath%" "%order%"
	) else (
		goto :str
	)
)
goto :EOF

:callcompile
cmd /c compile.bat "%~1" "%~2"
pause