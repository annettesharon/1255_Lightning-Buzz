Public Class ViewerForm

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Label8.Text = ""
        Label3.Text = ""
        Label1.Text = ""
        Label4.Text = ""
        Label6.Text = ""
        Label7.Text = ""
        Label9.Text = ""
        Label14.Text = ""
        Label13.Text = ""
        Label12.Text = ""
        Label11.Text = ""
        Label10.Text = ""
        PictureBox1.Image = Nothing
    End Sub

    Private Sub ViewerForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
    End Sub

End Class