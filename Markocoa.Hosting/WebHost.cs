using EmbedIO;
using EmbedIO.Files;
using System;
using System.IO;

namespace Markocoa.Hosting;

/// <summary>
/// Hosts a webserver out of a directory.
/// </summary>
public class WebHost
{
    private readonly string directory;
    private readonly int port;
    private WebServer? server;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebHost"/> class.
    /// </summary>
    /// <param name="directory">Directory to host out of.</param>
    /// <param name="port">Port to host on.</param>
    public WebHost(string directory, int port = 8080)
    {
        this.directory = directory;
        this.port = port;
    }

    /// <summary>
    /// Refreshes the webserver (stops and restarts it).
    /// </summary>
    public void Refresh()
    {
        // Stop the existing server if running
        if (server != null)
        {
            Console.WriteLine("Stopping previous server...");
            server.Dispose();
            server = null;
        }

        // Ensure the directory exists
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // Create and start a new server
        server = new WebServer(o => o
                .WithUrlPrefix($"http://localhost:{port}/")
                .WithMode(HttpListenerMode.EmbedIO))
            .WithStaticFolder("/", directory, false, null);

        server.StateChanged += (s, e) =>
            Console.WriteLine($"Server state changed: {e.NewState}");

        server.RunAsync();

        Console.WriteLine($"Hosting {directory} on http://localhost:{port}");
    }
}
