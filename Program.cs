using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Spectre.Console;
using static Spectre.Console.AnsiConsole;

Write(new FigletText("LIGHTS AI").Centered().Color(Color.Green));
Markup("[lightgreen dim]I am your lights assistant, get started -> [/] [underline gold3_1]how many lights are on?[/]");
WriteLine();

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: "gpt-35-turbo",
    endpoint: "<deployment-endpoint>",
    apiKey: "<api-key>"
);

var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<LightsPlugin>(pluginName: "Lights_Interactor");

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), //auto/required for function invoke by LLM
};

var history = new ChatHistory();
string? userInput;
while (true)
{
    WriteLine();
    var rule = new Rule("[red]Ask away[/]");
    rule.LeftJustified();
    Write(rule);

    WriteLine();
    userInput = Prompt(new TextPrompt<string>("> "));

    if (string.IsNullOrEmpty(userInput))
    {
        break;
    }

    await Status()
        .AutoRefresh(false)
    .Spinner(Spinner.Known.Star)
    .SpinnerStyle(Style.Parse("green bold"))
    .Start("...", async ctx =>
    {
        history.AddUserMessage(userInput);

        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);
        history.AddMessage(result.Role, result.Content ?? string.Empty);
        Write(
            new Panel($"[yellow]{result.Content}[/]")
                        .Expand()
                        .RoundedBorder()
                        .Header("[blue] Lights Assistant [/]")
                        .HeaderAlignment(Justify.Right));
        ctx.Refresh();
    });
}

