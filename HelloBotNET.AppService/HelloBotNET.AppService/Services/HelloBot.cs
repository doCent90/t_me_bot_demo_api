using Serilog;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.UpdatingMessages;

namespace HelloBotNET.AppService.Services;

/// <summary>
/// It contains the main functionality of the telegram bot. <br />
/// The application creates a new instance of this class to process each update received.
/// </summary>
public partial class HelloBot : SimpleTelegramBotBase
{
    private const string MainMenu = "Main menu";
    private const string TutorMenu = "Tutor menu";

    private InlineKeyboardButton _back;
    private InlineKeyboardButton _next;
    private InlineKeyboardButton _tutorial;

    private readonly InlineKeyboardMarkup _tutorMenuMarkup;
    private readonly InlineKeyboardMarkup _mainMenuMarkup;
    private readonly ILogger<HelloBot> logger;

    public ITelegramBotClient Client { get; }

    public HelloBot(ILogger<HelloBot> logger, IConfiguration configuration)
    {
        this.logger = logger;

        var botToken = configuration.GetValue<string>("Telegram:BotToken");
        this.Client = new TelegramBotClient(botToken);

        var myUsername = this.Client.GetMe().Username!;
        // This will provide a better command filtering.
        this.SetCommandExtractor(myUsername, true);

        logger.Log(LogLevel.Warning, "HelloBot Up");
        Log.Information("HelloBot Up");

        _back = new InlineKeyboardButton("Back");
        _back.CallbackData = "Back";
        _next = new InlineKeyboardButton("Next");
        _next.CallbackData = "Next";
        _tutorial = new InlineKeyboardButton("Tutor");
        _tutorial.Url = "https://core.telegram.org/bots/tutorial";

        _mainMenuMarkup = new(new[] { new[] { _next } });

        _tutorMenuMarkup = new(
            new[] {
                new[] { _back },
                new[] { _tutorial },
            }
        );
    }

    public async Task SendMainMenu(long chatId)
    {
        SendMessageArgs args = new(chatId, MainMenu);
        args.ParseMode = "Html";
        args.ReplyMarkup = _mainMenuMarkup;

        Log.Information($"Try send menu {args.Text}, {args.ParseMode}, {args.ChatId}, entity - {_mainMenuMarkup}");

        await this.Client.SendMessageAsync(args);
    }

    public async Task SendTutorMenu(long chatId)
    {
        SendMessageArgs args = new(chatId, TutorMenu);
        args.ParseMode = "Html";
        args.ReplyMarkup = _tutorMenuMarkup;

        Log.Information($"Try send menu {args.Text}, {args.ParseMode}, {args.ChatId}, entity - {_tutorMenuMarkup}");

        await this.Client.SendMessageAsync(args);
    }

    private async Task HandleButton(CallbackQuery query)
    {
        if (query == null)
            return;

        string text = string.Empty;
        InlineKeyboardMarkup markup = new(new[] { Array.Empty<InlineKeyboardButton>() });

        Log.Information($"Try handle button query {query.Message}, from {query.From}");

        if (query.Data == _next.Text)
        {
            text = TutorMenu;
            markup = _tutorMenuMarkup;
        }
        else if (query.Data == _back.Text)
        {
            text = MainMenu;
            markup = _mainMenuMarkup;
        }
        else
        {
            Log.Warning($"Handle button query fail {query.Data}, {query.Message}, from {query.From}");
        }

        Log.Information($"Handled button query succes {query.Data}");

        // Close the query to end the client-side loading animation

        var closed = await this.Client.AnswerCallbackQueryAsync(query.Id);
        Log.Information($"Handled button Answer Callbac close {query.Data} succes - {closed}");

        EditMessageTextArgs args = new(text);
        args.ChatId = query.Message!.Chat.Id;
        args.MessageId = query.Message.MessageId;
        args.Text = text;
        args.ReplyMarkup = markup;

        // Replace menu text and keyboard
        var result = this.Client.EditMessageText<Message>(args);

        Log.Information($"Try send edited message with result {result.Text}:: {args.Text}, {args.MessageId}, {args.ChatId}, entity - {markup}");
    }
}
