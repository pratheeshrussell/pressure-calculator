Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
'Imports GL = OpenTK.Graphics.OpenGL4.GL
Public Class Form1
    'constants
    Dim tank_width As Double = 0
    Dim tank_depth As Double = 0
    Dim rho As Double = 0
    Dim elem_width As Double = 0
    Dim elem_height As Double = 0
    Dim grav As Double = 9.81 'm/s2
    'max_min value used for legends
    Dim max As Double = 0
    Dim min As Double = 0

    'other variables
    Dim values(,) As Double = Nothing

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        set_legends("start")
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        tank_width = Convert.ToDouble(TextBox2.Text)
        tank_depth = Convert.ToDouble(TextBox3.Text)
        rho = Convert.ToDouble(TextBox4.Text)
        elem_width = Convert.ToDouble(TextBox5.Text)
        elem_height = Convert.ToDouble(TextBox6.Text)
        grav = 9.81

        calculate_pressure()
        set_legends("display_res")
        display_result()
    End Sub
    Function calculate_pressure()
        Dim no_row = tank_width / elem_width
        Dim no_coloumns = tank_depth / elem_height
        ReDim values(no_coloumns, no_row)

        Dim pres As Double = 0
        For i As Integer = 0 To values.GetUpperBound(0) 'no_coloumns - 1
            For x As Integer = 0 To values.GetUpperBound(1) 'no_row - 1
                pres = (rho * grav * i * elem_height) '+ 101000
                If min = 0 Then
                    min = pres
                End If
                If pres < min Then
                    min = pres
                ElseIf pres > max Then
                    max = pres
                End If
                values(i, x) = pres
            Next
        Next
    End Function
    Function display_result()
        TextBox1.Text = ""
        Dim tex_string = ""

        For i As Integer = 1 To values.GetUpperBound(0) 'no_coloumns - 1
            tex_string = ""
            For x As Integer = 1 To values.GetUpperBound(1) 'no_row - 1
                tex_string += values(i, x) & ","
            Next
            TextBox1.Text += tex_string & vbCrLf
        Next
        ' MsgBox(min, 0, "min")
        GlControl1.Invalidate()
    End Function
    Private Sub GlControl1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles GlControl1.Paint
        'First Clear Buffers
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Clear(ClearBufferMask.DepthBufferBit)

        'Basic Setup for viewing
        Dim perspective As Matrix4 = Matrix4.CreateOrthographic(1.04, 4 / 3, 1, 10000) 'Setup Perspective
        Dim lookat As Matrix4 = Matrix4.LookAt(100, 20, 0, 0, 0, 0, 0, 1, 0) 'Setup camera
        GL.MatrixMode(MatrixMode.Projection) 'Load Perspective
        GL.LoadIdentity()
        GL.LoadMatrix(perspective)
        GL.MatrixMode(MatrixMode.Modelview) 'Load Camera
        GL.LoadIdentity()
        GL.LoadMatrix(lookat)
        GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height) 'Size of window
        GL.Enable(EnableCap.DepthTest) 'Enable correct Z Drawings
        GL.DepthFunc(DepthFunction.Less) 'Enable correct Z Drawings

        If rho = 0 Then
            GraphicsContext.CurrentContext.SwapInterval = True 'Caps frame rate as to not over run GPU
            GlControl1.SwapBuffers() 'Takes from the 'GL' and puts into control
            Exit Sub
        End If
            
        'Rotating
        GL.Rotate(90, 0, 0, 1)
        GL.Rotate(90, 0, 1, 0)
        GL.Rotate(90, 1, 0, 0)
        GL.Scale(scale1, scale1, scale1)

        GL.Translate(New Vector3(defx, 0, 0))
        GL.Translate(New Vector3(0, defy, 0))
        'Draw pyramid, Y is up, Z is twards you, X is left and right
        'Vertex goes (X,Y,Z)
        ' GL.Begin(BeginMode.Triangles)
        GL.Begin(PrimitiveType.Quads)
        'Face 1
        For i As Integer = 1 To values.GetUpperBound(0) 'no_coloumns - 1
            For x As Integer = 1 To values.GetUpperBound(1) 'no_row - 1
                GL.Color3(return_col(values(i, x)))
                GL.Vertex3(elem_width * (x - 1), -elem_height * (i - 1), 0)
                GL.Vertex3(elem_width * x, -elem_height * (i - 1), 0)
                GL.Vertex3(elem_width * x, -elem_height * i, 0)
                GL.Vertex3(elem_width * (x - 1), -elem_height * i, 0)
                ' Exit For '-->remove
            Next
            ' Exit For '-->remove
        Next
        GL.End()

        'Finally...
        GraphicsContext.CurrentContext.SwapInterval = True 'Caps frame rate as to not over run GPU
        GlControl1.SwapBuffers() 'Takes from the 'GL' and puts into control
    End Sub

    Function return_col(ByVal value As String) As Color
        Dim no = Convert.ToDouble(value)
        no = Math.Ceiling(no)
        Dim percent = Math.Ceiling(max / 100)

        ' MsgBox(percent)
        If no <= percent * 10 Then
            Return Color.Blue
        ElseIf no <= percent * 20 And no >= percent * 10 Then
            Return Color.BlueViolet
        ElseIf no <= percent * 30 And no >= percent * 20 Then
            Return Color.MediumVioletRed
        ElseIf no <= percent * 40 And no >= percent * 30 Then
            Return Color.Green
        ElseIf no <= percent * 50 And no >= percent * 40 Then
            Return Color.LimeGreen
        ElseIf no <= percent * 60 And no >= percent * 50 Then
            Return Color.LightGreen
        ElseIf no <= percent * 70 And no >= percent * 60 Then
            Return Color.GreenYellow
        ElseIf no <= percent * 80 And no >= percent * 70 Then
            Return Color.Yellow
        ElseIf no <= percent * 90 And no >= percent * 80 Then
            Return Color.Orange
        ElseIf no <= percent * 100 And no >= percent * 90 Then
            Return Color.Red
        End If
    End Function
    Function set_legends(ByVal atstart As String)
        If atstart.ToLower = "start" Then
            PictureBox1.BackColor = Color.White
            PictureBox2.BackColor = Color.White
            PictureBox3.BackColor = Color.White
            PictureBox4.BackColor = Color.White
            PictureBox5.BackColor = Color.White
            PictureBox6.BackColor = Color.White
            PictureBox7.BackColor = Color.White
            PictureBox8.BackColor = Color.White
            PictureBox9.BackColor = Color.White
            PictureBox10.BackColor = Color.White
            Label6.Text = ""
            Label7.Text = ""
            Label8.Text = ""
            Label9.Text = ""
            Label10.Text = ""
            Label11.Text = ""
            Label12.Text = ""
            Label13.Text = ""
            Label14.Text = ""
            Label15.Text = ""
            Label16.Text = ""
            Label18.Text = ""
            Label19.Text = ""
            '   Exit Function
        End If
        'set legend
        PictureBox1.BackColor = Color.Blue
        PictureBox2.BackColor = Color.BlueViolet
        PictureBox3.BackColor = Color.MediumVioletRed
        PictureBox4.BackColor = Color.Green
        PictureBox5.BackColor = Color.LimeGreen
        PictureBox6.BackColor = Color.LightGreen
        PictureBox7.BackColor = Color.GreenYellow
        PictureBox8.BackColor = Color.Yellow
        PictureBox9.BackColor = Color.Orange
        PictureBox10.BackColor = Color.Red
        Label6.Text = min
        Label7.Text = Math.Ceiling(max / 100) * 20
        Label8.Text = Math.Ceiling(max / 100) * 30
        Label9.Text = Math.Ceiling(max / 100) * 40
        Label10.Text = Math.Ceiling(max / 100) * 50
        Label11.Text = Math.Ceiling(max / 100) * 60
        Label12.Text = Math.Ceiling(max / 100) * 70
        Label13.Text = Math.Ceiling(max / 100) * 80
        Label14.Text = Math.Ceiling(max / 100) * 90
        Label15.Text = Math.Ceiling(max)
        Label18.Text = "max: " & max
        Label19.Text = "min: " & min
        Label16.Text = "Fluid Pressure only" & vbCrLf & "in N/m2."
    End Function




    'display controls
    Dim defx = 0
    Dim defy = 0
    Dim mousepos As Point
    Dim mouseinitpos As Point
    Private Sub GlControl1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GlControl1.Load
        GL.ClearColor(Color.White)
    End Sub
    Dim mouseupdown = 0
    Private Sub GlControl1_mdown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            mouseinitpos = New Point(e.X, e.Y)
            mouseupdown = 1
        End If
    End Sub
    Private Sub GlControl1_mup(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GlControl1.MouseUp
        mouseupdown = 0
    End Sub
    Private Sub GlControl1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseMove
        If mouseupdown = 0 Then
        Else
            defx = (mouseinitpos.X - e.X) * -0.001
            defy = (mouseinitpos.Y - e.Y) * 0.001
            ' mousepos = New Point(e.X, e.Y)
            GlControl1.Invalidate()
        End If
    End Sub

    Dim scale1 As Double = 1
    Private Sub GlControl1_scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles GlControl1.MouseWheel
        'zoom
        If e.Delta > 0 Then
            scale1 += 0.1
        ElseIf e.Delta < 0 And scale1 > 0.1 Then
            scale1 -= 0.1
        End If
        GlControl1.Invalidate()
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        defx = 0
        defy = 0
        scale1 = 1
        GlControl1.Invalidate()
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.MouseDown
        defy += 0.001
        GlControl1.Invalidate()
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.MouseDown
        defy -= 0.001
        GlControl1.Invalidate()
    End Sub
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.MouseDown
        defx += 0.001
        GlControl1.Invalidate()
    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.MouseDown
        defx -= 0.001
        GlControl1.Invalidate()
    End Sub
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        scale1 += 0.1
        GlControl1.Invalidate()
    End Sub
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        scale1 -= 0.1
        GlControl1.Invalidate()
    End Sub
End Class
