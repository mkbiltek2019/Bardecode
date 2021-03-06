@echo off

REM Before we need to check that batch A, B, C in docs as scanned don't have a match in 'Docs by File'
REM only the ones without a match will proceed to Barcode Recognition
REM Database checking will go into main. before each merge. It will try to merge each file without a pagecount.

setlocal EnableDelayedExpansion
cls
goto :main


:main
setlocal

set "_A4_DoasS_Path=C:\2003-Southampton\Docs as Scanned"
set "_A4_DobF_Path=C:\2003-Southampton\Docs by File"



:A4_Checking

for /F %%B in ('dir /B "!_A4_DoasS_Path!"\*') do (

REM Folder in Docs as Scanned
set _A4_DoasS_Folder=%%B


echo. 


	if "!_A4_DoasS_Folder:~0,4!" NEQ "Done" (

		REM To check if there is a matching folder of the "Docs as Scanned" folder in "Docs by File" folder
		pushd "!_A4_DobF_Path!\!_A4_DoasS_Folder!" 2>NUL && popd
		@if not errorlevel 1 (
		echo !_A4_DoasS_Folder! Folder found in Docs by File, skip to the next box
		echo.
		echo.

		) ELSE (
			
			echo !_A4_DoasS_Folder! Folder not found in Docs by File, proceed with Barcode Separation
			echo.
			echo.
		
			call :Barcode_Separation_FS "A4_Southampton.ini"
			
			REM The moving of files to the subfolder is done by Bardecode, so no need for these lines
				REM for /F "tokens=1,2,3 delims=-" %%G in ('dir /B "!_A4_DobF_Path!\"*.pdf') do (
				REM mkdir "!_A4_DobF_Path!\!_A4_DoasS_Folder!"
				REM move "!_A4_DobF_Path!\"%%G-%%H-%%I "!_A4_DobF_Path!\!_A4_DoasS_Folder!\"%%I
				REM )
			
			
			call :Barcode_Separation_Sections "A4_Southampton_Sections.ini"
			
			rename "!_A4_DoasS_Path!\!_A4_DoasS_Folder!" "Done-!_A4_DoasS_Folder!"
			 
		)
	)
)
goto :eof



:Barcode_Separation_FS
setlocal

::Set working directory
PUSHD %~dp0

REM I need to modify the INI file with each step of the loop, so BardecodeFiler will only process one box at a time, and not the whole input folder
%~dp0\inifile\INIFILE "A4_Southampton.ini" [options] inputFolder=System.String,C:\2003-Southampton\Docs as Scanned\!_A4_DoasS_Folder!
   

"C:\Program Files (x86)\Softek Software\BardecodeFiler\"BardecodeFiler.exe %1
REM If error?????


REM _temp_Folder is the name of the subfolder created by BardecodeFiler, we rename it accordingly. Check the correct sintaxis for rename
rename "!_A4_DobF_Path!\_temp_Folder" "!_A4_DoasS_Folder!"


REM If you want to produce an empty assignment without removing it, use two equal signs.
REM Syntax:  INIFILE inifileName [section] item==
%~dp0\inifile\INIFILE "A4_Southampton.ini" [options] inputFolder==



:: Return to your original working directory.
POPD

endlocal
goto :eof





:Barcode_Separation_Sections
setlocal

::Set working directory
PUSHD %~dp0



REM I need to modify the INI file with each step of the loop, so BardecodeFiler will only process one box at a time, and not the whole input folder
%~dp0\inifile\INIFILE "A4_Southampton_Sections.ini" [options] inputFolder=System.String,!_A4_DobF_Path!\!_A4_DoasS_Folder!
%~dp0\inifile\INIFILE "A4_Southampton_Sections.ini" [options] outputFolder=System.String,!_A4_DobF_Path!\!_A4_DoasS_Folder!   



"C:\Program Files (x86)\Softek Software\BardecodeFiler\"BardecodeFiler.exe %1




REM If you want to produce an empty assignment without removing it, use two equal signs.
REM Syntax:  INIFILE inifileName [section] item==
%~dp0\inifile\INIFILE "A4_Southampton_Sections.ini" [options] inputFolder==
%~dp0\inifile\INIFILE "A4_Southampton_Sections.ini" [options] outputFolder==



REM Rename the original file
FOR /R "%_A4_DobF_Path%" %%G IN (*_NOBARCODE.pdf) DO (
	echo %%~nG
    SET "_X=%%~nG"
    SET _X=!_X:_NOBARCODE=!

	rename "%%G" !_X!.pdf
)


REM Rename the File End Sheet
FOR /R "%_A4_DobF_Path%" %%G IN (*_EDFE.pdf) DO (
	echo %%~nG
    SET "_X=%%~nG"
    SET _X=!_X:_EDFE=_File_End_Sheet!

	rename "%%G" !_X!.pdf
)

REM Rename the control sheets
FOR /R "%_A4_DobF_Path%" %%G IN (*D.pdf) DO (
	echo %%~nG
	SET "_X=%%~nG"
	set _X=!_X:~0,9!
	rename "%%G" !_X!D.pdf
)



:: Return to your original working directory.
POPD

endlocal
goto :eof