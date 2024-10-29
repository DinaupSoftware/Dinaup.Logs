
Module Program
    Sub Main(args As String())




        ' Configuración de Serilog
        Dinaup.Logs.Initialize("LogTest", "0.1", logFilePath:="log/log.txt", mmWebHook:="matte", elasticUrl:="http://localhost:9200", elasticPrefix:="")


        ' Generar logs de prueba
        Dinaup.Logs.Information("Iniciando pruebas de Serilog.")

        ' Llamadas a funciones de prueba
        GenerarLogsTexto()
        GenerarLogsNumeros()
        GenerarLogsExcepciones()


        Using Dinaup.Logs.IniContext("sync")
            Dinaup.Logs.Log.Information("Log de texto número {NumeroLog}", 2)
        End Using


        Dinaup.Logs.Log.Information("Pruebas de Serilog finalizadas.")
        Dinaup.Logs.CloseAndFlush()
        Console.ReadKey()

    End Sub

    ' Función para generar logs de texto
    Sub GenerarLogsTexto()
        For i As Integer = 1 To 1000
            dinaup.logs.Log.Information("Log de texto número {NumeroLog}", i)
        Next
    End Sub

    ' Función para generar logs con números
    Sub GenerarLogsNumeros()
        Dim rand As New Random()
        For i As Integer = 1 To 1000
            dinaup.logs.Log.Debug("Log con número aleatorio {NumeroAleatorio}", rand.Next())
        Next
    End Sub

    ' Función para simular y registrar excepciones
    Sub GenerarLogsExcepciones()
        For i As Integer = 1 To 100
            Try
                Throw New InvalidOperationException("Excepción de prueba número " & i.ToString())
            Catch ex As Exception
                dinaup.logs.Log.Error(ex, "Se capturó una excepción")
            End Try
        Next
    End Sub

End Module
    