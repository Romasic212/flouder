using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class TcpService
{
    private TcpListener _listener;

    public event Action<string> OnMessageReceived;

    public void StartServer(int port = 9000)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();

        Task.Run(async () =>
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                HandleClient(client);
            }
        });
    }

    private async void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];

        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        OnMessageReceived?.Invoke($"Получено: {message}");
    }

    public async Task Connect(string ip, int port = 9000)
    {
        var client = new TcpClient();
        await client.ConnectAsync(IPAddress.Parse(ip), port);

        var stream = client.GetStream();

        var message = Encoding.UTF8.GetBytes("hello");

        await stream.WriteAsync(message, 0, message.Length);
    }
}