using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using VideoStickerBot.Bot;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Database;
using VideoStickerBot.Services;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.UserActionHistory;
using VideoStickerBot.Services.VideoResize;

namespace VideoStickerBot.Controllers
{
    [Route("video-sticker-bot")]
    public class VideoStickerBotController : Controller
    {
        private IConfiguration configuration;
        private ILogger<VideoStickerBotController> logger;
        private readonly ITelegram telegramAdapter;
        private readonly IVideoResize videoResize;
        private readonly IDataStore dataStore;
        private readonly IUserActionHistory userActionHistory;

        public VideoStickerBotController(IConfiguration configuration,
            ILogger<VideoStickerBotController> logger,
            VideoStikersBotContext dbContext,
            ITelegram telegramAdapter,
            IVideoResize videoResize)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.telegramAdapter = telegramAdapter;
            this.videoResize = videoResize;
            this.dataStore = new DataStoreProxy(dbContext);
            this.userActionHistory = new UserActionHistoryImpl(dbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(Variables.GetInstance().BOT_DOMAIN_NAME);
        }



        [HttpPost("update")]
        public async Task<IActionResult> VideoStickBot([FromBody] Update update)
        {
            if (update == null) return BadRequest();

            logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            await CallBot(update);

            return Ok();
        }


        private async Task CallBot(Update update)
        {
            try
            {
                var tgUpdate = new TelegramUpdateMessageAdapter(update);

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
                logger.LogError(e, null);
            }
        }
    }
}
