Imports Serilog
Imports Serilog.Core
Imports Serilog.Events
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Friend Class MatterMostSink
    Implements ILogEventSink


    Private ReadOnly HttpClient As HttpClient
    Private MatterMostWebHook$


    Private contador As Integer
    Private contadorUltimoReset As Date



    Public Sub Emit(logEvent As LogEvent) Implements ILogEventSink.Emit


        If MatterMostWebHook <> "" AndAlso MatterMostWebHook.StartsWith("http") Then

            If logEvent.Level = LogEventLevel.Error OrElse logEvent.Level = LogEventLevel.Fatal Then

                If contadorUltimoReset < Date.UtcNow.AddSeconds(-30) Then
                    contador = 0
                    contadorUltimoReset = Date.UtcNow
                End If

                contador += 1
                If contador > 10 Then Exit Sub




                Dim stringBuilder As New StringBuilder
                stringBuilder.AppendLine("❌ EXCEPTION ❌")
                stringBuilder.AppendLine("----")
                stringBuilder.AppendLine($"**Level:** {logEvent.Level}")
                stringBuilder.AppendLine($"**Timestamp:** {logEvent.Timestamp:yyyy-MM-dd HH:mm:ss}")
                stringBuilder.AppendLine($"**Message:** {logEvent.RenderMessage}")

                If logEvent.Exception IsNot Nothing Then
                    stringBuilder.AppendLine($"**Exception:** {logEvent.Exception.GetType.FullName}")
                    stringBuilder.AppendLine($"**Message:** {logEvent.Exception.Message}")
                End If

                ' Incluir propiedades adicionales del logEvent si son necesarias
                If logEvent.Properties.Count > 0 Then
                    stringBuilder.AppendLine("**Properties:**")
                    For Each propertyActual In logEvent.Properties
                        stringBuilder.AppendLine("  - " & propertyActual.Key & ": " & propertyActual.Value.ToString)
                    Next
                End If

                stringBuilder.AppendLine("------")
                If logEvent.Exception IsNot Nothing Then
                    stringBuilder.AppendLine($"**Stack Trace:**{Environment.NewLine}```{logEvent.Exception.StackTrace}```")
                Else
                    stringBuilder.AppendLine($"**Stack Trace:**{Environment.NewLine}```{Environment.StackTrace }```")
                End If



                Dim message = New With {.text = stringBuilder.ToString}
                SendToMatterMost(JsonSerializer.Serialize(message))
            End If

        End If
    End Sub


    Private Async Sub SendToMatterMost(jsonMessage As String)
        Try
            Using content As New StringContent(jsonMessage, Encoding.UTF8, "application/json")
                ' Enviar el mensaje al webhook de Mattermost
                Dim response = Await HttpClient.PostAsync(MatterMostWebHook, content)
                If Not response.IsSuccessStatusCode Then
                    ' Manejo de errores en caso de que la respuesta no sea exitosa
                    Console.WriteLine($"Error al enviar a Mattermost: {response.StatusCode}")
                End If
            End Using
        Catch ex As Exception
            ' Manejo de excepciones
            Console.WriteLine($"Excepción al enviar a Mattermost: {ex.Message}")
        End Try
    End Sub

    Sub New(_MatterMostWebHook$)
        HttpClient = New HttpClient
        MatterMostWebHook = _MatterMostWebHook$
    End Sub
End Class

