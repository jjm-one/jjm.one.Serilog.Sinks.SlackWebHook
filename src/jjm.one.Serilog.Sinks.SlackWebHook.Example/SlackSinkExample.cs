﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace jjm.one.Serilog.Sinks.SlackWebHook.Example;

/// <summary>
///     This class contains examples on how to use the library.
/// </summary>
public static class SlackSinkExample
{
    /// <summary>
    ///     The main function of the <see cref="SlackSinkExample" /> class.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    [ExcludeFromCodeCoverage]
    public static void Main()
    {
        string? slackChannel = null;

        Console.WriteLine(
            "####################################################################################################");
        Console.WriteLine("Serilog Slack Sink Example (ver. " + typeof(SlackSinkExample).Assembly.GetName().Version +
                          ")");
        Console.WriteLine(
            "####################################################################################################");
        Console.WriteLine();

        Console.Write("Slack WebHook Url: ");
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) throw new ArgumentException("No valid WebHook Url!");
        var slackWebHookUrl = input;
        Console.WriteLine();

        Console.Write("Slack Channel (optional): ");
        input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input)) slackChannel = input;
        Console.WriteLine();

        Console.Write("Slack Username (optional): ");
        input = Console.ReadLine();
        var slackUsername = string.IsNullOrEmpty(input) ? input : "Serilog Slack Sink Example";
        Console.WriteLine();

        Console.Write("Slack User Icon (optional): ");
        input = Console.ReadLine();
        var slackUserIcon = string.IsNullOrEmpty(input) ? input : ":monkey_face:";
        Console.WriteLine();

        var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Slack(
                slackWebHookUrl,
                slackChannel,
                slackUsername,
                slackUserIcon,
                periodicBatchingSinkOptionsBatchSizeLimit: 1,
                periodicBatchingSinkOptionsPeriod: TimeSpan.FromMilliseconds(10),
                periodicBatchingSinkOptionsQueueLimit: 100,
                sinkRestrictedToMinimumLevel: LogEventLevel.Verbose,
                sinkOutputTemplate: "{Message}",
                sinkLevelSwitch: levelSwitch
            )
            .CreateLogger();

        Log.Write(LogEventLevel.Verbose, "Verbose Logging Message");
        Log.Write(LogEventLevel.Debug, "Debug Logging Message");
        Log.Write(LogEventLevel.Information, "Information Logging Message");
        Log.Write(LogEventLevel.Warning, "Warning Logging Message");
        Log.Write(LogEventLevel.Error, "Error Logging Message");
        Log.Write(LogEventLevel.Fatal, "Fatal Logging Message");

        try
        {
            throw new Exception("TEST EXCEPTION!");
        }
        catch (Exception e)
        {
            Log.Write(LogEventLevel.Fatal, e, "Fatal Logging Message with exception");
        }

        levelSwitch.MinimumLevel = LogEventLevel.Fatal;

        Log.Write(LogEventLevel.Error, "This Message shouldn't be send to Slack!");

        Thread.Sleep(10000);

        Console.WriteLine("Press any key to finish...");
        Console.ReadKey();

        Environment.Exit(0);
    }
}