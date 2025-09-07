using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

[SupportedOSPlatform("windows")]
internal static class Program
{
    private static async Task Main()
    {
        using var cts = new CancellationTokenSource();
        AppDomain.CurrentDomain.ProcessExit += (_, __) => cts.Cancel();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        var monitor = new PriorityMonitor(
            new[] { "osu!", "osu!lazer", "osulazer", "osu" },
            "Discord",
            TimeSpan.FromSeconds(1)
        );

        try
        {
            await monitor.RunAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }
}


