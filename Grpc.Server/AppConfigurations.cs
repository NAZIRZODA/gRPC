using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Grpc.Server;

public static class AppConfigurations
{
    internal static void SetupGrpcApplication(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var appName = configuration["ApplicationName"];
        if (!string.IsNullOrWhiteSpace(appName))
        {
            builder.Environment.ApplicationName = appName;
        }

        builder.SetupHttp2WithoutTls();
        builder.Services.AddGrpc();
    }

    private static void SetupHttp2WithoutTls(this WebApplicationBuilder builder)
    {
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        var urlList = urls?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? [];
        var port = GetPort(urlList.FirstOrDefault());
        _ = builder.WebHost.ConfigureKestrel(
            options =>
                options.ListenAnyIP(port, listenOptions => listenOptions.Protocols = HttpProtocols.Http2));
    }

    private static int GetPort(string? url)
    {
        const int defaultServicePort = 6006;
        if (url == null)
        {
            return defaultServicePort;
        }

        var match = Regex.Match(url, @"^(?<scheme>http|https)://\+:(?<port>\d+)$");
        if (!match.Success)
        {
            return defaultServicePort;
        }

        var port = int.Parse(match.Groups["port"].Value);
        return port;
    }
}