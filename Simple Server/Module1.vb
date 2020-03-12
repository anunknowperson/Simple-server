'Simple VB.NET TCP Socket server for use with everything, but originaly coded
'by Sergey Kiselev (https://github.com/anunknowperson) for Remixed Pixel Dungeon
'First server is being runned on www.anunknown.site:3002

'You can set IP and PORT in resources strings

Imports System.Net
Imports System.Net.Sockets
Imports System.Threading.Thread
Imports System.IO
Imports System.Text

Module Module1
    Dim Server As TcpListener 'TCP Server
    Dim Clients As List(Of TcpClient) = New List(Of TcpClient) 'Array of clients

    Sub Main()
        Dim port As Integer = Integer.Parse(My.Resources.port)
        Dim localAddr As IPAddress = IPAddress.Parse(My.Resources.ip)
        Server = New TcpListener(localAddr, port)
        Server.Start()

        While True
            Console.Write("Waiting for a connection... " & vbCrLf) 'We wait for connection and then open thread for each client
            Dim client As TcpClient = Server.AcceptTcpClient()
            Console.Write("Accepted new client" & vbCrLf)

            Clients.Add(client)

            Dim Thread As Threading.Thread = New Threading.Thread(AddressOf SpeakWithClient)
            Thread.Start(client)
        End While
    End Sub

    Sub SpeakWithClient(ByVal client As TcpClient)
        Dim stream = client.GetStream()

        Dim writer As BinaryWriter = New BinaryWriter(stream)
        Dim reader As BinaryReader = New BinaryReader(stream)
        Dim message As String

        Do
            Try
                message = ReceiveMessage(reader) 'Receiving messages and sending to all other clients

                Console.WriteLine("Received: " & message)

                For Each myclient In Clients
                    SendMessage(New BinaryWriter(myclient.GetStream()), message)
                Next
            Catch e As Exception
                Clients.Remove(client)
                Console.WriteLine("Client Disconected" & vbCrLf)
                Exit Do
            End Try
        Loop
    End Sub

#Region "Network functions"
    Sub SendMessage(ByVal writer As BinaryWriter, ByVal message As String)
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(message)

        writer.Write(bytes.Length)

        writer.Write(bytes)
    End Sub
    Function ReceiveMessage(ByVal reader As BinaryReader) As String
        Dim messageLength As Integer = reader.ReadInt32
        Dim messageData() As Byte = reader.ReadBytes(messageLength)
        Dim message As String = Encoding.UTF8.GetString(messageData)

        Return message
    End Function
#End Region
End Module
