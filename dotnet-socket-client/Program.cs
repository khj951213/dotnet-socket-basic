using System.Net;
using System.Net.Sockets;
using System.Text;

var ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
var ipAddress = ipHostInfo.AddressList[0];
var ipEndPoint = new IPEndPoint(ipAddress, 11000);

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);
while (true)
{
    // Send message.
    var message = Console.ReadLine();
    var eom = "<|EOM|>";
    var messageBytes = Encoding.UTF8.GetBytes(message + eom);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message}\"");

    // Receive ack.
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    if (response == "<|ACK|>")
    {
        Console.WriteLine(
            $"Socket client received acknowledgment: \"{response}\"");
    }
    // Sample output:
    //     Socket client sent message: "Hi friends 👋!<|EOM|>"
    //     Socket client received acknowledgment: "<|ACK|>"
}