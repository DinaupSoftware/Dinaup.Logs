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







    Public Shared LevelSwitch As New LoggingLevelSwitch(LogEventLevel.Verbose)


    Public Shared Sub SetLoggingLevel(x As LogEventLevel)
        LevelSwitch.MinimumLevel = x
    End Sub


    Private Shared elasticSearchURL$
    Private Shared httpClient As New HttpClient() With {.Timeout = TimeSpan.FromSeconds(5)}
    Public Shared Async Function ElasticSearchIsAvailable() As Task(Of Boolean)
        Try
            Dim response As HttpResponseMessage = Await httpClient.GetAsync(elasticSearchURL)
            Return response.IsSuccessStatusCode
        Catch ex As HttpRequestException
            Return False
        Catch ex As TaskCanceledException
            Return False
        End Try
    End Function


    Friend Shared ElasticConfig As ElasticConfig
    Friend Shared AppName$
    Friend Shared AppVersion$
    Friend Shared EnvName$


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="_aplicationName$"></param>
    ''' <param name="_applicationVersion$"></param>
    ''' <param name="_logFilePath$"></param>
    ''' <param name="_mattermostWebHook$"></param>
    Public Shared Sub Initialize(_aplicationName$, _applicationVersion$, Optional _logFilePath$ = "logs\log.txt", Optional _elasticConfig As ElasticConfig = Nothing, Optional integrationsConfig As IntegrationsConfig = Nothing, Optional _enviroment As String = "Release")
        ElasticConfig = _elasticConfig
        AppName = _aplicationName
        AppVersion = _applicationVersion
        EnvName = _enviroment




        Dim esDebug = ""

#If DEBUG Then
        esDebug = " [DEBUG]"
#End If


        Dim ElasticLogs As ElasticsearchSinkOptions



        Dim prefix = ""


        If ElasticConfig IsNot Nothing AndAlso Not String.IsNullOrEmpty(ElasticConfig.Endpoint) Then

            prefix = ElasticConfig.IndexPrefix
            elasticSearchURL = ElasticConfig.Endpoint

            ' Configuración para ElasticLogs
            ElasticLogs = New ElasticsearchSinkOptions(New Uri(elasticSearchURL)) With {
        .IndexFormat = "logs." & _aplicationName.Replace(" ", "") & "-{0:yyyy.MM}-" & _enviroment.ToLower,
        .BatchPostingLimit = 1000,  ' Número máximo de eventos a enviar en cada lote
        .BufferBaseFilename = "buffer-logs",  ' Nombre del archivo de buffer para almacenamiento temporal
        .BufferFileSizeLimitBytes = 50 * 1024 * 1024,  ' Tamaño máximo del archivo de buffer (50 MB)
        .BufferLogShippingInterval = TimeSpan.FromSeconds(5)  ' Intervalo de tiempo para enviar los registros en buffer
    }



            ' Si se proporcionan usuario y contraseña, se agrega la autenticación básica
            If Not String.IsNullOrEmpty(ElasticConfig.Username) AndAlso Not String.IsNullOrEmpty(ElasticConfig.Password) Then
                ElasticLogs.ModifyConnectionSettings = Function(connection) connection.BasicAuthentication(ElasticConfig.Username, ElasticConfig.Password)
            End If
        End If



        Dim logger As New LoggerConfiguration()
        logger.MinimumLevel.ControlledBy(LevelSwitch)
        logger.Enrich.WithProperty("Aplicacion", _aplicationName)
        logger.Enrich.WithProperty("MachineName", System.Environment.MachineName)
        logger.Enrich.WithProperty("Version", _applicationVersion & esDebug)
        logger.Enrich.WithProperty("Environment", _enviroment)


        logger.Enrich.WithThreadId()
        logger.Enrich.WithThreadName()
        logger.Enrich.FromLogContext()
        logger.Enrich.WithMachineName()
        logger.Enrich.WithEnvironmentName()
        logger.Enrich.WithEnvironmentUserName()
        logger.WriteTo.Console(LogEventLevel.Verbose)



        If ElasticLogs IsNot Nothing Then
            logger.WriteTo.Elasticsearch(ElasticLogs)
        End If



        If integrationsConfig IsNot Nothing Then
            If integrationsConfig.MattermostWebhook <> "" Then logger.WriteTo.Sink(New MatterMostSink(integrationsConfig.MattermostWebhook))
            If integrationsConfig.TeamsWebhook <> "" Then logger.WriteTo.Sink(New TeamsSink(integrationsConfig.TeamsWebhook))
        End If


        logger.WriteTo.File(formatter:=New Serilog.Formatting.Json.JsonFormatter(), path:=_logFilePath,
                rollingInterval:=RollingInterval.Day,
                fileSizeLimitBytes:=100 * 1024 * 1024, ' 100MB como ejemplo
                retainedFileCountLimit:=7, ' Mantener solo 7 archivos (una semana) si se usa RollingInterval.Day
                shared:=True)
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

