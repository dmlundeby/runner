using System.Diagnostics;
using CommandLine;

namespace Runner;

public class Options
{
    [Option('d', "duration", Required = false, HelpText = "Duration of the run in seconds. Default is 10 seconds.")]
    public double DurationSeconds { get; set; } = 10;

    [Option('i', "interval", Required = false, HelpText = "Interval between log messages in seconds. Default is 1 second.")]
    public double LogIntervalSeconds { get; set; } = 1;

    [Option('b', "blocking", Required = false, HelpText = "If true, the runner will block while waiting. Default is false.")]
    public bool IsBlocking { get; set; } = false;
}

internal class Program
{
    static async Task Main(string[] args)
    {
        var parser = Parser.Default.ParseArguments<Options>(args);
        await parser.WithParsedAsync(Run);
    }

    static async Task Run(Options options)
    {
        if (options.LogIntervalSeconds > options.DurationSeconds)
        {
            Console.WriteLine("Log interval cannot be greater than duration.");
            return;
        }
        var duration = TimeSpan.FromSeconds(options.DurationSeconds);
        var logInterval = TimeSpan.FromSeconds(options.LogIntervalSeconds);

        var quotient = ((int)duration.TotalMilliseconds) / ((int)logInterval.TotalMilliseconds);
        var remainderMs = ((int)duration.TotalMilliseconds) % ((int)logInterval.TotalMilliseconds);
        Console.WriteLine("Starting Runner");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (var i = 0; i < quotient; i++)
        {
            if (options.IsBlocking)
                Thread.Sleep((int)logInterval.TotalMilliseconds);
            else
                await Task.Delay((int)logInterval.TotalMilliseconds);
            Console.WriteLine($"Elapsed: {(i + 1) * logInterval.TotalSeconds:0.#}s");
        }
        await Task.Delay(remainderMs);
        Console.WriteLine($"Runner finished after {duration.TotalSeconds:0.#} seconds.");
    }
}
