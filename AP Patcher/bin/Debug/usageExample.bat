@echo on
"AP Patcher.exe" "http://www.server.com/repo/" "C:\Download" testpack silent

goto SkipComment

This is an example batch file to show the automation usage of the application.
Arguments usage:
Arg 1 - OPTIONAL - Repo URL, setting up repos is explained in GitHub docs.
Arg 2 - OPTIONAL (Must be specified if the above is set) - Local path to extract the download to. Not specifying will use the InstallDirectory.txt file
Arg 3 - OPTIONAL - The pack id to download. Not entering an id will prompt the user.
Arg 4 - OPTIONAL - Set to 'silent' to run without printing anything to the console.

:SkipComment