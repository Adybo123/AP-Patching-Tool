Imports Ionic.Zip
Module APPatcherMain

    'Declarations
    Dim RepoURL As String = "RepoURI"
    Dim CurrentDirectory As String = System.IO.Directory.GetCurrentDirectory
    Dim InstallDirectory As String = ""
    Dim PrefLoadFail As Boolean = False
    Dim ArgsPackID As String = ""
    Dim QuitOnInstallPack As Boolean = False
    Dim RunAsSilent As Boolean = False
    Dim ShowDownloadUI As Boolean = True

    Sub Main()
        LoadPreferencesFromArguments()
        If PrefLoadFail = False Then
            BackToMainMenu()
        End If
    End Sub

    Sub WriteToConsole(ConsoleWriteLn As String)
        If RunAsSilent = False Then
            Console.WriteLine(ConsoleWriteLn)
        End If
    End Sub

    Sub DrawLogo()
        WriteToConsole("=======================")
        WriteToConsole("AP Patcher package tool")
        WriteToConsole("Created by Adam Soutar")
        WriteToConsole("=======================")
        WriteToConsole("")
    End Sub

    Sub LoadPreferencesFromArguments()
        Console.Clear()
        Dim LoadArguments As String() = Environment.GetCommandLineArgs()
        Try
            'Check for silent command BEFORE we draw graphics
            Dim SilentCommand As String = LoadArguments(4)
            If SilentCommand = "silent" Then
                RunAsSilent = True
                ShowDownloadUI = False
            End If
        Catch ex As Exception
            'No "silent" command
        End Try
        DrawLogo()

        'TODO: Check if paths are valid
        WriteToConsole("Retrieving command line arguments... OK!")
        CurrentDirectory = System.IO.Path.GetDirectoryName(LoadArguments(0))
        WriteToConsole("Loading current directory... OK!")
        Try
            'Load repo
            RepoURL = LoadArguments(1)
            WriteToConsole("Loading repo URI... OK!")
            If RepoURL = "default" Then
                WriteToConsole("Repo specified was 'default', load fallbacks.")
                LoadRepoPreference()
            End If
        Catch ex As Exception
            LoadRepoPreference()
        End Try
        Dim LoadFallbacks As Boolean = False
        Try
            InstallDirectory = LoadArguments(2)
            WriteToConsole("Loading install directory... OK!")
            If InstallDirectory = "default" Then
                'Use default folder
                LoadFallbacks = True
                WriteToConsole("Provided argument 'default', loading fallback.")
            End If
        Catch ex As Exception
            'No directory specified
            LoadFallbacks = True
            WriteToConsole("No argument provided, loading fallback.")
        End Try
        If LoadFallbacks = True Then
            LoadInstallDirectoryPreference()
        End If
        Try
            'If this succeeds, the user's automating the pack
            ArgsPackID = LoadArguments(3)
        Catch ex As Exception
            'The user wants to enter the pack manually
        End Try
    End Sub

    Sub LoadRepoPreference()
        Console.Clear()
        DrawLogo()
        Try
            Dim SysRdr As New System.IO.StreamReader(CurrentDirectory & "\MainRepo.txt")
            RepoURL = SysRdr.ReadLine
            SysRdr.Close()
        Catch ex As Exception
            WriteToConsole("Loading preferences failed!")
            WriteToConsole("Couldn't find this file: " & CurrentDirectory & "\MainRepo.txt")
            WriteToConsole("")
            WriteToConsole("Starting the application requires a Repo URL from command line arguments or a file.")
            'TODO: Ask if they want to learn how to use command line args
            WriteToConsole("")
            WriteToConsole("Press enter to quit.")
            PrefLoadFail = True
            Dim QuitResponse As String = Console.ReadLine
        End Try
    End Sub

    Sub LoadInstallDirectoryPreference()
        Console.Clear()
        DrawLogo()
        Try
            Dim SysRdr As New System.IO.StreamReader(CurrentDirectory & "\InstallDirectory.txt")
            InstallDirectory = SysRdr.ReadLine
            SysRdr.Close()
        Catch ex As Exception
            WriteToConsole("Loading preferences failed!")
            WriteToConsole("Couldn't find this file: " & CurrentDirectory & "\InstallDirectory.txt")
            WriteToConsole("")
            WriteToConsole("Starting the application requires an install directory from command line arguments or a file.")
            'TODO: Ask if they want to learn how to use command line args
            WriteToConsole("")
            WriteToConsole("Press enter to quit.")
            PrefLoadFail = True
            Dim QuitResponse As String = Console.ReadLine
        End Try
    End Sub

    Sub BackToMainMenu()
        Console.Clear()
        DrawLogo()
        WriteToConsole("Automatic patching tool for automated packages.")
        WriteToConsole("")
        WriteToConsole("Loaded with these commands:")
        WriteToConsole("Temp Directory - " & CurrentDirectory)
        WriteToConsole("Install Directory - " & InstallDirectory)
        WriteToConsole("Repo - " & RepoURL)
        WriteToConsole("")
        Dim PackIdToFetch As String
        If ArgsPackID = "" Then
            WriteToConsole("Please enter the desired Pack ID:")
            Console.Write(">")
            PackIdToFetch = Console.ReadLine
        Else
            WriteToConsole("Automatically downloading pack - " & ArgsPackID)
            PackIdToFetch = ArgsPackID
            QuitOnInstallPack = True
        End If
        PatchFromID(PackIdToFetch)
    End Sub

    Sub PatchFromID(PackID)
        WriteToConsole("")
        WriteToConsole("Fetching pack " & PackID & "...")

        'Attempt pack download
        Dim LocalPackPath As String = System.IO.Directory.GetCurrentDirectory & "\" & PackID & ".zip"
        Try
            My.Computer.Clipboard.SetText(RepoURL & PackID & ".zip")
            My.Computer.Network.DownloadFile(RepoURL & PackID & ".zip", LocalPackPath, "", "", ShowDownloadUI, 10000, True)
            WriteToConsole("Asking repo for response... OK!")
            WriteToConsole("Checking pack exists... OK!")
            WriteToConsole("Patch downloaded successfully.")
        Catch ex As Exception
            WriteToConsole("Asking repo for response... FAIL!")
            WriteToConsole("Pack ID might be incorrect, repo URI may be invalid or not responding correctly.")
        End Try

        'Attempt zip file extraction
        WriteToConsole("Attempting to extract compressed pack...")
        Try
            Dim ZipToUnpack As String = LocalPackPath
            Dim UnpackDirectory As String = InstallDirectory & "\"
            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                Dim e As ZipEntry
                Dim TotalItems As Integer = 0
                For Each x In zip1
                    TotalItems = TotalItems + 1
                Next
                WriteToConsole("File scan complete. Compressed file contains " & TotalItems & " textures.")
                Dim CurrentItem As Integer = 1
                For Each e In zip1
                    WriteToConsole("Extracting item " & CurrentItem & " of " & TotalItems)
                    e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                    CurrentItem = CurrentItem + 1
                Next
            End Using
            WriteToConsole("")
            WriteToConsole("Pack extracted.")
        Catch ex As Exception
            WriteToConsole("")
            WriteToConsole("Failed to extract the patch! The Pack ID may have been invalid or the repo might not be responding properly.")
        End Try
        WriteToConsole("")
        If QuitOnInstallPack = True Then
            Exit Sub
        End If
        WriteToConsole("Type 'quit' to close the application, type 'menu' to return to the menu.")
        Console.Write(">")
        Dim OptionResponse As String = Console.ReadLine
        If OptionResponse.ToLower() = "quit" Then
            Exit Sub
        End If
        BackToMainMenu()
    End Sub

    Function TakeConsoleInput()
        Console.Write(">")
        Dim ResponseText As String = Console.ReadLine
        Return ResponseText
    End Function

End Module
