﻿Imports System.IO
Imports System.Collections
Imports System.Drawing.Imaging

Public Structure PropertiesCustom
    Public Input1 As String
    Public Input2 As String
    Public Output As String
    Public CustomRunID As String

    Public Sub SetDefaultvalues()
        Me.Input1 = Directory.GetCurrentDirectory()
        Me.Input2 = Directory.GetCurrentDirectory()
        Me.Output = Directory.GetCurrentDirectory()
        Me.CustomRunID = ""
    End Sub

End Structure