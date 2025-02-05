using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.KeyboardDto;
using VideoStickerBot.Constants;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand
{
    public class CmdSettingsHandler : BaseMessageHandler
    {
        public CmdSettingsHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand && TelegramUpdate.MessageText.Contains(BotCommands.SETTINGS);

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            await Telegram.SendTextMessage("__Выберите вариант сортировки кружочков в поисковике__:" +
                "\n\n1. По полурности среди всех пользователей 👨‍👩‍👦‍👦" +
                "\n\n2. По полурности у вас 💕 (выше в списке будут те кружочки, которые вы кликали чаще всего)" +
                "\n\n3. По дате добавления 🗓(вверху будут самые новые)" +
                "\n\n4. Сначала добавленные мной 👤", TelegramUpdate.ChatId.Value, null, GeKeyBoardData());
        }

        private Dictionary<string, string> GeKeyBoardData()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            int count = 0;
            foreach (var key in new List<SortEnum>() { SortEnum.POPULAR, SortEnum.PERSON_RANKING, SortEnum.NEWEST, SortEnum.OWN })
            {
                count++;
                string btnText = key.Equals((SortEnum)CurrentUser.SortedType) ? count + "✅" : count.ToString();
                string data = KeyboadBaseDto<StikerSortDto>.InitJson(KeyboardTypeEnum.STICKER_SORT_SELECTED, StikerSortDto.Init(key));
                result.Add(btnText, data);
            }

            return result;
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.SETTINGS;
        }
    }
}