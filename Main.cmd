@echo off
setlocal EnableDelayedExpansion
cls
goto :main


:main
setlocal

SET "_A4_Path=C:\1839-Sutton\Docs by File"
SET "_LF_Path=C:\1839-Sutton\Drawings by File"
set "_Output_Path=C:\1839-Sutton\Output"

:outer
For /D %%A in ("%_A4_Path%\*.*") DO (
SET _A4_Folder=%%~nA
SET _LF_Folder=!_A4_Folder!D
SET "_LF_Path_Folder=%_LF_Path%\!_LF_Folder!"
ECho !_A4_Folder!
Echo !_LF_Folder!
REM Echo !_LF_Path_Folder!

REM To check if the drawing folder exists
pushd !_LF_Path_Folder! 2>NUL && popd

@if not errorlevel 1 (
	echo Box to be processed is: !_A4_Folder!
	call :inner
	) ELSE (
	echo Drawings missing for box !_LF_Folder!
	)

pause
)

goto :eof




:inner
REM To count the files from both folders - A4 and LF
SET /a _countA4=0
SET /a _countLF=0

for %%G in ("%_A4_Path%\!_A4_Folder!\*.*") do set /a _countA4+=1
echo %_countA4%

for %%H in ("%_LF_Path%\!_LF_Folder!\*.*") do set /a _countLF+=1
echo %_countLF%

	if %_countA4% == %_countLF% (
	echo ehehe good one
	echo.
	
	
	call :innerinner
	
	) ELSE (
		echo A4 Folder and LF Folder have different numbers of files
	)


goto :eof



:innerinner
setlocal


for /F %%B in ('dir /B "!_A4_Path!\!_A4_Folder!"\*.pdf') do (

set _A4_File=%%B
set _LF_File=!_A4_File:~0,-4!D!_A4_File:~-4,4!
set _Output_File=!_A4_File:~0,-4!F!_A4_File:~-4,4!


    if exist "!_LF_Path!\!_LF_Folder!\!_LF_File!" (
	echo.
	echo.
	echo yes, A4 and LF File match!

	echo Proceeding with merge^^! ^^!
	call sejda-console.bat merge -f "!_A4_Path!"\!_A4_Folder!\!_A4_File! "!_LF_Path!"\!_LF_Folder!\!_LF_File! -o !_Output_Path!\!_Output_File!
	
	call :Update_Data "!_A4_Path!\!_A4_Folder!\%%B"
	set _A4_No_of_Pages=!numberOfPages!
	echo A4 Number of Pages: !_A4_No_of_Pages!
	
	
	call :Update_Data "!_LF_Path!\!_LF_Folder!\!_LF_File!"
	set _LF_No_of_Pages=!numberOfPages!
	echo LF Number of Pages: !_LF_No_of_Pages!
	
	SET SQL="UPDATE Pablo_Scandata SET PageCount=!_A4_No_of_Pages!, DrawingCount=!_LF_No_of_Pages! WHERE Barcode ='!_A4_File:~0,-4!'"
	sqlcmd -d TimerSQL -Q !SQL!
	
	set "numberOfPages="
	
	) ELSE ( 
	
	echo A4 File doesnt have a match in the Drawings Folder
	
	goto :eof
	)

)


endlocal
goto :eof



:Update_Data
REM setlocal



REM pdftk %1 dumpdata | findstr NumberOfPages 

for /f "tokens=2" %%C in ('pdftk.exe %1 dump_data ^| findstr NumberOfPages') do set numberOfPages=%%C

REM echo !numberOfPages!



REM endlocal & set numberOfPages=%%C
goto :eof
