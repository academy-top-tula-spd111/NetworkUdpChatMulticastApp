using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks.Dataflow;

int portLocal = 7777;
IPAddress addressBrodcast = IPAddress.Parse("235.5.5.11");

Console.Write("Input name: ");
string name = Console.ReadLine();

Task.Run(ReceiveMessageAsync);
await SendMessageAsync();


async Task SendMessageAsync()
{
    using UdpClient cliendSender = new UdpClient();

    while(true)
    {
        string message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message)) break;

        message = $"{name}: {message}";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await cliendSender.SendAsync(buffer, new IPEndPoint(addressBrodcast, portLocal));
    }
}

async Task ReceiveMessageAsync()
{
    using UdpClient clientReceiver = new UdpClient(portLocal);
    clientReceiver.JoinMulticastGroup(addressBrodcast);
    clientReceiver.MulticastLoopback = false;

    while (true)
    {
        var messageBytes = await clientReceiver.ReceiveAsync();
        var message = Encoding.UTF8.GetString(messageBytes.Buffer);
        if (message == "END") break;
    }

    clientReceiver.DropMulticastGroup(addressBrodcast);

}