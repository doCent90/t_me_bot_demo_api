using Serilog;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;

namespace HelloBotNET.AppService.Services;

/// <summary>
/// It contains the main functionality of the telegram bot. <br />
/// The application creates a new instance of this class to process each update received.
/// </summary>
public partial class HelloBot : SimpleTelegramBotBase
{
    protected override async void OnCommand(Message message, string commandName, string commandParameters)
    {
        var args = commandParameters.Split(' ');
#if DEBUG
        this.logger.LogInformation("Params: {ArgsLenght}", args.Length);
#endif
        Log.Information("Command " + commandName);

        switch (commandName)
        {
            case "hello": // Reply to /hello command
                var hello = string.Format("Hello, че надо?, {0}!", message.From!.FirstName);
                this.Client.SendMessage(message.Chat.Id, hello);
                break;
            case "show":
                this.Client.SendMessage(message.Chat.Id, "Ага, cек");
                await SendMainMenu(message.Chat.Id);
                break;
            default:
                if (message.Chat.Type == ChatTypes.Private)
                {
                    this.Client.SendMessage(message.Chat.Id, "Unrecognized command.");
                }
                break;
        }
    }
}
