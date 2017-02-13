@echo off
 
SET "data=1 2 3"
FOR /f "tokens=1,2,3 delims= " %%a IN ("%data%") do echo %%a&echo %%b&echo %%c
 
SET /p exit=Press any key to exit