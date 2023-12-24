Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class A2SInfo

    'This is the A2S_Info packet in Hex: ÿÿÿÿTSource Engine Query\0
    Public Shared A2S_INFO_Request() As Byte = {&HFF, &HFF, &HFF, &HFF, &H54, &H53, &H6F, &H75, &H72, &H63, &H65, &H20, &H45, &H6E, &H67, &H69, &H6E, &H65, &H20, &H51, &H75, &H65, &H72, &H79, &H0}

    Public Shared Sub A2SInfo()

        'Declare the target server as IPEndpoint IP/PORT
        Dim targetServer As New IPEndPoint(IPAddress.Parse("123.123.123.123"), 27015)

        'Declare UDPClient
        Using client As New UdpClient            
            'Timeout settings
            client.Client.ReceiveTimeout = 2000
            client.Client.SendTimeout = 2000

            'Send A2S_Info packet to target server
            client.Send(A2S_INFO_Request, A2S_INFO_Request.Length, targetServer)

            'Receive response and parse Challenge bytes
            Dim Challenge_Request As Byte() = client.Receive(targetServer)
            Dim Challenge_Bytes As Byte() = {Challenge_Request(5), Challenge_Request(6), Challenge_Request(7), Challenge_Request(8)}

            'Build the response to challenge request:  A2S_INFO_Request + Challenge bytes
            Dim A2S_Request_With_Challenge_Bytes As Byte() = {&HFF, &HFF, &HFF, &HFF, &H54, &H53, &H6F, &H75, &H72, &H63, &H65, &H20, &H45, &H6E, &H67, &H69, &H6E, &H65, &H20, &H51, &H75, &H65, &H72, &H79, &H0, Challenge_Bytes(0), Challenge_Bytes(1), Challenge_Bytes(2), Challenge_Bytes(3)}

            'Send the challenge request response
            client.Send(A2S_Request_With_Challenge_Bytes, A2S_Request_With_Challenge_Bytes.Length, targetServer)

            'Receive the response from server
            Dim A2S_Info As Byte() = client.Receive(targetServer)

            'Put server response in a memory stream And start a binary reader to read the stream
            Dim stream As MemoryStream = New MemoryStream(A2S_Info)
            Dim reader As BinaryReader = New BinaryReader(stream)

            'Header has to be -1, if not, bad response from server
            Dim header As Integer = reader.ReadInt32()

            'Protocol used
            Dim protocol As Integer = reader.ReadByte()

            'Protocol version
            Dim ver As Integer = reader.ReadByte()

            'Server visible host name
            Dim serverName As String = ReadSteamString(reader)

            'Current map
            Dim mapName As String = ReadSteamString(reader)

            'Game folder like cstrike or csgo
            Dim folder As String = ReadSteamString(reader)

            'Game full name like Counter-Strike 2
            Dim Game As String = ReadSteamString(reader)

            '730 for CS2
            Dim SteamApplicationID As Short = reader.ReadInt16()

            'Number of players in the server
            Dim players As Integer = reader.ReadByte()

            'Max players allowed in the server
            Dim maxplayers As Integer = reader.ReadByte()

            'Number of bots in the server
            Dim bots As Integer = reader.ReadByte()

            '100 = dedicated
            Dim ServerType As Byte = reader.ReadByte()
            Dim STOut As Char = Convert.ToChar(ServerType)

            'w = Windows, l = Linux
            Dim ServerOS As Byte = reader.ReadByte()
            Dim SOOut As Char = Convert.ToChar(ServerOS)

            '0 = Public, 1 = private
            Dim Visibility As Integer = reader.ReadByte()

            '0 = Disabled, 1 = Enabled
            Dim VAC As Integer = reader.ReadByte()

            'Server version like 1.23.0.8
            Dim Version As String = ReadSteamString(reader)
        End Using
    End Sub
    

    'Function to read null terminated strings with binary reader
    Public Shared Function ReadSteamString(ByVal reader As BinaryReader) As String
        Dim str As List(Of Byte) = New List(Of Byte)()
        Dim nextByte As Byte = reader.ReadByte()
        While nextByte <> 0
            str.Add(nextByte)
            nextByte = reader.ReadByte()
        End While
        Return Encoding.UTF8.GetString(str.ToArray())
    End Function

    

End Class
