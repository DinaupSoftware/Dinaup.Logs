Imports System.Net.Http
Imports System.Text
Imports Serilog
Imports System.IO
Imports Serilog.Core
Imports Serilog.Events
Imports System.Collections.Concurrent
Imports System.Text.Json
Imports System.Timers
Imports Serilog.Sinks.Elasticsearch
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Friend Class DinaLog


    Private Shared _mmWebHook As String



    Public Shared Function ReadLog(JSON_FilePath_FileURL As String) As LogEntry()
        Dim ContenidoTXT = ___readcontent(JSON_FilePath_FileURL)
        If String.IsNullOrWhiteSpace(ContenidoTXT) Then Return New LogEntry() {}
        Dim json = ContenidoTXT.Replace(vbCrLf, ",")
        json = json.Substring(0, json.Length - 1)
        Dim objList = System.Text.Json.JsonSerializer.Deserialize(Of LogEntry())("[" & json & "]")
        Return objList
    End Function

    Private Shared Function ___readcontent(JsonORutaDiscoORutaHTTP As String) As String
        If String.IsNullOrWhiteSpace(JsonORutaDiscoORutaHTTP) Then Return String.Empty
        If JsonORutaDiscoORutaHTTP.Contains("{"c) Then Return JsonORutaDiscoORutaHTTP
        If JsonORutaDiscoORutaHTTP.StartsWith("http://") OrElse JsonORutaDiscoORutaHTTP.StartsWith("https://") Then
            Using client = New HttpClient()
                Return client.GetStringAsync(JsonORutaDiscoORutaHTTP).Result
            End Using
        ElseIf File.Exists(JsonORutaDiscoORutaHTTP) Then
            ' Si es una ruta de archivo, lee el archivo y devuelve su contenido
            Return File.ReadAllText(JsonORutaDiscoORutaHTTP, Encoding.UTF8)
        Else
            ' Si no es una URL ni una ruta de archivo válida, asume que es un JSON directo
            Return JsonORutaDiscoORutaHTTP
        End If
    End Function



    Public Shared LevelSwitch As New LoggingLevelSwitch(LogEventLevel.Information)


    Public Shared Sub SetLoggingLevel(x As LogEventLevel)
        LevelSwitch.MinimumLevel = x
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="_aplicationName$"></param>
    ''' <param name="_applicationVersion$"></param>
    ''' <param name="_logFilePath$"></param>
    ''' <param name="__mmWebHook$"></param>
    ''' <param name="_elasticUrl$"></param>
    ''' <param name="_elasticPrefix$">logs.MiApp-{0:yyyy.MM}"</param>
    Public Shared Sub Initialize(_aplicationName$, _applicationVersion$, Optional _logFilePath$ = "logs\log.txt", Optional __mmWebHook$ = "", Optional _elasticUrl$ = "", Optional _elasticPrefix$ = "")


        Dim esDebug = ""

#If DEBUG Then
        esDebug = " [DEBUG]"
#End If



        Dim logger As New LoggerConfiguration()
        logger.MinimumLevel.ControlledBy(LevelSwitch)
        logger.Enrich.WithProperty("Aplicacion", _aplicationName)
        logger.Enrich.WithProperty("MachineName", System.Environment.MachineName)
        logger.Enrich.WithProperty("Version", _applicationVersion & esDebug)
        logger.Enrich.WithThreadId()
        logger.Enrich.WithThreadName()
        logger.Enrich.FromLogContext()
        logger.Enrich.WithMachineName()
        logger.Enrich.WithEnvironmentName()
        logger.Enrich.WithEnvironmentUserName()
        logger.WriteTo.Console(LogEventLevel.Verbose)

        ' Configuración de Elasticsearch con buffering
        If _elasticUrl <> "" Then
            If _elasticPrefix = "" Then _elasticPrefix = "logs." & _aplicationName.Replace(" ", "") & "-{0:yyyy.MM}"
            logger.WriteTo.Elasticsearch(New ElasticsearchSinkOptions(New Uri(_elasticUrl)) With {
            .IndexFormat = _elasticPrefix,
            .BatchPostingLimit = 1000, ' Número máximo de eventos a enviar en un lote
            .BufferBaseFilename = "buffer-logs", ' Archivo de buffer para almacenar registros temporalmente
            .BufferFileSizeLimitBytes = 50 * 1024 * 1024, ' Tamaño máximo del archivo de buffer (50 MB)
            .BufferLogShippingInterval = TimeSpan.FromSeconds(5) ' Intervalo de tiempo para enviar los registros en buffer
        })
        End If


        If __mmWebHook <> "" Then
            logger.WriteTo.Sink(New MatterMostSink(__mmWebHook))
        End If
        logger.WriteTo.File(formatter:=New Serilog.Formatting.Json.JsonFormatter(), path:=_logFilePath,
                rollingInterval:=RollingInterval.Day,
                fileSizeLimitBytes:=100 * 1024 * 1024, ' 100MB como ejemplo
                retainedFileCountLimit:=7, ' Mantener solo 7 archivos (una semana) si se usa RollingInterval.Day
                shared:=True)

        '    ' Configuración del archivo de logs con buffering
        '    logger.WriteTo.File(
        '    formatter:=New Serilog.Formatting.Json.JsonFormatter(),
        '    path:=_logFilePath,
        '    rollingInterval:=RollingInterval.Day,
        '    fileSizeLimitBytes:=100 * 1024 * 1024, ' 100 MB como ejemplo
        '    retainedFileCountLimit:=7, ' Mantener solo 7 archivos (una semana) si se usa RollingInterval.Day
        '    shared:=True,
        '    buffered:=True, ' Habilitar buffer
        '    flushToDiskInterval:=TimeSpan.FromSeconds(3) ' Intervalo de tiempo para enviar los registros en buffer
        ')

        Serilog.Log.Logger = logger.CreateLogger()


        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledExceptionTrapper
        AddHandler TaskScheduler.UnobservedTaskException, AddressOf UnobservedTaskExceptionTrapper



    End Sub



    Private Shared Sub UnhandledExceptionTrapper(sender As Object, e As UnhandledExceptionEventArgs)
        Try
            Dim ex As Exception = CType(e.ExceptionObject, Exception)
            Fatal(ex, "UnhandledException")
        Catch exf As Exception
        End Try

    End Sub

    Private Shared Sub UnobservedTaskExceptionTrapper(sender As Object, e As UnobservedTaskExceptionEventArgs)
        Try
            e.SetObserved()
            Dim ex = e.Exception
            Fatal(ex, "UnobservedTaskException")
        Catch
        End Try
    End Sub



End Class

