Imports System.Collections.Concurrent
Imports System.Diagnostics.Metrics

Public Class CounterMetric
    Inherits Metric



    Public Sub New(metricName As String, Optional unit As String = "count", Optional initial As Decimal = 0, Optional tags As Dictionary(Of String, String) = Nothing)
        MyBase.New("Counter", metricName, unit, tags)
    End Sub



    Public Sub Increment(Optional value As Decimal = 1)
        Me.Value += value
    End Sub


End Class

Public Class HistogramMetric
    Implements IDisposable

    Private ReadOnly _name As String
    Private ReadOnly _metricType As String
    Private ReadOnly _tags As Dictionary(Of String, String)
    Private ReadOnly _buckets As List(Of Tuple(Of Double, Double))
    Private ReadOnly _counters As ConcurrentDictionary(Of String, CounterMetric)
    Private _isDisposed As Boolean = False

    ''' <summary>
    ''' Constructor de HistogramMetric.
    ''' </summary>
    ''' <param name="name">Nombre del histograma.</param>
    ''' <param name="metricType">Tipo de métrica (por ejemplo, "response_time").</param>
    ''' <param name="initialValue">Valor inicial (usualmente 0).</param>
    ''' <param name="tags">Etiquetas para categorizar la métrica.</param>
    ''' <param name="buckets">Lista de tuplas que representan los rangos de los buckets (min, max).</param>
    Public Sub New(name As String, metricType As String, tags As Dictionary(Of String, String), buckets As List(Of Tuple(Of Double, Double)))
        _name = name
        _metricType = metricType
        _tags = tags
        _buckets = buckets
        _counters = New ConcurrentDictionary(Of String, CounterMetric)()

        ' Inicializar contadores para cada bucket
        For Each bucket In _buckets
            Dim bucketKey As String = GetBucketKey(bucket.Item1, bucket.Item2)
            Dim bucketTags As New Dictionary(Of String, String)(_tags)
            bucketTags.Add("range", $"{bucket.Item1} - {bucket.Item2}")
            Dim counter As New CounterMetric(_name, _metricType, 0, bucketTags)
            counter.Type = "Histogram"
            _counters.TryAdd(bucketKey, counter)
        Next
    End Sub

    ''' <summary>
    ''' Registra un valor en el histograma, incrementando el contador del bucket correspondiente.
    ''' </summary>
    ''' <param name="value">Valor a registrar.</param>
    Public Sub Record(value As Double)
        For Each bucket In _buckets
            If value >= bucket.Item1 AndAlso value < bucket.Item2 Then
                Dim bucketKey As String = GetBucketKey(bucket.Item1, bucket.Item2)
                Dim counter As CounterMetric
                If _counters.TryGetValue(bucketKey, counter) Then
                    counter.Increment()
                    Exit Sub
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Obtiene la clave única para un bucket dado su rango.
    ''' </summary>
    ''' <param name="min">Valor mínimo del bucket.</param>
    ''' <param name="max">Valor máximo del bucket.</param>
    ''' <returns>Clave única como cadena.</returns>
    Private Function GetBucketKey(min As Double, max As Double) As String
        Return $"{min}-{max}"
    End Function



    ''' <summary>
    ''' Limpia y libera recursos.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        If Not _isDisposed Then
            For Each counter In _counters.Values
                counter.Dispose()
            Next
            _isDisposed = True
        End If
    End Sub
End Class