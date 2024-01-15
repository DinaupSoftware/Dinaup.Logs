Imports System
Imports System.Text.Json
Imports System.Text.Json.Serialization

Public Class LogEntry
    Public Property Timestamp As DateTime
    Public Property Level As String
    Public Property MessageTemplate As String
    Public Property Properties As LogProperties
End Class

Public Class LogProperties
    Public Property Aplicacion As String
    Public Property MachineName As String
    Public Property Version As String
    Public Property ThreadId As Integer
    Public Property job As String
End Class
