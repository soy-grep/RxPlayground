﻿
Imports System.Reactive
Imports System.Reactive.Linq


Public Class FrmMain

    Private _delayedClickSubscription As IDisposable
    Private _randomClickErrorSubscription As IDisposable


    Private Sub FrmMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        DelayedClickInit()
        RandomClickErrorInit()

    End Sub


    Private Sub DelayedClickInit()

        ' vytvoríme stream z eventu Klik na tlačidlo, bude to stream časových pečiatok času kliku
        Dim delayedClickStream = Observable.FromEventPattern(Of EventArgs)(Me.BtnClick, "Click").
            Select(Of DateTime)(Function(args) DateTime.Now).  ' do streamu posielam presný čas kliku
            Delay(TimeSpan.FromSeconds(1)).                    ' v rámci streamu je event oneskorený o 1 sekundu
            ObserveOn(Me)                                      ' oznámenia sú posielané v event-loope prezentačnej vrstvy - teda tam kde bol vytvorený tento form

        _delayedClickSubscription = delayedClickStream.
            Subscribe(Sub(x) WriteLine($"Button Click at {x.ToString("HH:mm:ss.FFF")}", Color.Blue.ToArgb()))

    End Sub


    Private Sub RandomClickErrorInit()

        Dim randomClickErrorStream = Observable.FromEventPattern(Of EventArgs)(Me.BtnRandomExceptions, "Click").
            Do(Sub(x)
                   Dim rng = New Random()
                   If rng.Next(5) >= 4 Then
                       Throw New Exception("Boom!")
                   End If
               End Sub)

        _randomClickErrorSubscription = randomClickErrorStream.
            Subscribe(onNext:=Sub(x) WriteLine($"Click - No Error"),
                      onCompleted:=Sub() WriteLine("Completed"),
                      onError:=Sub(ex) WriteLine($"Click - Exception {ex.Message}", Color.Red.ToArgb()))

    End Sub


    Private Sub BtnUsnubscribe_Click(sender As Object, e As EventArgs) Handles BtnUsnubscribe.Click

        ' odhlásim sa z odberov eventov
        _delayedClickSubscription.Dispose()
        _randomClickErrorSubscription.Dispose()

    End Sub


    ''' <summary>
    ''' Zapísanie textu do jedného riadku
    ''' </summary>
    ''' <param name="text"></param>
    Private Sub WriteLine(text As String, Optional color As Integer = 0)
        TxtOutput.SelectionColor = System.Drawing.Color.FromArgb(color)
        TxtOutput.AppendText(text & System.Environment.NewLine)
    End Sub


End Class
