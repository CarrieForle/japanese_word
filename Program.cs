﻿using System.CommandLine;
using lol.Run;

record struct Vocabulary
{
    public string word;
    public string description;
    public string pronunciation;
};

/* Options to run the program */
enum RunOption
{
    desc,
    pron
};

public class Program
{
    private static async Task Main(string[] args)
    {
        var DefaultRunOption = () => RunOption.desc;
        var DefaultFilePath = () => @"README.md";
        
        Option<RunOption> runOption = new(
            aliases: ["-m", "--mode"],
            description: "Determine the type of test. Default to description test.",
            getDefaultValue: DefaultRunOption);
        Option<string> filePathOption = new(
            aliases: ["-f", "--file"],
            description: "The file to to read from.",
            getDefaultValue: DefaultFilePath);

        RootCommand rootCommand = new("Test your Japanese skills!");
        rootCommand.AddOption(runOption);
        rootCommand.AddOption(filePathOption);

        rootCommand.SetHandler(Run, runOption, filePathOption);

        await rootCommand.InvokeAsync(args);    
    }

    private static void Run(RunOption runOption, string filePath) {
        switch (runOption) {
            case RunOption.desc:
                RunByDescription.InitializeInstance(filePath);
                RunByDescription.GetInstance().Run();
                break;
            case RunOption.pron:
                RunByPronunciation.InitializeInstance(filePath);
                RunByPronunciation.GetInstance().Run();
                break;
        }
    }
}
