Imports Serilog.Context

Public Class LogContextSchema

    Default Public ReadOnly Property WithAction(action As String) As LogContextSchema
        Get
            Return CreateNewAction(action)
        End Get
    End Property

    Public Property Component As String
    Public Property Action As String
    Public Property CorrelationId As String


    Sub New(_Component As String)
        Me.Component = _Component
    End Sub
    Sub New()

    End Sub
    Public Function CreateNewAction(action As String) As LogContextSchema
        Dim newSchema As New LogContextSchema With {.Component = Me.Component, .Action = action, .CorrelationId = Me.CorrelationId}
        Return newSchema
    End Function

    Public Function BeginContext(newAction$) As LogContextDisposer
        Dim componentContext As IDisposable = LogContext.PushProperty("Component", Me.Component)
        Dim actionContext As IDisposable = LogContext.PushProperty("Action", newAction)
        Dim correlationContext As IDisposable = LogContext.PushProperty("CorrelationId", Me.CorrelationId)
        Return New LogContextDisposer(componentContext, actionContext, correlationContext)
    End Function

    Public Function BeginContext(newAction$, newCorrelationId$) As LogContextDisposer
        Dim componentContext As IDisposable = LogContext.PushProperty("Component", Me.Component)
        Dim actionContext As IDisposable = LogContext.PushProperty("Action", newAction)
        Dim correlationContext As IDisposable = LogContext.PushProperty("CorrelationId", newCorrelationId)
        Return New LogContextDisposer(componentContext, actionContext, correlationContext)
    End Function

    Public Class LogContextDisposer
        Implements IDisposable

        Private _componentContext() As IDisposable
        Public Sub New(ParamArray disposablelist() As IDisposable)
            _componentContext = disposablelist
        End Sub
        Private Sub Dispose(disposing As Boolean)
            If disposing Then
                If _componentContext IsNot Nothing Then
                    If _componentContext.Length > 0 Then
                        For i = 0 To _componentContext.Length - 1
                            _componentContext(i).Dispose()
                        Next
                    End If
                End If
                _componentContext = Nothing
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Class
