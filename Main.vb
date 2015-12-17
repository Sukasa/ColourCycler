Imports System.Windows.Forms

Module Main
    Dim Keyboard As GSeriesKeyboard
    Dim Controller As GSeriesController = GSeriesController.Instance()
    Dim Lum As Double = 64

    Function Main() As Integer
        Controller = GSeriesController.Instance()
        AddHandler Controller.DeviceChange, AddressOf DeviceChange
        DeviceChange()
        Dim Hue As Double = 0


        While True
            Hue += 0.5
            If Hue = 256 Then Hue = 0
            If Keyboard IsNot Nothing Then Keyboard.BacklightColour = HSL_to_RGB(Hue, 256, Math.Min(192, Lum))
            Do
                Threading.Thread.Sleep(75)
                If Lum > 64 Then
                    Lum = Math.Max(Lum - 3, 64)
                End If
                Controller.InputTick()
            Loop Until Keyboard IsNot Nothing AndAlso Keyboard.Connected = True
        End While
    End Function

    Private Sub KeyDown(Sender As Object, Params As Keys)
        Lum = 300
    End Sub

    Private Sub DeviceChange()
        Keyboard = Controller.Keyboards.Find(Function(T) T.GetType Is GetType(G510Keyboard))
        AddHandler Keyboard.RKeyDown, AddressOf KeyDown
    End Sub

    Private Function HSL_to_RGB(ByVal Hue As Double, ByVal Sat As Double, ByVal Lum As Double) As Color
        Dim r As Double = 0, g As Double = 0, b As Double = 0
        Dim temp1 As Double, temp2 As Double

        Hue /= 256
        Lum /= 256
        Sat /= 256

        If Lum = 0 Then
            r = 0
            g = 0
            b = 0
        Else
            If Sat = 0 Then
                r = Lum
                g = Lum
                b = Lum
            Else
                temp2 = (If((Lum <= 0.5), Lum * (1.0 + Sat), Lum + Sat - (Lum * Sat)))
                temp1 = 2.0 * Lum - temp2

                Dim t3 As Double() = New Double() {Hue + 1.0 / 3.0, Hue, Hue - 1.0 / 3.0}
                Dim clr As Double() = New Double() {0, 0, 0}
                For i As Integer = 0 To 2
                    If t3(i) < 0 Then
                        t3(i) += 1.0
                    End If
                    If t3(i) > 1 Then
                        t3(i) -= 1.0
                    End If

                    If 6.0 * t3(i) < 1.0 Then
                        clr(i) = temp1 + (temp2 - temp1) * t3(i) * 6.0
                    ElseIf 2.0 * t3(i) < 1.0 Then
                        clr(i) = temp2
                    ElseIf 3.0 * t3(i) < 2.0 Then
                        clr(i) = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3(i)) * 6.0)
                    Else
                        clr(i) = temp1
                    End If
                Next
                r = clr(0)
                g = clr(1)
                b = clr(2)
            End If
        End If

        Return Color.FromArgb(CInt(Math.Truncate(255 * r)), CInt(Math.Truncate(255 * g)), CInt(Math.Truncate(255 * b)))

    End Function

End Module
