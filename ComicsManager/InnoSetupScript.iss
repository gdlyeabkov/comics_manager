; ������ ������ ��� ������ ������� �������� ��������.
; ��. ������������ ��� �������� ������� ������������ �������� ������ ������� INNO SETUP!

#define MyAppName "ComicsManager"
#define MyAppVerName "ComicsManager 1.0"
#define MyAppPublisher "Office ware game manager"
#define MyAppExeName "ComicsManager.exe"

[Setup]
AppName={#MyAppName}
AppVerName={#MyAppVerName}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=C:\wpf_projects\ComicsManager\ComicsManager
OutputBaseFilename=ComicsManager
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Languages\English.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\wpf_projects\ComicsManager\ComicsManager\bin\Release\ComicsManager.exe"; DestDir: "{app}"; Flags: ignoreversion
; ��������: �� ����������� "������: ��������������� ������" �� ����� ������������� ��������� ������

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

