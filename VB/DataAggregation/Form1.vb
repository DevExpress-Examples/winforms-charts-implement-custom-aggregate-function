Imports DevExpress.XtraCharts
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace DataAggregation

    Public Partial Class Form1
        Inherits DevExpress.XtraEditors.XtraForm

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Function GenerateData(ByVal pointCount As Integer) As List(Of DataPoint)
            Dim result As List(Of DataPoint) = New List(Of DataPoint)(pointCount)
            Dim value As Double = 0
            Dim argument As DateTime = DateTime.Now.AddHours(-pointCount)
            Dim random As Random = New Random()
            For i As Double = 0 To pointCount - 1
                result.Add(New DataPoint With {.Argument = argument.AddHours(i), .Value = Math.Abs(value)})
                value +=(random.NextDouble() * 10.0 - 5.0)
            Next

            Return result
        End Function

#Region "#CustomAggregateFunction"
        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim series As Series = chartControl.Series("Random Data")
            series.DataSource = Me.GenerateData(100_000)
            series.ArgumentDataMember = "Argument"
            series.ValueDataMembers.AddRange("Value", "Value", "Value", "Value")
            Dim diagram As XYDiagram = TryCast(chartControl.Diagram, XYDiagram)
            diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.Custom
            diagram.AxisX.DateTimeScaleOptions.CustomAggregateFunction = New OhlcAggregateFunction()
        End Sub

        Private Class OhlcAggregateFunction
            Inherits CustomAggregateFunction

            Public Overrides Function Calculate(ByVal groupInfo As GroupInfo) As Double()
                Dim open As Double = groupInfo.Values1.First()
                Dim close As Double = groupInfo.Values1.Last()
                Dim high As Double = [Double].MinValue
                Dim low As Double = [Double].MaxValue
                For Each value As Double In groupInfo.Values1
                    If high < value Then high = value
                    If low > value Then low = value
                Next

                Return New Double() {high, low, open, close}
            End Function
        End Class

#End Region  ' #CustomAggregateFunction
        Private Class DataPoint

            Public Property Argument As DateTime

            Public Property Value As Double
        End Class
    End Class
End Namespace
