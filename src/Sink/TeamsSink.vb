Imports Serilog
Imports Serilog.Core
Imports Serilog.Events
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Friend Class TeamsSink
    Implements ILogEventSink

    Private ReadOnly HttpClient As HttpClient
    Private TeamsWebHook As String

    Private contador As Integer
    Private contadorUltimoReset As Date

    Public Sub Emit(logEvent As LogEvent) Implements ILogEventSink.Emit





        If String.IsNullOrWhiteSpace(TeamsWebHook) Then Exit Sub
        If TeamsWebHook.StartsWith("https://") = False Then Exit Sub

        If logEvent.Level <> LogEventLevel.Error AndAlso logEvent.Level <> LogEventLevel.Fatal Then
            Exit Sub
        End If



        If contadorUltimoReset < Date.UtcNow.AddSeconds(-30) Then
            contador = 0
            contadorUltimoReset = Date.UtcNow
        End If
        contador += 1
        If contador > 20 Then Exit Sub





        Try


            ' Obtener el call stack
            Dim callStack As String = If(logEvent.Exception IsNot Nothing, logEvent.Exception.StackTrace, Environment.StackTrace)
            Dim Titulo As String = If(logEvent.Exception IsNot Nothing, "EXCEPTION", logEvent.Level.ToString())

            ' Formatear el mensaje como una "Adaptive Card" de Microsoft Teams
            Dim card As New With {
                    .type = "MessageCard",
                    .context = "http://schema.org/extensions",
                    .summary = "Error Notification",
                    .themeColor = "FF0000",
                    .title = "❌ " & Titulo & " ❌",
                    .sections = New Object() {
                        New With {
                            .activityTitle = "**Level:** " & logEvent.Level.ToString(),
                            .activitySubtitle = "**Timestamp:** " & logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                            .text = "**Message:** " & logEvent.RenderMessage()
                        },
                        New With {
                            .text = If(logEvent.Exception IsNot Nothing, "**Exception:** " & logEvent.Exception.Message, "")
                        },
                        New With {
                            .facts = If(logEvent.Properties.Count > 0, logEvent.Properties.Select(Function(p) New With {.name = p.Key, .value = p.Value.ToString()}).ToArray(), Nothing)
                        },
                        New With {
                            .text = "**Call Stack:**" & Environment.NewLine & "```" & callStack & "```"
                        }
                    }
                }

            ' Serializar y enviar la carga útil al webhook de Teams
            Dim jsonMessage As String = JsonSerializer.Serialize(card)
            SendToTeams(jsonMessage)
        Catch
        End Try


    End Sub

    Private Async Sub SendToTeams(jsonMessage As String)
        Try
            Using content As New StringContent(jsonMessage, Encoding.UTF8, "application/json")
                Dim response = Await HttpClient.PostAsync(TeamsWebHook, content)
                If Not response.IsSuccessStatusCode Then
                    Console.WriteLine($"Error sending to Teams: {response.StatusCode}")
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine($"Exception when sending to Teams: {ex.Message}")
        End Try
    End Sub

    Sub New(_TeamsWebHook As String)
        HttpClient = New HttpClient()
        TeamsWebHook = _TeamsWebHook
    End Sub
End Class
