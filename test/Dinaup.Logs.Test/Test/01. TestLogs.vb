Imports System
Imports Xunit

Public Class Test








    <Fact>
    Public Sub ExceptionsToTeams()
        Dinaup.Logs.Initialize("Dinaup.Logs.Test", "2.0", Nothing, New IntegrationsConfig() With {.TeamsWebhook = TeamsWebHook})
        Dinaup.Logs.Fatal("Mensaje manual enviado a Teams")
        Try
            Throw New Exception("Exception reportada a Teams")
        Catch ex As Exception
            Dinaup.Logs.Fatal(ex, "Exception")
        End Try
        Dinaup.Logs.CloseAndFlush()
    End Sub




    <Fact>
    Public Sub LogElasticSearch()


        Dinaup.Logs.Initialize("Dinaup.Logs.Test", "2.0", New ElasticConfig() With {.Endpoint = ElasticSearchEndPoint, .Username = ElasticSearchUser, .Password = ElasticSearchPass})

        Dinaup.Logs.Verbose("Log Verbose")
        Dinaup.Logs.Debug("Log Debug")
        Dinaup.Logs.Information("Log Information")
        Dinaup.Logs.Error("Log Error")
        Dinaup.Logs.Fatal("Log Fatal")



        Dinaup.Logs.SetLoggingLevel(Serilog.Events.LogEventLevel.Debug)
        Dinaup.Logs.Verbose("No se envía Log Verbose")
        Dinaup.Logs.Debug("Sí se envia")






        Try
            Throw New Exception("Exception reportada a Teams")
        Catch ex As Exception
            Dinaup.Logs.Fatal(ex, "Exception")
        End Try
        Dinaup.Logs.CloseAndFlush()
    End Sub




    <Fact>
    Public Sub Context()


        Dinaup.Logs.Initialize("Dinaup.Logs.Test", "2.0", New ElasticConfig() With {.Endpoint = ElasticSearchEndPoint, .Username = ElasticSearchUser, .Password = ElasticSearchPass})


        Using Dinaup.Logs.BeginContext(NameOf(Test), NameOf(Context))
            Dinaup.Logs.Debug("Log Debug")
        End Using



        Dinaup.Logs.CloseAndFlush()

    End Sub




    <Fact>
    Public Sub ContextWithCorrelation()


        Dinaup.Logs.Initialize("Dinaup.Logs.Test", "2.0", New ElasticConfig() With {.Endpoint = ElasticSearchEndPoint, .Username = ElasticSearchUser, .Password = ElasticSearchPass})
        Using Dinaup.Logs.BeginCorrelationContext(NameOf(Test), NameOf(Context), Guid.NewGuid.ToString())
            Dinaup.Logs.Debug("Este log tiene  Correlacion id ")
            OtraFuncion()
        End Using
        Dinaup.Logs.CloseAndFlush()

    End Sub

    Public Sub OtraFuncion()
        Using Dinaup.Logs.BeginContext(NameOf(Test), NameOf(OtraFuncion))
            Dinaup.Logs.Debug("Este log cambia de contexto pero sigue teniendo la misma correlación ID")
        End Using
    End Sub








End Class