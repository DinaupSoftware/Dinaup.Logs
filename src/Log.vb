Imports Dinaup.Logs.LogContextSchema
Imports Serilog.Context
Imports Serilog.Events

Public Module Log


    Public Sub SetLoggingLevel(x As LogEventLevel)
        DinaLog.LevelSwitch.MinimumLevel = x
    End Sub

    Public Sub Initialize(Aplicacion$, Version$, Optional logFilePath$ = "logs\log.txt", Optional mmWebHook$ = "", Optional elasticUrl$ = "", Optional elasticPrefix$ = "")
        DinaLog.Initialize(Aplicacion, Version, logFilePath, mmWebHook, elasticUrl, elasticPrefix)
    End Sub

    Public Function IniContext(Name$, Value$) As IDisposable
        Return LogContext.PushProperty(Name, Value)
    End Function

    Public Function IniContext(Context$) As IDisposable
        Return LogContext.PushProperty("Context", Context)
    End Function


    Public Function BeginContext(component$, action$, correlationId$) As LogContextDisposer
        Dim componentContext As IDisposable = LogContext.PushProperty("Component", component)
        Dim actionContext As IDisposable = LogContext.PushProperty("Action", action)
        Dim correlationContext As IDisposable = LogContext.PushProperty("CorrelationId", correlationId)
        Return New LogContextDisposer(componentContext, actionContext, correlationContext)
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







    Public Sub Verbose(messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Verbose(messageTemplate, properyValues)
    End Sub
    Public Sub Debug(messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Debug(messageTemplate, properyValues)
    End Sub
    Public Sub Information(messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Information(messageTemplate, properyValues)
    End Sub
    Public Sub Warning(messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Warning(messageTemplate, properyValues)
    End Sub

    Public Sub [Error](messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Error(messageTemplate, properyValues)
    End Sub
    Public Sub Fatal(messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Fatal(messageTemplate, properyValues)
    End Sub










    Public Function ForContext(Of t)() As Serilog.ILogger
        Return Serilog.Log.ForContext(Of t)
    End Function





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





    Public Sub Verbose(ex As Exception, log$, messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Verbose(ex, log, messageTemplate, properyValues)
    End Sub
    Public Sub Debug(ex As Exception, log$, messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Debug(ex, log, messageTemplate, properyValues)
    End Sub
    Public Sub Information(ex As Exception, log$, messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Information(ex, log, messageTemplate, properyValues)
    End Sub
    Public Sub Warning(ex As Exception, log$, messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Warning(ex, log, messageTemplate, properyValues)
    End Sub

    Public Sub Fatal(ex As Exception, log$, messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Fatal(ex, log, messageTemplate, properyValues)
    End Sub

    Public Sub [Error](ex As Exception, log$, messageTemplate$, ParamArray properyValues() As Object)
        Serilog.Log.Error(ex, log, messageTemplate, properyValues)
    End Sub





End Module
