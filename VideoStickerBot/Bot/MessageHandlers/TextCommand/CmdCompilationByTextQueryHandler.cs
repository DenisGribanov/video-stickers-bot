using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdCompilationByTextQueryHandler : BaseMessageHandler
    {
        public CmdCompilationByTextQueryHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            // /compilation@VideoStickersBot брат2
            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand &&
                                       TelegramUpdate.MessageText.Contains(BotCommands.COMPILATION.Replace("/", ""))
                                       && TelegramUpdate.MessageText.Split(" ").Length > 1;

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            var query = TelegramUpdate.MessageText.Split(" ")[1].Trim().ToLower();

            List<List<KeyValuePair<string, string>>> keyboard = new List<List<KeyValuePair<string, string>>>();
            keyboard.Add(new List<KeyValuePair<string, string>>());

            if (new List<string>() { CashTagValues.ALL, CashTagValues.FRESH, CashTagValues.BEST }.Any(x => x.Replace("$", "").Equals(query)))
            {
                keyboard.LastOrDefault().Add(new KeyValuePair<string, string>("Жми сюда 🙃", $"${query}"));
            }
            else
            {
                var stickers = DataStore.GetVideoStickers().Where(x => x.IsPublished()).ToList();

                foreach (var sticker in stickers)
                {
                    foreach (string tag in sticker.GetHashTags())
                    {
                        if (!$"#{query}".Equals(tag.ToLower()))
                        {
                            continue;
                        }

                        keyboard.LastOrDefault().Add(new KeyValuePair<string, string>("Жми сюда 🙃", $"#{query}"));
                        break;
                    }

                    if (keyboard.LastOrDefault().Count > 0)
                        break;
                }
            }

            if (TelegramUpdate.ChatId.HasValue)
            {
                await Telegram.SendTextMessage("Подоборка кружочков для важных переговоров 😉🙃😊", TelegramUpdate.ChatId.Value, keyboard);
            }
            else
            {
                await Telegram.SendTextMessage("Подоборка кружочков для важных переговоров 😉🙃😊", CurrentUser.ChatId, keyboard);
            }
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.COMPILATION_BY_TEXT_QUERY_VIEW;
        }
    }
}