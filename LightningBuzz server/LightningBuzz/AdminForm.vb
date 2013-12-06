Imports System.IO

Public Class AdminForm
    Dim connStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Application.StartupPath & "\QuizDB.accdb;Persist Security Info=False;"
    Dim viewF As New ViewerForm
    Dim setbF As New SetBuzzerAnswer

    Dim r As New Random
    Dim dictRow As New Dictionary(Of Integer, DataGridViewRow)
    Dim waitingFlg As Boolean = False     'Waiting for user option
    Dim currenttime As Integer = 0

    'The main socket on which the server listens to the clients
    Private serverSocket As Net.Sockets.Socket
    Private byteData As Byte() = New Byte(2047) {}

    Dim currentQn As Integer = 0

    Private Sub AdminForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
    End Sub

    Private Sub AdminForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadDefaults()
        loadTeams()
        ComboBox1.SelectedIndex = 0
        ListenFromUser()
    End Sub

    Private Sub loadDefaults()
        ComboBox3.SelectedIndex = 3
        ComboBox9.SelectedIndex = 3

        ComboBox4.SelectedIndex = 0
        ComboBox5.SelectedIndex = 0

        ComboBox8.SelectedIndex = 1
        ComboBox6.SelectedIndex = 1
    End Sub

    Private Sub loadTeams()
        For i = 0 To 9
            dg1.Rows.Add(i + 1, "Team " & Convert.ToChar(65 + i), Rand, 0)
            dictRow.Add(i + 1, dg1.Rows(dg1.RowCount - 1))
            dg1.Rows(dg1.RowCount - 1).DefaultCellStyle.BackColor = Color.LightYellow
        Next
    End Sub

    Private Function Rand() As Integer
        Return r.Next(50000, 100000)
    End Function

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            dg1.Columns(2).Visible = True
            dg1.Columns(4).Visible = True
            TextBox4.Visible = True
        Else
            dg1.Columns(2).Visible = False
            dg1.Columns(4).Visible = False
            TextBox4.Visible = False
        End If
    End Sub

    Private Sub dg1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dg1.CellClick
        If e.RowIndex < 0 Or e.ColumnIndex < 0 Then Exit Sub
        If e.ColumnIndex = 4 Then
            If MsgBox("Are you sure to Reset the Connected Key?" & vbCrLf & "It may disconnect the previously connected node.", MsgBoxStyle.YesNo, "Are you sure?") = MsgBoxResult.Yes Then
                dg1.Item(2, e.RowIndex).Value = Rand()
                dg1.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightYellow
            End If
        ElseIf e.ColumnIndex = 5 Then
            dg1.Rows(e.RowIndex).Cells(3).Value += 5
        ElseIf e.ColumnIndex = 6 Then
            dg1.Rows(e.RowIndex).Cells(3).Value -= 5
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 0 Then
            Panel1.Visible = True
            Panel3.Visible = False
            viewF.dg2.Columns(3).Visible = True
            viewF.dg2.Columns(4).Visible = True
            viewF.dg2.Columns(5).Visible = True
            viewF.dg2.Columns(6).Visible = True
        ElseIf ComboBox1.SelectedIndex = 1 Then
            Panel1.Visible = False
            Panel3.Visible = True
            viewF.dg2.Columns(3).Visible = False
            viewF.dg2.Columns(4).Visible = False
            viewF.dg2.Columns(5).Visible = False
            viewF.dg2.Columns(6).Visible = False
        End If
    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            Panel2.Visible = True
            Panel4.Visible = False
        Else
            Panel2.Visible = False
            Panel4.Visible = True
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        LoadQn("N")
    End Sub

    Private Sub ResetUsersOptions()
        dg2.Rows.Clear()
        waitingFlg = False
    End Sub

    Private Sub ListenFromUser()
        Try
            CheckForIllegalCrossThreadCalls = False

            'We are using UDP sockets
            serverSocket = New Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Dgram, Net.Sockets.ProtocolType.Udp)

            'Assign the any IP of the machine and listen on port number 5000
            Dim ipEndPoint As New Net.IPEndPoint(Net.IPAddress.Any, 5000)

            'Bind this address to the server
            serverSocket.Bind(ipEndPoint)

            Dim ipeSender As New Net.IPEndPoint(Net.IPAddress.Any, 0)
            'The epSender identifies the incoming clients
            Dim epSender As Net.EndPoint = DirectCast(ipeSender, Net.EndPoint)

            'Start receiving data
            serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, Net.Sockets.SocketFlags.None, epSender, New AsyncCallback(AddressOf OnReceive), _
                epSender)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "UDP Problem", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try
    End Sub



    Private Sub OnReceive(ar As IAsyncResult)
        Try
            Dim ipeSender As New Net.IPEndPoint(Net.IPAddress.Any, 0)
            Dim epSender As Net.EndPoint = DirectCast(ipeSender, Net.EndPoint)

            serverSocket.EndReceiveFrom(ar, epSender)

            '  MessageBox.Show(System.Text.Encoding.UTF8.GetString(byteData));
            ' return;

            'Transform the array of bytes received from the user into an
            'intelligent form of object Data
            'Dim msgReceived As New Data(byteData)
            Dim msgReceived As String = System.Text.Encoding.UTF8.GetString(byteData)
            TextBox4.Text &= msgReceived & vbCrLf

            Dim decodestr() As String = msgReceived.Split("|")

            'command,teamid,key,ans
            If decodestr.Count >= 4 Then

                If Val(decodestr(0)) = 1 Then   'Connection Request
                    If checkUserWithKey(Val(decodestr(1)), Val(decodestr(2))) Then
                        dictRow(Val(decodestr(1))).DefaultCellStyle.BackColor = Color.LightGreen
                    Else
                        'Invalid User Request. Skip
                    End If
                ElseIf Val(decodestr(0)) = 2 Then   'Round 1, 4option round
                    If waitingFlg And ComboBox1.SelectedIndex = 0 Then
                        If checkUserWithKey(Val(decodestr(1)), Val(decodestr(2))) Then
                            Dim availableFlg As Boolean = False
                            For i = 0 To dg2.RowCount - 1
                                If dg2.Item(0, i).Value = Val(decodestr(1)) Then
                                    availableFlg = True
                                    Exit For
                                End If
                            Next
                            If availableFlg = False Then
                                dg2.Rows.Add(dictRow(Val(decodestr(1))).Cells(0).Value, dictRow(Val(decodestr(1))).Cells(1).Value, dg2.RowCount + 1, Val(decodestr(3)))
                                viewF.dg2.Rows.Add(dictRow(Val(decodestr(1))).Cells(0).Value, dg2.RowCount, dictRow(Val(decodestr(1))).Cells(1).Value)
                            End If

                            'Received All Options
                            If dg2.RowCount >= 10 Then
                                waitingFlg = False
                                CheckBox2.Enabled = True
                                CheckBox4.Enabled = True
                            End If
                        Else
                            'Invalid User Request. Skip
                        End If
                    End If
                ElseIf Val(decodestr(0)) = 3 Then   'Round 2, Buzzer round
                    If waitingFlg And ComboBox1.SelectedIndex = 1 Then
                        If checkUserWithKey(Val(decodestr(1)), Val(decodestr(2))) Then
                            Dim availableFlg As Boolean = False
                            For i = 0 To dg2.RowCount - 1
                                If dg2.Item(0, i).Value = Val(decodestr(1)) Then
                                    availableFlg = True
                                    Exit For
                                End If
                            Next
                            If availableFlg = False Then
                                dg2.Rows.Add(dictRow(Val(decodestr(1))).Cells(0).Value, dictRow(Val(decodestr(1))).Cells(1).Value, dg2.RowCount + 1, "")
                                viewF.dg2.Rows.Add(dictRow(Val(decodestr(1))).Cells(0).Value, dg2.RowCount, dictRow(Val(decodestr(1))).Cells(1).Value)
                            End If

                            'Received All Options
                            If dg2.RowCount >= 10 Then
                                waitingFlg = False
                            End If
                        Else
                            'Invalid User Request. Skip
                        End If
                    End If
                End If
            End If

            'Start listening to the message sent by the user
            serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, Net.Sockets.SocketFlags.None, epSender, New AsyncCallback(AddressOf OnReceive), _
                epSender)
            viewF.dg2.ClearSelection()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try
    End Sub

    Private Function checkUserWithKey(ByVal userid As Integer, key As Integer) As Boolean
        If dictRow.ContainsKey(userid) Then
            If dictRow(userid).Cells(2).Value = key Then Return True Else Return False
        End If
        Return False
    End Function

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If IsWaiting() = True Then Exit Sub
        If ComboBox1.SelectedIndex = 0 Then
            viewF.Label1.BackColor = Color.White
            viewF.Label4.BackColor = Color.White
            viewF.Label6.BackColor = Color.White
            viewF.Label7.BackColor = Color.White
            viewF.Label1.Visible = True
            viewF.Label4.Visible = True
            viewF.Label6.Visible = True
            viewF.Label7.Visible = True

            currenttime = ComboBox3.Text
            waitingFlg = True
            RoundTimer.Enabled = True
            CheckBox2.Enabled = False
            Label14.Text = " Time Remaining : " & currenttime & " sec"
            viewF.Label2.Text = currenttime
        End If
    End Sub

    Private Function IsWaiting() As Boolean
        If waitingFlg Then
            MsgBox("You cannot proceed while the system waits for user option. Please stop and then continue.")
            Return True
        End If
        Return False
    End Function

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        waitingFlg = False
        RoundTimer.Enabled = False
        CheckBox2.Enabled = True
    End Sub

    Private Sub RoundTimer_Tick(sender As Object, e As EventArgs) Handles RoundTimer.Tick
        currenttime -= 1
        Label14.Text = " Time Remaining : " & currenttime & " sec"
        viewF.Label2.Text = currenttime
        If currenttime <= 0 Then
            waitingFlg = False
            Label14.Text = " Time Remaining : " & 0 & " sec"
            viewF.Label2.Text = "00"
            RoundTimer.Enabled = False
            CheckBox2.Enabled = True
            CheckBox4.Enabled = True
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBox2.CheckedChanged

        If CheckBox2.Checked = True Then
            GroupBox1.Enabled = True
            DisplayQn(True)
        Else
            GroupBox1.Enabled = False
            DisplayQn(False)
        End If
    End Sub

    Private Sub DisplayQn(ByVal show As Boolean)
        If ComboBox1.SelectedIndex = 0 And RadioButton1.Checked Then    '4option rounds - qn round
            If ComboBox2.SelectedIndex < 0 Then Exit Sub
            viewF.Panel1.Visible = show
            viewF.Panel2.Visible = False
            viewF.ScorePanel.Visible = False
            viewF.PictureBox2.Visible = False
        ElseIf ComboBox1.SelectedIndex = 0 And RadioButton2.Checked Then    '4option rounds - image round
            If ComboBox7.SelectedIndex < 0 Then Exit Sub
            viewF.Panel1.Visible = False
            viewF.Panel2.Visible = show
            viewF.ScorePanel.Visible = False
        ElseIf ComboBox1.SelectedIndex = 1 Then                          'buzzer round 
            If ComboBox10.SelectedIndex < 0 Then Exit Sub
            viewF.Panel1.Visible = show
            viewF.Panel2.Visible = False
            viewF.ScorePanel.Visible = False
            viewF.PictureBox2.Visible = True
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked = True Then
            GroupBox2.Enabled = True
            DisplayQn(True)
        Else
            GroupBox2.Enabled = False
            DisplayQn(False)
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim myScreen = Screen.FromControl(Me)
        Dim otherScreen = If(Screen.AllScreens.FirstOrDefault(Function(s) s IsNot myScreen), myScreen)
        viewF.Left = otherScreen.WorkingArea.Left + 120
        viewF.Top = otherScreen.WorkingArea.Top + 120
        viewF.Visible = Not viewF.Visible
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox2.SelectedIndex < 0 Then Exit Sub
        arrDG.DataSource = Nothing
        Dim conn As New OleDb.OleDbConnection(connStr)
        Dim comm As New OleDb.OleDbCommand("", conn)
        conn.Open()
        If ComboBox2.SelectedIndex = 0 Then
            comm.CommandText = "select Qno, Qn, Option1, Option2, Option3, Option4, Answer, ImgLoc from Questions where Category=10 order by qno"
        ElseIf ComboBox2.SelectedIndex = 1 Then
            comm.CommandText = "select Qno, Qn, Option1, Option2, Option3, Option4, Answer, ImgLoc from Questions where Category=12 order by qno"
        End If
        Dim adap As New OleDb.OleDbDataAdapter(comm)
        Dim dt As New DataTable
        adap.Fill(dt)
        arrDG.DataSource = dt
        conn.Dispose()
        If arrDG.RowCount > 0 Then currentQn = 0
        LoadQn("N")
    End Sub

    Private Sub LoadQn(ByVal mode As Char)
        If IsWaiting() = True Then
            MsgBox("Please Stop Waiting for the User Options and then try again", MsgBoxStyle.OkOnly, "Cannot Proceed")
            Exit Sub
        End If

        dg2.Rows.Clear()
        viewF.dg2.Rows.Clear()
        If arrDG.RowCount > 0 Then
            If ComboBox1.SelectedIndex = 0 And RadioButton1.Checked Then    '4option rounds - qn round
                If ComboBox2.SelectedIndex < 0 Then Exit Sub
                viewF.Label1.BackColor = Color.White
                viewF.Label4.BackColor = Color.White
                viewF.Label6.BackColor = Color.White
                viewF.Label7.BackColor = Color.White
                viewF.Label1.Visible = False
                viewF.Label4.Visible = False
                viewF.Label6.Visible = False
                viewF.Label7.Visible = False

                If mode = "N" Then
                    If arrDG.RowCount >= currentQn + 1 Then currentQn += 1
                ElseIf mode = "P" Then
                    If currentQn - 1 >= 1 Then currentQn -= 1
                ElseIf mode = "F" Then
                    currentQn = 1
                ElseIf mode = "L" Then
                    currentQn = arrDG.RowCount
                End If
                viewF.Label2.Text = "00"
                Label15.Text = currentQn & "/" & arrDG.RowCount
                viewF.Label8.Text = currentQn & "/" & arrDG.RowCount
                TextBox1.Text = arrDG.Item(1, currentQn - 1).Value & vbCrLf & "1) " & arrDG.Item(2, currentQn - 1).Value & vbCrLf & "2) " & arrDG.Item(3, currentQn - 1).Value & vbCrLf & "3) " & arrDG.Item(4, currentQn - 1).Value & vbCrLf & "4) " & arrDG.Item(5, currentQn - 1).Value
                viewF.Label3.Text = arrDG.Item(1, currentQn - 1).Value
                viewF.Label1.Text = "1) " & arrDG.Item(2, currentQn - 1).Value
                viewF.Label4.Text = "2) " & arrDG.Item(3, currentQn - 1).Value
                viewF.Label6.Text = "3) " & arrDG.Item(4, currentQn - 1).Value
                viewF.Label7.Text = "4) " & arrDG.Item(5, currentQn - 1).Value

            ElseIf ComboBox1.SelectedIndex = 0 And RadioButton2.Checked Then    '4option rounds - image round
                If ComboBox7.SelectedIndex < 0 Then Exit Sub
                viewF.Label13.BackColor = Color.White
                viewF.Label12.BackColor = Color.White
                viewF.Label11.BackColor = Color.White
                viewF.Label10.BackColor = Color.White
                viewF.Label13.Visible = False
                viewF.Label12.Visible = False
                viewF.Label11.Visible = False
                viewF.Label10.Visible = False

                If mode = "N" Then
                    If arrDG.RowCount >= currentQn + 1 Then currentQn += 1
                ElseIf mode = "P" Then
                    If currentQn - 1 >= 1 Then currentQn -= 1
                ElseIf mode = "F" Then
                    currentQn = 1
                ElseIf mode = "L" Then
                    currentQn = arrDG.RowCount
                End If
                Label16.Text = currentQn & "/" & arrDG.RowCount
                viewF.Label9.Text = currentQn & "/" & arrDG.RowCount
                TextBox2.Text = arrDG.Item(1, currentQn - 1).Value & vbCrLf & "1) " & arrDG.Item(2, currentQn - 1).Value & vbCrLf & "2) " & arrDG.Item(3, currentQn - 1).Value & vbCrLf & "3) " & arrDG.Item(4, currentQn - 1).Value & vbCrLf & "4) " & arrDG.Item(5, currentQn - 1).Value
                PictureBox1.Image = Nothing
                viewF.PictureBox1.Image = Nothing
                If IsDBNull(arrDG.Item(7, currentQn - 1).Value) = False Then
                    If Trim(arrDG.Item(7, currentQn - 1).Value) <> "" Then
                        If File.Exists(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value) Then
                            If Trim(arrDG.Item(7, currentQn - 1).Value).EndsWith("mp3") Then
                                Process.Start(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value)
                            Else
                                PictureBox1.Image = Image.FromFile(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value)
                                viewF.PictureBox1.Image = Image.FromFile(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value)
                            End If
                        End If
                    End If
                End If
                viewF.Label14.Text = arrDG.Item(1, currentQn - 1).Value
                viewF.Label13.Text = "1) " & arrDG.Item(2, currentQn - 1).Value
                viewF.Label12.Text = "2) " & arrDG.Item(3, currentQn - 1).Value
                viewF.Label11.Text = "3) " & arrDG.Item(4, currentQn - 1).Value
                viewF.Label10.Text = "4) " & arrDG.Item(5, currentQn - 1).Value
                viewF.Label2.Text = "00"

            ElseIf ComboBox1.SelectedIndex = 1 Then    'buzzer round 
                If ComboBox10.SelectedIndex < 0 Then Exit Sub
                viewF.Label1.Visible = False
                viewF.Label4.Visible = False
                viewF.Label6.Visible = False
                viewF.Label7.Visible = False

                If mode = "N" Then
                    If arrDG.RowCount >= currentQn + 1 Then currentQn += 1
                ElseIf mode = "P" Then
                    If currentQn - 1 >= 1 Then currentQn -= 1
                ElseIf mode = "F" Then
                    currentQn = 1
                ElseIf mode = "L" Then
                    currentQn = arrDG.RowCount
                End If

                Label17.Text = currentQn & "/" & arrDG.RowCount
                TextBox3.Text = arrDG.Item(1, currentQn - 1).Value
                viewF.Label8.Text = currentQn & "/" & arrDG.RowCount
                viewF.Label3.Text = arrDG.Item(1, currentQn - 1).Value
                viewF.Label2.Text = "00"

                PictureBox2.Image = Nothing
                viewF.PictureBox2.Image = Nothing
                If IsDBNull(arrDG.Item(7, currentQn - 1).Value) = False Then
                    If Trim(arrDG.Item(7, currentQn - 1).Value) <> "" Then
                        If File.Exists(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value) Then
                                PictureBox2.Image = Image.FromFile(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value)
                            viewF.PictureBox2.Image = Image.FromFile(Application.StartupPath & "\" & arrDG.Item(7, currentQn - 1).Value)
                            End If
                    End If
                End If
            End If
        Else
            MsgBox("Please load the questions first")
            End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        LoadQn("P")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        LoadQn("F")
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        LoadQn("L")
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        If ComboBox7.SelectedIndex < 0 Then Exit Sub
        arrDG.DataSource = Nothing
        Dim conn As New OleDb.OleDbConnection(connStr)
        Dim comm As New OleDb.OleDbCommand("", conn)
        conn.Open()
        Dim cat As Integer = 0
        If ComboBox7.SelectedIndex = 0 Then     'General
            cat = 1
        ElseIf ComboBox7.SelectedIndex = 1 Then 'Logo
            cat = 2
        ElseIf ComboBox7.SelectedIndex = 2 Then 'Personality
            cat = 3
        ElseIf ComboBox7.SelectedIndex = 3 Then 'Motto
            cat = 4
        ElseIf ComboBox7.SelectedIndex = 4 Then 'Award
            cat = 5
        ElseIf ComboBox7.SelectedIndex = 5 Then 'Audio
            cat = 6
        End If
        If cat = 0 Then Exit Sub
        comm.CommandText = "select Qno, Qn, Option1, Option2, Option3, Option4, Answer, ImgLoc from Questions where Category=" & cat & " order by qno"
        Dim adap As New OleDb.OleDbDataAdapter(comm)
        Dim dt As New DataTable
        adap.Fill(dt)
        arrDG.DataSource = dt
        conn.Dispose()
        If arrDG.RowCount > 0 Then currentQn = 0
        LoadQn("N")
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        LoadQn("N")
    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        LoadQn("P")
    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        LoadQn("F")
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        LoadQn("L")
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        If ComboBox10.SelectedIndex < 0 Then Exit Sub
        arrDG.DataSource = Nothing
        Dim conn As New OleDb.OleDbConnection(connStr)
        Dim comm As New OleDb.OleDbCommand("", conn)
        conn.Open()
        If ComboBox10.SelectedIndex = 0 Then
            comm.CommandText = "select Qno, Qn, Option1, Option2, Option3, Option4, Answer, ImgLoc from Questions where Category=7 order by qno"
        ElseIf ComboBox10.SelectedIndex = 1 Then
            comm.CommandText = "select Qno, Qn, Option1, Option2, Option3, Option4, Answer, ImgLoc from Questions where Category=8 order by qno"
        End If
        Dim adap As New OleDb.OleDbDataAdapter(comm)
        Dim dt As New DataTable
        adap.Fill(dt)
        arrDG.DataSource = dt
        conn.Dispose()
        If arrDG.RowCount > 0 Then currentQn = 0
        LoadQn("N")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        LoadQn("N")
    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        LoadQn("P")
    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        LoadQn("F")
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        LoadQn("L")
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        showAns()
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        If IsWaiting() = True Then Exit Sub
        If ComboBox1.SelectedIndex = 0 Then
            viewF.Label13.BackColor = Color.White
            viewF.Label12.BackColor = Color.White
            viewF.Label11.BackColor = Color.White
            viewF.Label10.BackColor = Color.White

            viewF.Label13.Visible = True
            viewF.Label12.Visible = True
            viewF.Label11.Visible = True
            viewF.Label10.Visible = True

            currenttime = ComboBox3.Text
            waitingFlg = True
            RoundTimer.Enabled = True
            CheckBox4.Enabled = False
            Label14.Text = " Time Remaining : " & currenttime & " sec"
            viewF.Label2.Text = currenttime
        End If
    End Sub

    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        waitingFlg = False
        RoundTimer.Enabled = False
        CheckBox4.Enabled = True
    End Sub

    Private Sub CheckBox3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            GroupBox3.Enabled = True
            DisplayQn(True)
        Else
            GroupBox3.Enabled = False
            DisplayQn(False)
        End If
    End Sub

    Private Sub Button27_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button27.Click
        showAns()
    End Sub

    Private Sub showAns()
        If IsWaiting() = True Then
            MsgBox("You cannot Show the Answer." & vbCrLf & vbCrLf & "The system is still listening for user options. Please Stop and then Try again.")
            Exit Sub
        End If
        If arrDG.RowCount > 0 Then
            If ComboBox1.SelectedIndex = 0 Then    '4option rounds 
                Dim actualAns As Integer = Val(arrDG.Item(6, currentQn - 1).Value)
                If RadioButton1.Checked Then
                    If actualAns = 1 Then viewF.Label1.BackColor = Color.LightGreen
                    If actualAns = 2 Then viewF.Label4.BackColor = Color.LightGreen
                    If actualAns = 3 Then viewF.Label6.BackColor = Color.LightGreen
                    If actualAns = 4 Then viewF.Label7.BackColor = Color.LightGreen
                ElseIf RadioButton2.Checked Then
                    If actualAns = 1 Then viewF.Label13.BackColor = Color.LightGreen
                    If actualAns = 2 Then viewF.Label12.BackColor = Color.LightGreen
                    If actualAns = 3 Then viewF.Label11.BackColor = Color.LightGreen
                    If actualAns = 4 Then viewF.Label10.BackColor = Color.LightGreen
                End If
                

                Dim uopt As Integer = 0
                Dim firstans As Boolean = False
                For i = 0 To dg2.RowCount - 1
                    uopt = Val(dg2.Item(3, i).Value)
                    viewF.dg2.Rows(i).Cells(uopt + 2).Style.BackColor = If(uopt = actualAns, Color.LightGreen, Color.LightPink)
                    If firstans = False Then
                        If uopt = actualAns Then
                            firstans = True
                            viewF.dg2.Rows(i).Cells(1).Style.BackColor = Color.LightGreen
                            viewF.dg2.Rows(i).Cells(2).Style.BackColor = Color.LightGreen
                            dictRow(Val(dg2.Item(0, i).Value)).Cells(3).Value += Val(ComboBox4.Text)    'Adds Score
                        Else
                            GoTo wrongoption
                        End If
                    Else
wrongoption:
                        If uopt <> actualAns Then       'Reduce Score only to wrong ans pressed
                            viewF.dg2.Rows(i).Cells(1).Style.BackColor = Color.LightPink
                            viewF.dg2.Rows(i).Cells(2).Style.BackColor = Color.LightPink
                            dictRow(Val(dg2.Item(0, i).Value)).Cells(3).Value -= Val(ComboBox5.Text)    'Reduces Score - Negative
                        Else
                            viewF.dg2.Rows(i).Cells(1).Style.BackColor = Color.LightYellow
                            viewF.dg2.Rows(i).Cells(2).Style.BackColor = Color.LightYellow
                        End If
                    End If
                Next

            ElseIf ComboBox1.SelectedIndex = 1 Then    'buzzer round 
                setbF.dg2.Rows.Clear()
                For i = 0 To dg2.RowCount - 1
                    setbF.dg2.Rows.Add(dg2.Item(0, i).Value, dg2.Item(1, i).Value, dg2.Item(2, i).Value)
                Next
                If setbF.ShowDialog = Windows.Forms.DialogResult.OK Then
                    'Calculate Score
                    For i = 0 To dg2.RowCount - 1
                        If i < setbF.rightRowNo Then    'Set Negative
                            viewF.dg2.Rows(i).Cells(1).Style.BackColor = Color.LightPink
                            viewF.dg2.Rows(i).Cells(2).Style.BackColor = Color.LightPink
                            dictRow(Val(dg2.Item(0, i).Value)).Cells(3).Value -= Val(ComboBox6.Text)    'Reduce Score
                        ElseIf i = setbF.rightRowNo Then    'Set Score
                            viewF.dg2.Rows(i).Cells(1).Style.BackColor = Color.LightGreen
                            viewF.dg2.Rows(i).Cells(2).Style.BackColor = Color.LightGreen
                            dictRow(Val(dg2.Item(0, i).Value)).Cells(3).Value += Val(ComboBox8.Text)    'Adds Score
                        Else
                            viewF.dg2.Rows(i).Cells(1).Style.BackColor = Color.LightYellow
                            viewF.dg2.Rows(i).Cells(2).Style.BackColor = Color.LightYellow
                        End If
                    Next
                End If
            End If
        End If
        viewF.dg2.ClearSelection()
    End Sub

    Private Sub Button25_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button25.Click
        If IsWaiting() = True Then Exit Sub
        If ComboBox1.SelectedIndex = 1 Then
            currenttime = ComboBox9.Text
            waitingFlg = True
            RoundTimer.Enabled = True
            CheckBox3.Enabled = False
            Label14.Text = " Time Remaining : " & currenttime & " sec"
            viewF.Label2.Text = currenttime
        End If
    End Sub

    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        waitingFlg = False
        RoundTimer.Enabled = False
        CheckBox3.Enabled = True
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        dg1.Columns(5).Visible = CheckBox5.Checked
        dg1.Columns(6).Visible = CheckBox5.Checked
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If viewF.ScorePanel.Visible = False Then
            viewF.dgScore.Rows.Clear()
            For i = 0 To dg1.RowCount - 1
                viewF.dgScore.Rows.Add(dg1.Item(0, i).Value, dg1.Item(1, i).Value, dg1.Item(3, i).Value)
            Next
            viewF.dgScore.Sort(viewF.dgScore.Columns(2), System.ComponentModel.ListSortDirection.Descending)
            viewF.dgScore.ClearSelection()
        End If
        viewF.ScorePanel.Visible = Not viewF.ScorePanel.Visible
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        showAns()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        viewF.Panel1.Visible = False
        viewF.ScorePanel.Visible = False
        viewF.Panel2.Visible = False
        viewF.dg2.Rows.Clear()
        viewF.Label2.Text = "00"
    End Sub

    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        If MsgBox("Are you Sure to Exit?" & vbCrLf & vbCrLf & "Note: All the Settings will be reset when you exit the application", MsgBoxStyle.YesNo, "Really want to exit?") = MsgBoxResult.Yes Then
            Me.Dispose()
        End If
    End Sub
End Class
