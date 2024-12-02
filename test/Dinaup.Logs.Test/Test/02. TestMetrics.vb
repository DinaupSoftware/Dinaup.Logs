Imports System
Imports Xunit

Public Class TestMetrics









    <Fact>
    Public Sub TestCounterMetric()


        Dinaup.Logs.Initialize("Dinaup.Logs.Test", "2.0", New ElasticConfig() With {.Endpoint = ElasticSearchEndPoint, .Username = ElasticSearchUser, .Password = ElasticSearchPass})





        If True Then
            Dim Counter_ArchivosSubidos = New Dinaup.Logs.CounterMetric("ArchivosSubidos")
            Counter_ArchivosSubidos.Increment()
            Counter_ArchivosSubidos.Increment()
            Counter_ArchivosSubidos.Increment()
            Counter_ArchivosSubidos.Increment()
            Counter_ArchivosSubidos.Increment()
            MetricExporter.Flush()
            Counter_ArchivosSubidos.Increment()
            Counter_ArchivosSubidos.Increment()
            Counter_ArchivosSubidos.Increment()
            MetricExporter.Flush()
            Counter_ArchivosSubidos.Dispose()
        End If



        For Each usuarioActual In {"Salva", "Angel", "Paco de Lucia"}


            Dim SuidoUsuario = New Dinaup.Logs.CounterMetric("Llamadas API", "request", 0, New Dictionary(Of String, String) From {{"usuario", usuarioActual}})
            SuidoUsuario.Increment()
            SuidoUsuario.Increment()
            SuidoUsuario.Increment()
            SuidoUsuario.Increment()
            SuidoUsuario.Increment()
            MetricExporter.Flush()
            SuidoUsuario.Increment()
            SuidoUsuario.Increment()
            SuidoUsuario.Increment()
            MetricExporter.Flush()
            SuidoUsuario.Dispose()


        Next










    End Sub




    <Fact>
    Public Sub TestHistogram()



        Dinaup.Logs.Initialize("Dinaup.Logs.Test", "2.0", New ElasticConfig() With {.Endpoint = ElasticSearchEndPoint, .Username = ElasticSearchUser, .Password = ElasticSearchPass})

        ' Definir los rangos de los buckets para el histograma de tiempos de respuesta (en ms)
        Dim responseTimeBuckets As New List(Of Tuple(Of Double, Double)) From {
            Tuple.Create(0.0, 100.0),
            Tuple.Create(100.0, 200.0),
            Tuple.Create(200.0, 300.0),
            Tuple.Create(300.0, 400.0),
            Tuple.Create(400.0, 500.0)
        }

        ' Definir etiquetas adicionales para categorizar el histograma
        Dim tags As New Dictionary(Of String, String) From {
            {"endpoint", "api/v1/resource"},
            {"usuario", "paco"}
        }

        ' Crear una instancia de HistogramMetric para medir los tiempos de respuesta de la API
        Using responseTimeHistogram = New Dinaup.Logs.HistogramMetric("TiemposRespuestaAPI", "durantion_request", tags, responseTimeBuckets)

            ' Simular el registro de diferentes tiempos de respuesta de la API
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(50)    ' Incrementa el bucket 0-100 ms
            responseTimeHistogram.Record(150)   ' Incrementa el bucket 100-200 ms
            responseTimeHistogram.Record(250)   ' Incrementa el bucket 200-300 ms
            responseTimeHistogram.Record(350)   ' Incrementa el bucket 300-400 ms
            responseTimeHistogram.Record(450)   ' Incrementa el bucket 400-500 ms
            responseTimeHistogram.Record(-10)   ' Incrementa el bucket underflow
            responseTimeHistogram.Record(600)   ' Incrementa el bucket overflow

            ' Exportar las métricas actuales al sistema de métricas (por ejemplo, Elasticsearch)
            MetricExporter.Flush()

            ' Simular el registro de más tiempos de respuesta de la API
            responseTimeHistogram.Record(120)   ' Incrementa el bucket 100-200 ms
            responseTimeHistogram.Record(220)   ' Incrementa el bucket 200-300 ms
            responseTimeHistogram.Record(320)   ' Incrementa el bucket 300-400 ms

            ' Exportar las métricas actualizadas al sistema de métricas
            MetricExporter.Flush()

        End Using

        ' Confirmación de que las métricas han sido exportadas correctamente
        Console.WriteLine("Histogram metrics exported successfully.")
    End Sub


End Class