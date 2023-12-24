# vb.net-a2s_Info
Private A2S_INFO_Request() As Byte = {&HFF, &HFF, &HFF, &HFF, &H54, &H53, &H6F, &H75, &H72, &H63, &H65, &H20, &H45, &H6E, &H67, &H69, &H6E, &H65, &H20, &H51, &H75, &H65, &H72, &H79, &H0}
Dim targetServer As New IPEndPoint(IPAddress.Parse("123.123.123.123"), 27015)
  Using client As New UdpClient
  
    client.Client.ReceiveTimeout = 5000
    client.Client.SendTimeout = 5000
    
    client.Send(A2S_INFO_Request, A2S_INFO_Request.Length, targetServer)
    Dim Challenge_Request As Byte() = client.Receive(targetServer)
    Dim Challenge_Bytes As Byte() = {Challenge_Request(5), Challenge_Request(6), Challenge_Request(7), Challenge_Request(8)}
    Dim A2S_Request_With_Challenge_Bytes As Byte() = {&HFF, &HFF, &HFF, &HFF, &H54, &H53, &H6F, &H75, &H72, &H63, &H65, &H20, &H45, &H6E, &H67, &H69, &H6E, &H65, &H20, &H51, &H75, &H65, &H72, &H79, &H0, Challenge_Bytes(0), Challenge_Bytes(1), Challenge_Bytes(2), Challenge_Bytes(3)}
    client.Send(A2S_Request_With_Challenge_Bytes, A2S_Request_With_Challenge_Bytes.Length, targetServer)
    Dim A2S_Info As Byte() = client.Receive(targetServer)

     Dim stream As MemoryStream = New MemoryStream(A2S_Info)
     Dim reader As BinaryReader = New BinaryReader(stream)
      
     Dim header As Integer = reader.ReadInt32()
     'MsgBox(header.ToString)
     Dim protocol As String = reader.ReadByte()
     'MsgBox(protocol)
     Dim ver As String = reader.ReadByte()
     'MsgBox(ver)
            Dim serverName As String = ReadSteamString(reader)
            ' MsgBox(serverName)
            Dim mapName As String = ReadSteamString(reader)
            'MsgBox(mapName)
            Dim folder As String = ReadSteamString(reader)
            'MsgBox(folder)
            Dim Game As String = ReadSteamString(reader)
            ' MsgBox(Game)
            Dim SteamApplicationID As Short = reader.ReadInt16()
            'MsgBox(SteamApplicationID.ToString)
            Dim players As String = reader.ReadByte()
            'MsgBox(players)
            Dim maxplayers As String = reader.ReadByte()
            'MsgBox(maxplayers)
            Dim bots As String = reader.ReadByte()
            'MsgBox(bots)

            Dim ServerType As Byte = reader.ReadByte()
            Dim dOut As Char = Convert.ToChar(ServerType)
            'MsgBox(dOut) '100 = dedicated

            Dim ServerOS As Byte = reader.ReadByte()
            Dim vOut As Char = Convert.ToChar(ServerOS)
            'MsgBox(vOut)

            Dim Visibility As String = reader.ReadByte() ' 0 = public - 1 = private
            'MsgBox(Visibility)

            Dim VAC As String = reader.ReadByte() ' 0 = Disabled - 1 = Enabled
            'MsgBox(VAC)

            Dim Version As String = ReadSteamString(reader)
            'MsgBox(Version)          
        End Using


         Private Function ReadSteamString(ByVal reader As BinaryReader) As String
        Dim str As List(Of Byte) = New List(Of Byte)()
        Dim nextByte As Byte = reader.ReadByte()

        While nextByte <> 0
            str.Add(nextByte)
            nextByte = reader.ReadByte()
        End While

        Return Encoding.UTF8.GetString(str.ToArray())
    End Function
