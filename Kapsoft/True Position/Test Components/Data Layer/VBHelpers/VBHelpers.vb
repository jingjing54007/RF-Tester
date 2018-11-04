Imports System.Globalization

Namespace TruePosition.Test.DataLayer
    ''' <summary>
    ''' Provides helpers for things that are only available in VB.NET.
    ''' </summary>
    Public NotInheritable Class VBHelpers
        Public Shared SimpleWildcards() As Char = {"*"c, "%"c, "?"c, "_"c}
        Public Shared AdvancedWildcards() As Char = {"["c, "]"c, "^"c, "!"c, "-"c, "#"c}
        Public Shared AllWildcards() As Char = SimpleWildcards + AdvancedWildcards

        ''' <summary>
        ''' Performs a VB 'like' operation comparing value against the provided pattern. Also supports T-SQL pattern matching wildcards.
        ''' </summary>
        ''' <param name="value">string to compare</param>
        ''' <param name="pattern">VB Like or T-SQL LIKE pattern to match</param>
        ''' <returns>true/false - match/no match</returns>
        Public Shared Function Match(ByVal value As String, ByVal pattern As String) As Boolean
            Dim vbPattern As String = pattern

            Match = True
            If (Not String.IsNullOrEmpty(vbPattern)) Then
                ' Replace T-SQL wildcards with VB equivallent...
                vbPattern = vbPattern.Replace("_", "?").Replace("%", "*").Replace("^", "!")

                ' P.S. - I hate VB...
                If (Not String.IsNullOrEmpty(value)) Then
                    value = value.ToUpper(CultureInfo.CurrentUICulture)
                End If

                Match = value Like vbPattern.ToUpper(CultureInfo.CurrentUICulture)
            End If

        End Function
        Public Shared Function Length(ByVal value As Object) As Integer
            Length = Len(value)
        End Function
        Public Shared Function ValidESN(ByVal strESN As String) As Boolean
            Dim n As Integer

            ValidESN = True
            For n = 1 To Len(strESN)
                If (Mid(strESN, n, 1) Like "[!a-zA-Z0-9 /,]") Then
                    ValidESN = False
                    Exit For
                End If
            Next
        End Function
    End Class
End Namespace
