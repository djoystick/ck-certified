; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define CKVersion ReadIni(SourcePath + "\\BuildInfo.ini","Info","Version","0");                                  
#define ApplicationName ReadIni(SourcePath + "\\BuildInfo.ini","Info","ApplicationName","0");
#define DistribName ReadIni(SourcePath + "\\BuildInfo.ini","Info","DistribName","0");
#define IsStandAloneInstance ReadIni(SourcePath + "\\BuildInfo.ini","Info","IsStandAloneInstance","false");
#define UpdateServerUrl ReadIni(SourcePath + "\\BuildInfo.ini","Update","UpdateServerUrl","0");
#define UpdaterRunningKey ReadIni(SourcePath + "\\BuildInfo.ini","Update","UpdaterRunningKey","0");
#define CiviKeyMutex ReadIni(SourcePath + "\\BuildInfo.ini","Update","CiviKeyMutex","0");
#define OverridePreviousContext ReadIni(SourcePath + "\\BuildInfo.ini","Update","OverridePreviousContext","false");

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{79CA0471-5C7D-4E7E-949D-5B29BBB4E960}}
AppName={#ApplicationName}-{#DistribName}
AppVersion={#CKVersion}
AppPublisher=Invenietis
AppPublisherURL=http://www.invenietis.com/
DefaultDirName={pf}\{#ApplicationName}\{#DistribName}
DefaultGroupName={#ApplicationName}
AllowNoIcons=yes
OutputDir=.\
OutputBaseFilename={#ApplicationName}-{#DistribName}-{#CKVersion}
Compression=lzma
SolidCompression=yes
SetupIconFile=CiviKey.ico
UninstallDisplayIcon={app}\resources\CiviKey.ico
AppMutex=CiviKeyMutex
VersionInfoVersion={#CKVersion}
VersionInfoProductName={#ApplicationName}-{#DistribName}

[Files]
Source: "..\..\Output\Release\*"; DestDir: "{app}\binaries"; Excludes: "*.pdb, *.xml, *.ck, *.vshost.exe.*, *.manifest, *.iss, \Setup, \Tests"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "CiviKey.ico"; DestDir: "{app}\resources"; Permissions: users-modify;
Source: "CiviKeyPostInstallScript.exe"; DestDir: "{tmp}"; Flags: ignoreversion ; Permissions: users-modify;
Source: "System.config.ck"; DestDir: {code:GetOutputDir|{#IsStandAloneInstance}}; Flags: onlyifdoesntexist; Permissions: users-modify; 
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[InstallDelete]
Type: filesandordirs; Name: "{app}\binaries\*"

[Icons]
Name: "{group}\CiviKey"; Filename: "{app}\binaries\CiviKey.exe"; WorkingDir: "{app}"; IconFileName:"{app}\resources\CiviKey.ico";
Name: "{commondesktop}\CiviKey"; Filename: "{app}\binaries\CiviKey.exe"; WorkingDir: "{app}"; IconFileName:"{app}\resources\CiviKey.ico"

[Run]
Filename: "{tmp}\CiviKeyPostInstallScript.exe"; Parameters: """{#CKVersion}"" ""{#ApplicationName}"" ""{#DistribName}"" ""{#UpdateServerUrl}"" ""{#UpdaterRunningKey}"" ""{#OverridePreviousContext}"" ""{#IsStandAloneInstance}"" ""{code:GetOutputDir|{#IsStandAloneInstance}}"" ""{app}\binaries\CiviKey.exe"""
                                                                                                                                                                                                                                               ;System configuration directory                          ;.exe path
[Code]
function GetOutputDir(Param: String): String;
begin
    if Param = 'false' then begin
        Result := ExpandConstant('{commonappdata}\{#ApplicationName}\{#DistribName}\\');
    end else begin
        Result := ExpandConstant('{app}\Configurations');
    end
end;

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, serviceCount: cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;
    // .NET 4.0 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;
    result := success and (install = 1) and (serviceCount >= service);
end;

function IsRegularUser(): Boolean;
begin
Result := not (IsAdminLoggedOn or IsPowerUserLoggedOn);
end;

function InitializeSetup(): Boolean;
var 
	ResultCode: Integer;
begin
    if not IsDotNetDetected('v4\Client', 0) 
    then begin
     
        if(MsgBox('CiviKey nécessite le Framework .NET 4.0 Client Profile.'#13#13'Cliquez sur "Non" pour annuler l''installation ou "Oui" pour télécharger le Framework .NET 4.0 Client Profile automatiquement sur le site de Microsoft.', mbConfirmation, MB_YESNO) = IDYES) 
        then begin 
          ExtractTemporaryFile('dotNetFx40_Client_setup.exe');
          ShellExec('', ExpandConstant('{tmp}\dotNetFx40_Client_setup.exe'), '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
          result := true;
        end else begin
          MsgBox('Installation annulée.', mbInformation, MB_OK);
          Abort;
        end;
	end;
	result := true;
end;
