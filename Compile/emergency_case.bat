@Echo OFF

::Solution: http://stackoverflow.com/questions/42211530/batch-error-was-not-expected-at-this-time/42213519#42213519
::Call this in case you've broke you version of the API and you cannot start Unity...
 
:str
CLS
GoTo :Main
 
:Main
Echo:Hello. Well, if you're opening this it is because you have done something wrong and you cannot start Unity...
Echo+
Choice.exe /C "YN" /M "Do you want to read your path from the \"project_path.txt\" file or do you want to specify "
Set /A "opt=%ErrorLevel%" & REM 1=Y, 2=N
Echo+
Echo:Also, these options below are optional but you can try to establish an order for the compilation.
Echo+
Echo:1.- Build the API
Echo:2.- Build the RAW Scripts
Echo:3.- Build the Editor API
Echo+
Set /P "order=Type, for example: [2 1 3], to compile in this order, or the way you want: "
 
If %opt% EQU 1 (
    FOR /F "Delims=" %%f in (
    	"project_path.txt"
    ) Do (
        If "%%~f" NEQ "" (
            Call :RunCompiler "%%~f" "%order%"
        )
    )
 
) Else (
    Set /P "cpath=Path: "
    Call :RunCompiler "%cpath%" "%order%"
 
)
GoTo :Terminate
 
:RunCompiler
::CMD.exe /C " Start /W "" "compile.bat" "%~1" "%~2" " 
::Beautiful error, above
CMD.exe /C " compile.bat "%~1" "%~2" "
GoTo :EOF
 
:Terminate
Pause&Exit