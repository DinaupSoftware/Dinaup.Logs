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

        Private _componentContext As IDisposable
        Private _actionContext As IDisposable
        Private _correlationContext As IDisposable
        Public Sub New(componentContext As IDisposable, actionContext As IDisposable)
            _componentContext = componentContext
            _actionContext = actionContext
        End Sub

        Public Sub New(componentContext As IDisposable, actionContext As IDisposable, correlationContext As IDisposable)
            _componentContext = componentContext
            _actionContext = actionContext
            _correlationContext = correlationContext
        End Sub

        Private Sub Dispose(disposing As Boolean)
            If disposing Then
                _componentContext?.Dispose()
                _actionContext?.Dispose()
                _correlationContext?.Dispose()
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Class
