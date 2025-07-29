@echo off
for /d /r %%i in (bin,obj) do (
    if exist "%%i" rd /s /q "%%i"
)
echo All bin and obj folders cleaned.
pause