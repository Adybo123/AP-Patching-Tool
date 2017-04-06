Module APPatcherMain

    Sub Main()
        Console.WriteLine("AP Patching Tool")
        Console.WriteLine("")
        Console.WriteLine(TakeConsoleInput())
    End Sub

    Function TakeConsoleInput()
        Console.Write(">")
        Dim ResponseText As String = Console.ReadLine
        Return ResponseText
    End Function

End Module
