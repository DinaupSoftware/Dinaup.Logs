Imports System.Net.Http
Imports System.Text
Imports Serilog
Imports System.IO
Imports Serilog.Core
Imports Serilog.Events
Imports System.Collections.Concurrent
Imports System.Text.Json
Imports System.Timers
Public Class DinaLog


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


    Public Shared Sub Ini(Aplicacion$, Version$, Optional logFilePath$ = "logs\log.txt", Optional mmWebHook$ = "")


        Dim esDebug = ""

#If DEBUG Then
        esDebug = " [DEBUG]"
#End If


        Dim logger As New LoggerConfiguration()
        logger.Enrich.WithProperty("Aplicacion", Aplicacion)
        logger.Enrich.WithProperty("MachineName", System.Environment.MachineName)
        logger.Enrich.WithProperty("Version", Version & esDebug)
        logger.Enrich.WithThreadId()
        logger.Enrich.WithThreadName()
        logger.Enrich.FromLogContext()
        logger.Enrich.WithMachineName()
        logger.Enrich.WithEnvironmentName()
        logger.Enrich.WithEnvironmentUserName()
        logger.WriteTo.Console()
        logger.WriteTo.Sink(New MatterMostSink(mmWebHook))

        logger.WriteTo.File(formatter:=New Serilog.Formatting.Json.JsonFormatter(), path:=logFilePath,
                    rollingInterval:=RollingInterval.Day,
                    fileSizeLimitBytes:=100 * 1024 * 1024, ' 100MB como ejemplo
                    retainedFileCountLimit:=7, ' Mantener solo 7 archivos (una semana) si se usa RollingInterval.Day
                    shared:=True)

        Serilog.Log.Logger = logger.CreateLogger()


    End Sub







End Class

