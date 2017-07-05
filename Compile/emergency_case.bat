@Echo OFF

::Solution: http://stackoverflow.com/questions/42211530/batch-error-was-not-expected-at-this-time/42213519#42213519
::Call this in case you've broke you version of the API and you cannot start Unity...

::I have to fix blank lines on the project_path.txt
::I have to check order syntax is right...

::Opt
set "first=%~1"

::Order
set "second=%~2"

::Number of line
set "third=%~3"

:str
CLS
GoTo :Main
 
:Main
IF "%third%" EQU "" (
    IF "%first%" EQU "" (
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
        Echo:4.- Build the Console
        Echo+
        Set /P "order=Type, for example: [2 1 4 3], to compile in this order, or the way you want: "

        echo.
        echo Now, you have to select one of the following paths to copy the generated contents:
        echo.

        setlocal enableextensions enabledelayedexpansion
        set /a count=0
        for /f "tokens=*" %%a in (project_path.txt) do (
            IF %%a NEQ "" (
                set /a count += 1
                echo !count!. %%a
            )
        )
        call :ChooseLine !count! "%opt%" "%order%"
        endlocal
        goto:EOF
    )
) else (
    echo Going directly to compile everything...
    set /a "rline=%third%-1"
    call :SelectLine numeric %third% 0 %first% "%second%" 
)
goto:EOF

:ChooseLine

::%~1 = Numero de lineas
::%~2 = Opt
::%~3 = Order

echo.
set /p "line=Which path do you choose? [1-%~1]: "
set /a "rline=%line%-1"

echo(%line%|findstr "^[-][1-9][0-9]*$ ^[1-9][0-9]*$ ^0$">nul&&call :SelectLine numeric %rline% %~1 %~2 "%~3"||call :SelectLine not_numeric
goto:EOF

:SelectLine

::%~1 = IsNumeric
::%~2 = Numero de linea real seleccionado
::%~3 = Numero de lineas totales
::%~4 = Opt
::%~5 = Order

IF "%~1" EQU "numeric" (
    for /F "skip=%~2 delims=" %%i in (project_path.txt) do (
        call :Compiler "%%i" %~4 "%~5"
        goto:EOF
    )
) else (
    echo.
    echo Invalid number given.
    goto :ChooseLine %~3 %~4 "%~5"
)
goto:EOF

:Compiler

::%~1 = Path
::%~2 = Opt
::%~3 = Order

If %~2 EQU 1 (
    If "%~1" NEQ "" (
        Call :RunCompiler "%~1" "%~3"
    ) Else (
        Call :EmptyError
    )
    GoTo :Terminate
) Else (
    Set /P "cpath=Write path manually: "
    Call :RunCompiler "%cpath%" "%~3"
)
GoTo :Terminate
 
:RunCompiler
CMD.exe /C " compile.bat "%~1" "%~2" "
GoTo :EOF

:EmptyError
Echo:The file 'project_path.txt' is empty, please check it. If it's empty, please put the path where you want to copy the file.
GoTo :Terminate 

:Terminate
Pause&Exit