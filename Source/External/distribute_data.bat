REM only edit the data files in THIS FOLDER, because this script copies them from here to the other locations!

echo Y | del "Gcm\bin\Debug\Data\*"
echo Y | del "Gcm\bin\Release\Data\*"
echo Y | del "..\SCEWeb\SCE\App_Data\gcm\Data\*"

xcopy /e /v /y "Data" "..\Gcm\Gcm\bin\Debug\Data"
xcopy /e /v /y "Data" "..\Gcm\Gcm\bin\Release\Data"
xcopy /e /v /y "Data" "..\SCEWeb\SCE\App_Data\gcm\Data"