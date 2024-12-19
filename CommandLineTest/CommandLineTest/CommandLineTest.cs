using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;

class CommandLineTest
{
    static int Main(string[] args)
    {
        return ProcessCommandLine(args);
    }

    static int ProcessCommandLine(string[] args)
    {
        // Get the executable name
        string executableName = AppDomain.CurrentDomain.FriendlyName;

        // Retrieve the version from the assembly
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        Console.WriteLine($"{executableName} v{version}");

        Console.WriteLine();

        // Define the allowed choices
        var allowedBoards = new[] { "control", "exciter", "multiplexer", "universal" };

        // Create a root command with some options
        var rootCommand = new RootCommand
        
        {
            new Option<string>(
                new string[] { "-b", "--board" },
                "Board to update")
            {
                ArgumentHelpName = "board",
                IsRequired = true // Make this option required
            }.FromAmong(allowedBoards),
            new Option<string>(
                new string[] { "-f", "--filename"},
                ".hex upgrade file"),
            new Option<bool>(
                new string[] { "-g", "--go"},
                "Run application (exit Bootloader)"),
            new Option<bool>(
                new string[] { "-m"},
                "Send messages through Multiplexer"),
            new Option<int>(
                new string[] { "-n", "--number"},
                "Control/MUX board to talk to"),
            new Option<bool>(
                new string[] { "-r", "--reset"},
                "Reset board to Bootloader")
        };

        // Set the description for the root command
        rootCommand.Description = $"The Bootloader Firmware Updater (BLFWU) " +
        "is used to send firmware (.hex files) to boards running the " +
        "Bootloader. This version will NOT update older boards that expect " +
        "the 'A'/'B' firmware files. It can also reboot a board back in to " +
        "the Bootloader, or run the application on the board.";

        // Set the handler for the root command
        rootCommand.SetHandler((string board, string filename, bool go, bool usemux, int number, bool reset) =>
        {
            if (!string.IsNullOrEmpty(board))
            {
                Console.WriteLine($"Option -b was provided with value: {board}");
            }
            if (!string.IsNullOrEmpty(filename))
            {
                Console.WriteLine($"Option -f was provided with value: {filename}");
            }
            if (go)
            {
                Console.WriteLine("Option -g was provided");
            }
            if (usemux)
            {
                Console.WriteLine("Option -m was provided");
            }
            if (number != 0)
            {
                Console.WriteLine($"Option -n was provided with value: {number}");
            }
            if (reset)
            {
                Console.WriteLine("Option --reset was provided");
            }
            if (string.IsNullOrEmpty(board) && string.IsNullOrEmpty(filename) && !go && !usemux && number == 0 && !reset)
            {
                Console.WriteLine("No options provided");
            }
        }, 
        rootCommand.Options[0] as Option<string>,
        rootCommand.Options[1] as Option<string>,
        rootCommand.Options[2] as Option<bool>,
        rootCommand.Options[3] as Option<bool>,
        rootCommand.Options[4] as Option<int>,
        rootCommand.Options[5] as Option<bool>);

        // Invoke the root command
        return rootCommand.InvokeAsync(args).Result;
    }
}