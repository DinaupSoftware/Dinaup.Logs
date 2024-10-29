Imports System.Timers



Public MustInherit Class Metric
    Implements IDisposable

    Private disposedValue As Boolean
    Public Property Type As String
    Public Property MetricName As String
    Public Property Value As Decimal
    Public Property Unit As String = "count"
    Public Property Timestamp As DateTime = DateTime.UtcNow

    Public Property AppVersion As String
    Public Property App As String
    Public Property Env As String
    Public Property Tags As Dictionary(Of String, String)





    Public Sub Reset()
        Me.Value = 0
    End Sub

    Public Sub New(metricType As String, metricName As String, Optional unit As String = "count", Optional _tags As Dictionary(Of String, String) = Nothing)
        Me.Tags = _tags
        Me.Type = metricType
        Me.MetricName = metricName
        Me.Unit = unit
        MetricExporter.AddMetric(Me)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                MetricExporter.RemoveMetric(Me)
            End If
            disposedValue = True
        End If
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        ' No cambie este código. Coloque el código de limpieza en el método "Dispose(disposing As Boolean)".
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class



