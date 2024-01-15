Imports Serilog.Context

Public Module Log


    Public Function IniContext(Name$, Value$) As IDisposable
        Return LogContext.PushProperty(Name, Value)
    End Function

    Public Function IniContext(Context$) As IDisposable
        Return LogContext.PushProperty("Context", Context)
    End Function

    Public Sub CloseAndFlush()
        Serilog.Log.CloseAndFlush()
    End Sub

    Public Sub Verbose(log$)
        Serilog.Log.Verbose(log)
    End Sub
    Public Sub Debug(log$)
        Serilog.Log.Debug(log)
    End Sub
    Public Sub Information(log$)
        Serilog.Log.Information(log)
    End Sub
    Public Sub Warning(log$)
        Serilog.Log.Warning(log)
    End Sub





    Public Sub [Error](log$)
        Serilog.Log.Error(log)
    End Sub
    Public Sub Fatal(log$)
        Serilog.Log.Fatal(log)
    End Sub

















    Public Sub Verbose(ex As Exception, log$)
        Serilog.Log.Verbose(ex, log)
    End Sub
    Public Sub Debug(ex As Exception, log$)
        Serilog.Log.Debug(ex, log)
    End Sub
    Public Sub Information(ex As Exception, log$)
        Serilog.Log.Information(ex, log)
    End Sub
    Public Sub Warning(ex As Exception, log$)
        Serilog.Log.Warning(ex, log)
    End Sub

    Public Sub Fatal(ex As Exception, log$)
        Serilog.Log.Fatal(ex, log)
    End Sub

    Public Sub [Error](ex As Exception, log$)
        Serilog.Log.Error(ex, log)
    End Sub
End Module
