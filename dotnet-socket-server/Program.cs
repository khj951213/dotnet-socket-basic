using System.Net;
using System.Net.Sockets;
using System.Text;

var ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
var ipAddress = ipHostInfo.AddressList[0];
var ipEndPoint = new IPEndPoint(ipAddress, 11000);

using var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(ipEndPoint);
listener.Listen(100);

var handler = await listener.AcceptAsync();

while (true)
{
    try
    {
        // receive message
        var buffer = new byte[1024];
        var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);
        var eom = "<|EOM|>";
        if (response.IndexOf(eom) > -1)
        {
            Console.WriteLine($"Socket server receive message: {response.Replace(eom, "")}");
            var ackMessage = "<|ACK|>";
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await handler.SendAsync(echoBytes, 0);
            Console.WriteLine($"Socket server sent acknowledgement: {ackMessage}");
        }
    }
    catch (Exception ex)
    {

    }
}