; Скрипт создан при помощи мастера создания скриптов.
; СМ. ДОКУМЕНТАЦИЮ ДЛЯ ИЗУЧЕНИЯ ДЕТАЛЕЙ ОТНОСИТЕЛЬНО СОЗДАНИЯ ФАЙЛОВ СКРИПТА INNO SETUP!

#define MyAppName "ComicsManager"
#define MyAppVerName "ComicsManager 1.0"
#define MyAppPublisher "Office ware"
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
Source: "C:\wpf_projects\ComicsManager\ComicsManager\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion
; ОТМЕТЬТЕ: Не используйте "Флажки: Проигнорировать версию" на любых общедоступных системных файлах

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

