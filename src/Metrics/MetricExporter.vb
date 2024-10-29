Imports System.Timers
Imports System.Collections.Concurrent
Imports Serilog
Imports System.Text.Json
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.Extensions.Hosting

Public Class MetricExporter


    Private Shared _timer As Timer
    Private Shared _metricsCollection As List(Of Metric)
    Private Shared _httpClient As HttpClient




    Public Shared Sub Flush()
        OnTimedEvent(Nothing, Nothing)
    End Sub

    ' Constructor
    Shared Sub New()
        _metricsCollection = New List(Of Metric)()
        _timer = New Timer(5000)
        AddHandler _timer.Elapsed, AddressOf OnTimedEvent
        _timer.AutoReset = True
        _timer.Enabled = True
    End Sub

    ' Función para agregar una métrica a la colección
    Public Shared Sub AddMetric(metric As Metric)
        SyncLock _metricsCollection
            _metricsCollection.Add(metric)
        End SyncLock
    End Sub
    Public Shared Sub RemoveMetric(metric As Metric)
        SyncLock _metricsCollection
            _metricsCollection.Remove(metric)
        End SyncLock
    End Sub

    Private Shared Function GetIndexName() As String
        Dim currentDate As DateTime = DateTime.UtcNow
        Return $"metrics.{Dinaup.Logs.DinaLog.AppName.Replace(" ", "")}-{currentDate:yyyy.MM}-{Dinaup.Logs.DinaLog.EnvName}"
    End Function


    ' Evento que se dispara cada vez que el temporizador se activa
    Private Shared Sub OnTimedEvent(source As Object, e As ElapsedEventArgs)


        If _httpClient Is Nothing Then
            _httpClient = New HttpClient()
            If Dinaup.Logs.DinaLog.ElasticConfig IsNot Nothing Then
                If Dinaup.Logs.DinaLog.ElasticConfig.Username <> "" Then
                    If Dinaup.Logs.DinaLog.ElasticConfig.Password <> "" Then
                        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(Dinaup.Logs.DinaLog.ElasticConfig.Username & ":" & Dinaup.Logs.DinaLog.ElasticConfig.Password)
                        _httpClient.DefaultRequestHeaders.Authorization = New Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
                    End If
                End If
            End If
        End If



        ' Crear una lista temporal para almacenar las métricas a enviar
        Dim metricsToSend As New List(Of Metric)()



        SyncLock _metricsCollection
            For Each actual In _metricsCollection
                If actual.Value <> 0 Then
                    metricsToSend.Add(actual)
                End If
            Next
        End SyncLock


        ' Si hay métricas para enviar
        If _metricsCollection.Count > 0 Then

            ' Preparar el payload para Bulk API de Elasticsearch
            Dim bulkPayload As New StringBuilder()
            For Each metric In metricsToSend
                metric.App = Dinaup.Logs.DinaLog.AppName
                metric.AppVersion = Dinaup.Logs.DinaLog.AppVersion
                metric.Env = Dinaup.Logs.DinaLog.EnvName
                metric.Timestamp = Date.UtcNow
                ' Acción de índice para cada métrica
                bulkPayload.AppendLine("{""index"":{""_index"":""" & GetIndexName().ToLower & """}}")
                ' Datos de la métrica serializados a JSON
                bulkPayload.AppendLine(JsonSerializer.Serialize(metric))
                metric.Reset()
            Next

            ' Enviar las métricas a Elasticsearch

            Dim content As New StringContent(bulkPayload.ToString(), Encoding.UTF8, "application/x-ndjson")

            Try
                Dim response As HttpResponseMessage = _httpClient.PostAsync("https://elasticsearch.dinaup0.com/_bulk", content).Result
                If response.IsSuccessStatusCode Then
                    Log.Information("Métricas enviadas exitosamente a Elasticsearch.")
                Else
                    Log.Error($"Error al enviar métricas a Elasticsearch: {response.StatusCode} - {response.ReasonPhrase}")
                End If
            Catch ex As Exception
                Log.Error($"Excepción al enviar métricas a Elasticsearch: {ex.Message}")
            End Try

        End If

    End Sub

    Public Shared Sub StopExporter()
        On Error Resume Next
        _timer.Stop()
        OnTimedEvent(Nothing, Nothing)
        _httpClient.Dispose()
    End Sub
End Class

