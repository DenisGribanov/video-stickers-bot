using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.StickerStat;

namespace VideoStickerBot.Bot.MessageHandlers.InlineQuery
{
    public class InlineResultHandler : BaseMessageHandler
    {
        private readonly List<IStat> Stats = new List<IStat>();

        public InlineResultHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsChosenInlineResult && !string.IsNullOrEmpty(TelegramUpdate.ChosenInlineResultId);

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            int stickerId = Convert.ToInt32(TelegramUpdate.ChosenInlineResultId);

            InitStats();

            foreach (var stat in Stats)
            {
                stat.Load();
                stat.Update(stickerId);
            }
        }

        private void InitStats()
        {
            Stats.Add(new PersonsStat(DataStore, CurrentUser.ChatId));
            Stats.Add(new TotalStat(DataStore));
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.INLINE_RESULT;
        }
    }
}