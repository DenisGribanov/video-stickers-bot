using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.KeyboardDto;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.CallBackQuery
{
    public class SortTypeEnterHandler : BaseMessageHandler
    {
        private KeyboadBaseDto<StikerSortDto> KeyboadCallBackData;

        public SortTypeEnterHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (TelegramUpdate.IsCallBackQuery && !string.IsNullOrEmpty(TelegramUpdate.CallBackData)
                && GetKeyboardType(TelegramUpdate.CallBackData) == KeyboardTypeEnum.STICKER_SORT_SELECTED)
            {
                KeyboadCallBackData = KeyboadBaseDto<StikerSortDto>.FromJson(TelegramUpdate.CallBackData);

                isMatchForTelegramUpdate = true;
            }
            else
            {
                isMatchForTelegramUpdate = false;
            }

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            CurrentUser.SortedType = (int)KeyboadCallBackData.Data.Sort;
            DataStore.UpdateUser(CurrentUser);

            await Telegram.EditMessageReplyMarkup(TelegramUpdate.MessageId.Value, CurrentUser.ChatId, "Настройки отображения сохранены ✅\n\n/start", null);
            await Telegram.AnswerCallbackQuery(TelegramUpdate.CallBackQueryId, "Сохранено");
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.SORTING_SELECTED;
        }
    }
}