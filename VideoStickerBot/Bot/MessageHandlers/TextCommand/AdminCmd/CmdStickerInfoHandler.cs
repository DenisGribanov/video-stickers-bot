using System.Text;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd
{
    public class CmdStickerInfoHandler : BaseMessageHandler
    {
        long? stickerId;
        VideoSticker sticker;
        public CmdStickerInfoHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            if (TelegramUpdate.IsBotCommand && TelegramUpdate.MessageText.Contains(BotCommands.ID))
            {
                stickerId = DigitParse(TelegramUpdate.MessageText);

                isMatchForTelegramUpdate = stickerId.HasValue
                                            && CurrentUser.UserIsAdmin() || CurrentUser.UserIsReviewer();
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

            sticker = DataStore.GetVideoStickers().FirstOrDefault(x => x.Id == stickerId.Value);

            if (sticker == null)
            {
                await Telegram.SendTextMessage("Стикер не найден 🤷🏻‍♂️", CurrentUser.ChatId);
                return;
            }

            await Telegram.SendTextMessage(GetTextMessage(), CurrentUser.ChatId);
        }

        private string GetTextMessage()
        {
            StringBuilder sB = new StringBuilder();
            sB.Append("id:");
            sB.Append(sticker.Id);
            sB.Append("\n\nОписание: ");
            sB.Append(sticker.Description);
            sB.Append("\n\nХэштэги: ");
            sB.Append(sticker.Hashtags);
            sB.Append("\n\nСтатус: ");

            if (sticker.IsPublished())
            {
                sB.Append(TextWithUrl("Опубликован", sticker.GetPublicChannelPost().VideoUrl));

                if (sticker.CheckingVideoStickers != null && sticker.CheckingVideoStickers.Count > 0)
                {
                    var checking = sticker.CheckingVideoStickers
                                    .FirstOrDefault(x => x.Status.Value == (int)VideoReviewEnum.APPROVAL);

                    if (checking != null && checking.ModeratorChat != null)
                    {
                        sB.Append($" (Одобрил @{checking.ModeratorChat.UserName})");
                    }
                }
            }
            else
            {
                if (sticker.CheckingVideoStickers == null || sticker.CheckingVideoStickers.Count == 0)
                {
                    sB.Append("_Не опубликован_");
                }
                else
                {
                    var checking = sticker.CheckingVideoStickers
                                    .FirstOrDefault(x => x.Status.Value == (int)VideoReviewEnum.REJECTION);

                    if (checking != null && checking.ModeratorChat != null)
                    {
                        sB.Append($" (Отклонил @{checking.ModeratorChat.UserName})");
                    }
                }
            }

            sB.Append($"\n\nДата загрузки: {sticker.DateAdd.ToString("dd.MM.yyyy HH:mm")}");
            sB.Append($"\n\nАвтор: @{sticker.AuthorChat.UserName} (id: {sticker.AuthorChatId})");
            sB.Append($"\n\n");
            sB.Append("------------------------------------------------------------------------");
            sB.Append($"\n\n📝 Изменить описание {BotCommands.EDIT_DESCRIPTION}{sticker.Id}");
            sB.Append($"\n\n✏️ Изменить хэштег {BotCommands.EDIT_HASHTAG}{sticker.Id}");
            sB.Append($"\n\n🗑 Удалить {BotCommands.REMOVE_VIDEO}{sticker.Id}");

            return sB.ToString();
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.STICKER_MANAGMENT;
        }
    }
}