using System.Collections.Generic;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdCompilationHandler : BaseMessageHandler
    {
        public CmdCompilationHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand &&
                                        (TelegramUpdate.MessageText.Contains(BotCommands.START_BOT) && TelegramUpdate.MessageText.Contains(BotCommands.COMPILATION.Replace("/", ""))                                         
                                            || 
                                            TelegramUpdate.MessageText.Equals(BotCommands.COMPILATION) ||
                                            TelegramUpdate.MessageText.StartsWith(BotCommands.COMPILATION) && TelegramUpdate.MessageText.Contains("@")
                                            );

            return isMatchForTelegramUpdate.Value;
        }
        public override async Task Handle()
        {
            if (!Match()) return;

            var stickers = DataStore.GetVideoStickers().Where(x => x.IsPublished()).ToList();

            Dictionary<string, int> hashTag = new();

            foreach (var sticker in stickers)
            {
                foreach (string tag in sticker.GetHashTags())
                {
                    if (!hashTag.ContainsKey(tag))
                    {
                        hashTag.Add(tag, 1);
                        continue;
                    }

                    hashTag[tag]++;
                }
            }

            var sortedTags = from entry in hashTag where entry.Value > 1 orderby entry.Value descending select entry;

            var keyBoard = GetKeyboard(sortedTags);
            keyBoard.Add(new List<KeyValuePair<string, string>>());
            keyBoard.LastOrDefault().Add(new KeyValuePair<string, string>("Все 📲", ""));
            keyBoard.LastOrDefault().Add(new KeyValuePair<string, string>("Свежие 🆕", CashTagValues.FRESH));
            keyBoard.LastOrDefault().Add(new KeyValuePair<string, string>("Популярные 🔥", CashTagValues.BEST));

            if (TelegramUpdate.ChatId.HasValue)
            {
                await Telegram.SendTextMessage("Подоборки кружочков для важных переговоров 😉🙃😊", TelegramUpdate.ChatId.Value, keyBoard);
            }
            else
            {
                await Telegram.SendTextMessage("Подоборки кружочков для важных переговоров 😉🙃😊", CurrentUser.ChatId, keyBoard);
            }

        }

        private List<List<KeyValuePair<string, string>>> GetKeyboard(IOrderedEnumerable<KeyValuePair<string, int>> keyValuePairs)
        {
            const int MAX_ELEMENT_IN_LINE = 3;
            double rows = Math.Round((double)keyValuePairs.Count() / (double)MAX_ELEMENT_IN_LINE, MidpointRounding.ToPositiveInfinity);

            List<List<KeyValuePair<string, string>>> result = new();

            for (int i = 0; i < rows; i++)
            {
                List<KeyValuePair<string, string>> rowData = new List<KeyValuePair<string, string>>();
                int elCount = 0;

                for (int j = 0; j < MAX_ELEMENT_IN_LINE; j++)
                {
                    if (i * MAX_ELEMENT_IN_LINE + j >= keyValuePairs.Count()) break;

                    var keyValue = keyValuePairs.ElementAt(i * MAX_ELEMENT_IN_LINE + j);
                    rowData.Add(new KeyValuePair<string, string>(keyValue.Key, keyValue.Key));

                    elCount++;

                    if (elCount == MAX_ELEMENT_IN_LINE) break;
                }

                result.Add(rowData);
            }

            return result;

        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.COMPILATION_VIEW;
        }
    }
}
