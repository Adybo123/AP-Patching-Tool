Imports Ionic.Zip
Module APPatcherMain

    'Declarations
    Dim RepoURL As String = "RepoURI"
    Dim CurrentDirectory As String = System.IO.Directory.GetCurrentDirectory
    Dim InstallDirectory As String = ""
    Dim PrefLoadFail As Boolean = False
    Dim ArgsPackID As String = ""

    Sub Main()
        LoadPreferencesFromArguments()
        If PrefLoadFail = False Then
            BackToMainMenu()
        End If
    End Sub

    Sub DrawLogo()
        Console.WriteLine("=======================")
        Console.WriteLine("AP Patcher package tool")
        Console.WriteLine("Created by Adam Soutar")
        Console.WriteLine("=======================")
        Console.WriteLine("")
    End Sub

    Sub LoadPreferencesFromArguments()
        Console.Clear()
        DrawLogo()

        'TODO: Check if paths are valid
        Dim LoadArguments As String() = Environment.GetCommandLineArgs()
        Console.WriteLine("Retrieving command line arguments... OK!")
        CurrentDirectory = System.IO.Path.GetDirectoryName(LoadArguments(0))
        Console.WriteLine("Loading current directory... OK!")
        Dim LoadFallbacks As Boolean = False
        Try
            Console.WriteLine("Loading install directory...")
            InstallDirectory = LoadArguments(1)
            Console.WriteLine("Argument provided")
            If InstallDirectory = "default" Then
                'Use default folder
                LoadFallbacks = True
                Console.WriteLine("Provided argument 'default', loading fallback.")
            End If
        Catch ex As Exception
            'No directory specified
            LoadFallbacks = True
            Console.WriteLine("No argument provided, loading fallback.")
        End Try
        If LoadFallbacks = True Then
            LoadInstallDirectoryPreference()
        End If
        Try
            'If this succeeds, the user's automating the pack
            ArgsPackID = LoadArguments(2)
        Catch ex As Exception
            'The user wants to enter the pack manually
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
            Console.WriteLine("Loading preferences failed!")
            Console.WriteLine("Couldn't find this file: " & CurrentDirectory & "\InstallDirectory.txt")
            Console.WriteLine("")
            Console.WriteLine("Starting the application requires an install directory from command line arguments or a file.")
            'TODO: Ask if they want to learn how to use command line args
            Console.WriteLine("")
            Console.Write("Press enter to quit.")
            PrefLoadFail = True
            Dim QuitResponse As String = Console.ReadLine
        End Try
    End Sub

    Sub BackToMainMenu()
        Console.Clear()
        DrawLogo()
        Console.WriteLine("Automatic patching tool for automated packages.")
        Console.WriteLine("")
        Console.WriteLine("Loaded with these commands:")
        Console.WriteLine("Temp Directory - " & CurrentDirectory)
        Console.WriteLine("Install Directory - " & InstallDirectory)
        Console.WriteLine("")
        Console.WriteLine("Please enter the desired Pack ID:")
        Console.Write(">")
        Dim PackIdToFetch As String = Console.ReadLine
        PatchFromID(PackIdToFetch)
    End Sub

    Sub PatchFromID(PackID)
        Console.WriteLine("")
        Console.WriteLine("Fetching pack " & PackID & "...")

        'Attempt pack download
        Dim LocalPackPath As String = System.IO.Directory.GetCurrentDirectory & "\" & PackID & ".zip"
        Try
            My.Computer.Clipboard.SetText(RepoURL & PackID & ".zip")
            My.Computer.Network.DownloadFile(RepoURL & PackID & ".zip", LocalPackPath, "", "", True, 10000, True)
            Console.WriteLine("Asking repo for response... OK!")
            Console.WriteLine("Checking pack exists... OK!")
            Console.WriteLine("Patch downloaded successfully.")
        Catch ex As Exception
            Console.WriteLine("Asking repo for response... FAIL!")
            Console.WriteLine("Pack ID might be incorrect, repo URI may be invalid or not responding correctly.")
        End Try

        'Attempt zip file extraction
        Console.WriteLine("Attempting to extract compressed pack...")
        Try
            Dim ZipToUnpack As String = LocalPackPath
            Dim UnpackDirectory As String = InstallDirectory & "\"
            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                Dim e As ZipEntry
                Dim TotalItems As Integer = 0
                For Each x In zip1
                    TotalItems = TotalItems + 1
                Next
                Console.WriteLine("File scan complete. Compressed file contains " & TotalItems & " textures.")
                Dim CurrentItem As Integer = 1
                For Each e In zip1
                    Console.WriteLine("Extracting item " & CurrentItem & " of " & TotalItems)
                    e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                    CurrentItem = CurrentItem + 1
                Next
            End Using
            Console.WriteLine("")
            Console.WriteLine("Pack extracted, game patched.")
        Catch ex As Exception
            Console.WriteLine("")
            Console.WriteLine("Failed to extract the patch! The Pack ID may have been invalid or the repo might not be responding properly.")
        End Try
        Console.WriteLine("")
        Console.WriteLine("Type 'quit' to close the application, type 'menu' to return to the menu.")
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
