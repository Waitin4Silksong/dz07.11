using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

string host = "127.0.0.1";
int port = 8888;

using TcpClient client = new TcpClient();
Console.Write("Enter your name: ");
string? userName = Console.ReadLine();
Console.WriteLine($"Welcome, {userName}");

StreamReader? reader = null;
StreamWriter? writer = null;

try
{
    client.Connect(host, port);
    reader = new StreamReader(client.GetStream());
    writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
}
catch (Exception e)
{
    Console.WriteLine($"Connection error: {e.Message}");
    return;
}

if (writer is null || reader is null)
{
    Console.WriteLine("Failed to initialize reader or writer.");
    return;
}

Console.WriteLine("Connected to the server!");

Task receiveTask = ReceiveMessageAsync(reader);
Task sendTask = SendMessageAsync(writer, userName);

await Task.WhenAny(receiveTask, sendTask);

async Task SendMessageAsync(StreamWriter writer, string userName)
{
    Console.WriteLine("Type your messages below:");
    while (true)
    {
        string? message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message)) continue;

        string formattedMessage = $"{userName}: {message}";
        await writer.WriteLineAsync(formattedMessage);
    }
}

async Task ReceiveMessageAsync(StreamReader reader)
{
    try
    {
        while (true)
        {
            string? message = await reader.ReadLineAsync();
            if (message != null)
            {
                Print(message);
            }
        }
    }
    catch (IOException)
    {
        Console.WriteLine("Disconnected from server.");
    }
}

void Print(string message)
{
    if (OperatingSystem.IsWindows())
    {
        var position = Console.GetCursorPosition();
        int left = position.Left;
        int top = position.Top;
        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
        Console.SetCursorPosition(0, top);
        Console.WriteLine(message);
        Console.SetCursorPosition(left, top + 1);
    }
    else
    {
        Console.WriteLine(message);
    }
}
