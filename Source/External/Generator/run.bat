@echo off
set OLDPATH=%PATH%
set PATH="C:\Python27";%PATH%
python sift.py

copy Orders.cs "..\../ORG/Game/Gameplay/Orders/Orders.cs"
copy Structures.cs "..\../SCEShared/Structures.cs"
copy GcmSharedStructures.cs "..\../Gcm/GcmShared/GcmSharedStructures.cs"
copy GcmJsonParameters.cs "..\..\SCEWeb\SCE\Controllers\Helpers\GcmJsonParameters.cs"

pause>nul





