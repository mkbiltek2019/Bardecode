﻿Imports System.IO

Public Class StepBardecode
    Implements IStep

    Private BardecodeProcessInfo As ProcessStartInfo
    Public BardecodeProperties As PropertiesBardecode

    Public ReadOnly Property Type As Project.StepType Implements IStep.Type
        Get
            Return Project.StepType.Bardecode
        End Get
    End Property

    Sub New()
        Me.BardecodeProperties = New PropertiesBardecode
        Me.BardecodeProperties.SetDefaultvalues()
    End Sub

    Sub New(Properties As PropertiesBardecode)
        Me.BardecodeProperties = Properties
        If Me.BardecodeProperties.FolderType = 0 Then
            Me.BardecodeProperties.InputFolder = Directory.GetCurrentDirectory() + "\Processing\Documents"
            Me.BardecodeProperties.CreateOutputSubFolders = True
        Else
            Me.BardecodeProperties.InputFolder = Directory.GetCurrentDirectory() + "\Processing\Drawings"
            Me.BardecodeProperties.CreateOutputSubFolders = False
        End If
        Me.BardecodeProperties.OutputFolder = Me.BardecodeProperties.InputFolder
        Me.BardecodeProperties.DeleteInputFiles = True
        Me.BardecodeProperties.ProcessSubFolders = True
    End Sub

    Public Sub StartBardecodeProcess()
        Dim PInfo = New ProcessStartInfo
        PInfo.FileName = "C:\Program Files (x86)\Softek Software\BardecodeFiler\BardecodeFiler.exe"
        PInfo.Arguments = """" + Directory.GetCurrentDirectory() + "\BardecodeIni.ini"""
        PInfo.UseShellExecute = False
        PInfo.Verb = "runas"
        PInfo.RedirectStandardOutput = True

        With System.Diagnostics.Process.Start(PInfo)
            .PriorityClass = ProcessPriorityClass.High
            .WaitForExit()
            .Close()
            .Dispose()
        End With
    End Sub

    Public Sub SetBardecodeProperties()
        ' on this sub i will have to pick everything that i have on that class of properties
        ' and pass it to the inifile on the same directory were bardecode is placed
        ' will have to modify the inifiles each time that i run bardecode passing just the subfolders and not the whole input folder of the boxes
        ' check the type of the folder that are being processed and create the IniFile object according with it
        Dim BardecodeIni As IniFile = New IniFile(Directory.GetCurrentDirectory() + "\BardecodeIni.ini")

        'Folders and files
        BardecodeIni.WriteValue("options", "inputFolder", "System.String," + Me.BardecodeProperties.InputFolder)
        BardecodeIni.WriteValue("options", "outputFolder", "System.String," + Me.BardecodeProperties.OutputFolder)
        BardecodeIni.WriteValue("options", "processedFolder", "System.String," + Me.BardecodeProperties.ProcessedFolder)
        BardecodeIni.WriteValue("options", "exceptionFolder", "System.String," + Me.BardecodeProperties.ExceptionFolder)
        BardecodeIni.WriteValue("options", "outputTemplate", "System.String," + Me.BardecodeProperties.OutputNameTemplate)
        BardecodeIni.WriteValue("options", "FilePattern", "System.String," + Me.BardecodeProperties.FileNamePattern)
        BardecodeIni.WriteValue("options", "ProcessSubFolders", "System.Boolean," + "False")
        BardecodeIni.WriteValue("options", "SubFolderPattern", "System.String," + "")

        'Barcode Types
        BardecodeIni.WriteValue("options", "Codabar", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.Codabar).ToString())
        BardecodeIni.WriteValue("options", "Code 128", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.Code_128).ToString())
        BardecodeIni.WriteValue("options", "Code 2 of 5 (interleaved)", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.Code_2_of_5).ToString())
        BardecodeIni.WriteValue("options", "Code 2 of 5 (other)", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.Code_2_of_5_other).ToString())
        BardecodeIni.WriteValue("options", "Code 3 of 9", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.Code_3_of_9).ToString())
        BardecodeIni.WriteValue("options", "PDF-417", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.PDF_417).ToString())
        BardecodeIni.WriteValue("options", "EAN-13 and UPC-A", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.EAN_13_UPC_A).ToString())
        BardecodeIni.WriteValue("options", "EAN-8", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.EAN_8).ToString())
        BardecodeIni.WriteValue("options", "UPC-E", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.UPC_E).ToString())
        BardecodeIni.WriteValue("options", "Datamatrix", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.Datamatrix).ToString())
        BardecodeIni.WriteValue("options", "QR-Code", "System.Boolean," + Me.BardecodeProperties.BarcodeTypes.Contains(BarcodeType.QR_Code).ToString())

        'Barcode Pattern, Delete input files, Split Mode, Move to processed.
        BardecodeIni.WriteValue("options", "Pattern", "System.String," + Me.BardecodeProperties.BarcodePattern)
        BardecodeIni.WriteValue("options", "DeleteInputImages", "System.Boolean," + True.ToString())
        BardecodeIni.WriteValue("options", "moveToProcessedFolder", "System.Boolean," + False.ToString())
        BardecodeIni.WriteValue("options", "splitMode", "System.Int32," + Me.BardecodeProperties.SplitMode.ToString())
    End Sub

    Public Sub ChangePath(Path As String)
        ' this method will be invoked just through the loop to change the input folder of the bardecode inifile configuration
        ' we need to process just one folder at time so thats the reason why we need to change the input folder on the inifile on the go

        Dim BardecodeIni As IniFile = New IniFile(Directory.GetCurrentDirectory() + "\BardecodeIni.ini")
        BardecodeIni.WriteValue("options", "inputFolder", "System.String," + Path)

        If BardecodeProperties.CreateOutputSubFolders Then
            BardecodeIni.WriteValue("options", "outputFolder", "System.String," + BardecodeProperties.OutputFolder + Utils.GetOutputSubFolder(BardecodeProperties.InputFolder, Path))
        End If

    End Sub

    Public Shared Function LoadStep(StepId As Integer, ctx As VBProjectContext) As StepBardecode
        With ctx.ESteps.Single(Function(p) p.Id = StepId)
            LoadStep = New StepBardecode(Serializer.FromXml(.PropertiesObj, GetType(PropertiesBardecode)))
        End With
    End Function

    Public Sub Run(LogSub As IStep.LogSubDelegate) Implements IStep.Run
        LogSub("Bardecode Starting")
        SetBardecodeProperties()

        If BardecodeProperties.ProcessSubFolders Then
            RecursiveRun(LogSub, BardecodeProperties.InputFolder)
        Else
            LogSub("Folder: " + BardecodeProperties.InputFolder)
            StartBardecodeProcess()
        End If
        LogSub("Bardecode done" + vbCrLf)
    End Sub

    Public Sub RecursiveRun(LogSub As IStep.LogSubDelegate, Path As String)
        For Each subFolder In New DirectoryInfo(Path).GetDirectories()
            RecursiveRun(LogSub, subFolder.FullName)
        Next

        If Directory.GetFiles(Path).Count > 0 Then
            LogSub("Folder: " + Path)
            ChangePath(Path)
            StartBardecodeProcess()
        End If
    End Sub
End Class
