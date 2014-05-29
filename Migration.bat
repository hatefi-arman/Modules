@echo off

set arg1=%1
set arg2=%2

set migrateComm=packages\FluentMigrator.1.1.2.1\tools\
set migrateComm1=packages\FluentMigrator.1.1.2.1\tools\migrate -a 
set migrateComm2=Fuel\MITD.Fuel.Data.EF\bin\Debug\MITD.Fuel.Data.EF.dll 
set migrateComm3=-configPath packages\FluentMigrator.1.1.2.1\tools\Migrate.exe.config -db SqlServer2008 -connectionString 

attrib -R %migrateComm%\Migrate.exe.config
copy /Y Migrate.exe.config %migrateComm%\Migrate.exe.config


set ReleaseConn="DataContainer"

set DebugConn="DataContainer"


IF "%arg1%" == "release" ( 
	IF "%arg2%"=="u" (
	   GOTO ReleaseMigrateUp
	) ELSE  GOTO ReleaseMigrateDown 

) ELSE IF "%arg1%" == "debug" (
	  
	  IF "%arg2%" == "u" (
	    GOTO DebugMigrateUp 
      ) ELSE GOTO DebugMigrateDown  
    
) ElSE IF "%arg1%" == "both" (
	 
	  IF "%arg2%" == "u" ( 
	       GOTO BothMigrateUp 
      ) ELSE  GOTO BothMigrateDown 
) ELSE (
	echo "Invalid first argument"
	GOTO End
)


:ReleaseMigrateUp
echo "ReleaseMigrateUp"
%migrateComm1% %migrateComm2%  %migrateComm3% %ReleaseConn%  -profile=%arg1%  %3 %4
GOTO End

:ReleaseMigrateDown
echo "ReleaseMigrateDown"
%migrateComm1% %migrateComm2%  %migrateComm3%  %ReleaseConn% -t migrate:down 
GOTO End

:DebugMigrateUp
echo "DebugMigrateUp"
%migrateComm1% %migrateComm2%  %migrateComm3% %DebugConn% -profile=%arg1%  %3 %4  
GOTO End

:DebugMigrateDown
echo "DebugMigrateDown"
%migrateComm1% %migrateComm2%  %migrateComm3% %DebugConn%  -t migrate:down 
GOTO End

:BothMigrateUp
echo "BothMigrateUp"
%migrateComm1% %migrateComm2%  %migrateComm3%  %ReleaseConn%  -profile=%arg1%   %3 %4
%migrateComm1% %migrateComm2%  %migrateComm3%  %DebugConn%    -profile=%arg1%   %3 %4
GOTO End

:BothMigrateDown
echo "BothMigrateDown"
%migrateComm1% %migrateComm2%  %migrateComm3%  %ReleaseConn%  -t migrate:down 
%migrateComm1% %migrateComm2%  %migrateComm3%  %DebugConn%    -t migrate:down 
GOTO End


:End