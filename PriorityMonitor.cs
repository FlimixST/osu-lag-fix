using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal sealed class PriorityMonitor
{
    private readonly string[] _osuProcessNames;
    private readonly string _discordProcessName;
    private readonly TimeSpan _interval;

    public PriorityMonitor(string[] osuProcessNames, string discordProcessName, TimeSpan interval)
    {
        _osuProcessNames = osuProcessNames;
        _discordProcessName = discordProcessName;
        _interval = interval;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                bool osuRunning = TryGetOsuProcesses(out var osuProcesses);

                if (osuRunning)
                {
                    foreach (var p in osuProcesses)
                    {
                        TrySetPriority(p, ProcessPriorityClass.High);
                        p.Dispose();
                    }

                    foreach (var discord in GetProcessesBySafeName(_discordProcessName))
                    {
                        TrySetPriority(discord, ProcessPriorityClass.BelowNormal);
                        discord.Dispose();
                    }
                }
            }
            catch
            {
            }
        }
    }

    private bool TryGetOsuProcesses(out List<Process> osuProcesses)
    {
        var result = new List<Process>();
        var seenProcessIds = new HashSet<int>();
        foreach (var name in _osuProcessNames)
        {
            foreach (var p in GetProcessesBySafeName(name))
            {
                if (seenProcessIds.Add(p.Id))
                {
                    result.Add(p);
                }
                else
                {
                    try { p.Dispose(); } catch { }
                }
            }
        }
        osuProcesses = result;
        return osuProcesses.Count > 0;
    }

    private static IEnumerable<Process> GetProcessesBySafeName(string processNameWithoutExe)
    {
        try
        {
            return Process.GetProcessesByName(processNameWithoutExe);
        }
        catch
        {
            return Array.Empty<Process>();
        }
    }

    private static void TrySetPriority(Process process, ProcessPriorityClass priority)
    {
        try
        {
            if (!process.HasExited && process.PriorityClass != priority)
            {
                process.PriorityClass = priority;
            }
        }
        catch
        {
        }
    }
}


