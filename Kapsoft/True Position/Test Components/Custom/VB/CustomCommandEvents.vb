Imports TruePosition.Test.DataLayer

Namespace TruePosition.Test.Custom.VB

    Public Class CustomCommandEvents

        ' DESIGN NOTE:
        ' Global values will be stored in a global value cache accessible to all system components...
        Private Shared bRMA As Boolean = True
        Private Shared ESN_Customer_Scanned As String = "TRULMU5207872AE"
        Private Shared FPGAType As String

        ' Command: CUSTESN
        <ResponseProcessed("1A", 5, 2, 1)> _
        Public Shared Sub CustomerESN_Processed(ByVal sender As Object, ByVal args As ExpectedResponseProcessedArgs)

            Dim er As ExpectedResponse = sender
            If args.Value.GetString() <> ESN_Customer_Scanned And bRMA Then
                Throw New InvalidOperationException("ESN/barcode mismatch")
            End If

        End Sub

        ' Command: CP/DSP
        <ResponseProcessed("1A", 5, 7, 1)> _
        Public Shared Sub CPDSP_Processed(ByVal sender As Object, ByVal args As ExpectedResponseProcessedArgs)

            Dim er As ExpectedResponse = sender

            '**************************************************
            'Block below included to accomodate the addition
            'of the code to force FPGA code download. The
            'presence of any and all hard-coded numbers in
            'this section have been approved by John Ghabra.
            'In some sections below, FPGAType is also populated
            'based on the type of FPGA file currently present
            'in the LMU. That section will remain because not
            'all bootup sequences perform ?CO. Therefore, by
            'default, we will have the FPGAType = "N" because
            'it is more inclusive       W.D. '12/6/06
            '**************************************************
            If InStr(1, er.Value.GetString(), "061600") > 0 Then
                FPGAType = "O"  'O stands for Old CPDSP
            ElseIf InStr(1, er.Value.GetString(), "061641") > 0 Then
                FPGAType = "E"  'E stands for E-CPDSP
            ElseIf InStr(1, er.Value.GetString(), "061644") > 0 Then
                FPGAType = "N"  'N stands for NON-ECPDSP
            End If
            '**************************************************

        End Sub

    End Class

End Namespace

