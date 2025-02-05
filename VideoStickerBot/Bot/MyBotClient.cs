using VideoStickerBot.Bot.BuilderHandlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;
using VideoStickerBot.Services.UserActionHistory;
using VideoStickerBot.Services.VideoResize;

namespace VideoStickerBot.Bot
{
    public class MyBotClient
    {
        private readonly ITelegram telegram;

        private readonly ITelegramUpdateMessage telegramUpdateMessage;

        private readonly IStateData stateData;

        private List<IBuilderHandlerList> builderHandlerLists;

        private readonly IBotSubSystems botSubSystems;

        private readonly IUserActionHistory actionHistory;

        public MyBotClient(ITelegram telegram,
            ITelegramUpdateMessage telegramUpdateMessage,
            IDataStore dataStore,
            IVideoResize videoResize,
            IStateData stateData,
            IUserActionHistory userActionHistory)
        {
            this.telegramUpdateMessage = telegramUpdateMessage;
            this.telegram = telegram;
            this.stateData = stateData;
            this.actionHistory = userActionHistory;
            this.botSubSystems = new BotSubSystems(telegram, dataStore, telegramUpdateMessage, videoResize, stateData);
        }

        private List<IBuilderHandlerList> Init()
        {
            stateData.LoadData();

            builderHandlerLists = new List<IBuilderHandlerList>
            {
                new BuilderForCallBackQuery(botSubSystems),
                new BuilderForFileMessage(botSubSystems),
                new BuilderForInlineQuery(botSubSystems),
                new BuilderForTextCommand(botSubSystems),
            };

            return builderHandlerLists;
        }

        public async Task Run()
        {
            var handler = Init().Where(x => x.MatchHandlerExist())
                .FirstOrDefault()?.GetHandlers().Where(x => x.Match())
                .FirstOrDefault();

            BotState? stateNow = null;

            if (handler != null)
            {
                await handler.Handle();
                stateNow = handler.UpdateStateForCurrentUser();
            }

            await actionHistory.Write(stateNow?.ToString(), telegramUpdateMessage.ToString());
        }
    }
}