@echo on
"AP Patcher.exe" "http://www.server.com/repo/" "C:\Download" testpack

goto SkipComment

This is an example batch file to show the automation usage of the application.
Arguments usage:
Arg 1 - REQUIRED - Repo URL, setting up repos is explained in GitHub docs.
Arg 2 - OPTIONAL - Local path to extract the download to. Not specifying will use the InstallDirectory.txt file
Arg 3 - OPTIONAL - The pack id to download. Not entering an id will prompt the user.

:SkipComment