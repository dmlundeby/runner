using System.Diagnostics;
using CommandLine;

namespace Runner;

public class Options
{
    [Option('d', "duration", Required = false, HelpText = "Duration of the run in seconds. Default is 10 seconds.")]
    public double DurationSeconds { get; set; } = 10;

    [Option('i', "interval", Required = false, HelpText = "Interval between log messages in seconds. Default is 1 second.")]
    public double LogIntervalSeconds { get; set; } = 1;

    [Option('b', "blocking", Required = false, HelpText = "If true, the runner will block for the duration of the run. Default is false.")]
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
        Console.WriteLine("Starting Runner");
        if (options.LogIntervalSeconds > options.DurationSeconds)
        {
            Console.WriteLine("Log interval cannot be greater than duration.");
            return;
        }

        var duration = TimeSpan.FromSeconds(options.DurationSeconds);
        var logInterval = TimeSpan.FromSeconds(options.LogIntervalSeconds);

        var durationTask = Task.Run(async () =>
        {
            await Task.Delay((int)duration.TotalMilliseconds);
        });
        var loggerTask = Task.Run(async () =>
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await Task.Delay((int)logInterval.TotalMilliseconds);
            while (stopwatch.Elapsed < duration)
            {
                Console.WriteLine($"Elapsed: {Math.Round(stopwatch.Elapsed.TotalMilliseconds / 1000.0, 1)}s");
                await Task.Delay((int)logInterval.TotalMilliseconds);
            };
        });
        await durationTask;
        Console.WriteLine($"Runner finished after {Math.Round(duration.TotalSeconds, 0)} seconds.");
    }

    static void HandleError()
    {
        Console.WriteLine("Error parsing arguments.");
    }
}
