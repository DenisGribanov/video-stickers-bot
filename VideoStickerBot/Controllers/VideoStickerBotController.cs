using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using VideoStickerBot.Bot;
using VideoStickerBot.Database;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.UserActionHistory;
using VideoStickerBot.Services.VideoResize;
using Update = Telegram.Bot.Types.Update;

namespace VideoStickerBot.Controllers
{
    [Route("bot")]
    public class VideoStickerBotController : Controller
    {
        private IConfiguration configuration;
        private ILogger<VideoStickerBotController> logger;
        private readonly ITelegram telegramAdapter;
        private readonly IVideoResize videoResize;
        private readonly IDataStore dataStore;
        private readonly IUserActionHistory userActionHistory;
        private readonly ITelegramBotClient _telegramClient;

        public VideoStickerBotController(IConfiguration configuration,
            ILogger<VideoStickerBotController> logger,
            VideoStikersBotContext dbContext,
            ITelegram telegramAdapter,
            ITelegramBotClient telegramBotClient,
            IVideoResize videoResize)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.telegramAdapter = telegramAdapter;
            this.videoResize = videoResize;
            this.dataStore = new DataStoreProxy(dbContext);
            this.userActionHistory = new UserActionHistoryImpl(dbContext);
            _telegramClient = telegramBotClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(Variables.GetInstance().BOT_DOMAIN_NAME);
        }

        [HttpGet("sticker")]
        [ResponseCache(Duration = 2678400)]
        public IActionResult GetSticker([FromQuery] string fileUniqueId)
        {
            var sticker = dataStore.GetVideoStickers().Where(x => x.FileUniqueId != null &&
                    x.FileUniqueId.Equals(fileUniqueId)).FirstOrDefault();

            if (sticker == null) return NotFound();

            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Description", sticker.Description);
            result.Add("HashTags", sticker.Hashtags);

            return Ok(result);
        }

        [HttpPost("update")]
        public async Task<IActionResult> VideoStickBot([FromBody] Update update)
        {
            if (update == null) return BadRequest();

            logger.LogInformation("{@update}", Newtonsoft.Json.JsonConvert.SerializeObject(update));

            await CallBot(update);

            return Ok();
        }

        [HttpPut("webhook")]
        public async Task<IActionResult> WebHook(string url)
        {
            await _telegramClient.SetWebhookAsync(url);
            return Ok();
        }

        [HttpGet("webhook")]
        public async Task<IActionResult> WebHookInfo()
        {
            var result = await _telegramClient.GetWebhookInfoAsync();
            return Ok(result);
        }

        private async Task CallBot(Update update)
        {
            TelegramUpdateMessageAdapter tgUpdate = null;
            try
            {
                tgUpdate = new TelegramUpdateMessageAdapter(update, Variables.GetInstance().BOT_CHAT_ID);

                var stateData = new StateData(dataStore, tgUpdate);

                MyBotClient botHandlersBuilder = new(telegramAdapter,
                    tgUpdate,
                    dataStore,
                    videoResize,
                    stateData,
                    userActionHistory);

                await botHandlersBuilder.Run();
            }
            catch (Exception e)
            {
                string uid = Guid.NewGuid().ToString();
                logger.LogError(e, uid);
                await _telegramClient.SendTextMessageAsync(tgUpdate.UserFromId, "Произошла ошибка 🥺🥺🥺. Админ уже разбирается 💪");
                await _telegramClient.SendTextMessageAsync(Variables.GetInstance().BOT_OWNER_CHAT_ID, $"Произошла ошибка во время работы бота. ID {uid}");
            }
        }
    }
}