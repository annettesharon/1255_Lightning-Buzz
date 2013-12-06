Public Class SetBuzzerAnswer
    Public rightRowNo As Integer = 0
    Dim clsF As Integer = 0

    Private Sub SetBuzzerAnswer_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If clsF = 0 Then e.Cancel = True
    End Sub

    Private Sub SetBuzzerAnswer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        clsF = 0
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        clsF = 1
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub dg2_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dg2.CellClick
        If e.ColumnIndex = 3 Then
            If MsgBox("Are you sure to Set this Team as Right Answer" & vbCrLf & "Note: This will auto-calculate the score", MsgBoxStyle.YesNo, "Confirm?") = MsgBoxResult.Yes Then
                clsF = 1
                rightRowNo = e.RowIndex
                Me.DialogResult = Windows.Forms.DialogResult.OK
            End If
        End If
    End Sub
End Class