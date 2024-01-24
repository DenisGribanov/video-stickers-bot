using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Constants;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd
{
    public class CmdRemoveStickerHandler : CmdStickerManagmentBaseHandler
    {
        public CmdRemoveStickerHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsBotCommand
                                        && TelegramUpdate.MessageText.Contains(BotCommands.REMOVE_VIDEO)
                                        && CurrentUser.UserIsAdmin();

            return isMatchForTelegramUpdate.Value;
        }

        public async override Task Handle()
        {
            if (!Match()) return;

            stickerId = DigitParse(TelegramUpdate.MessageText);
            
            if (stickerId == null)
            {
                await Telegram.SendTextMessage("Не удалось распознать команду." +
                    "\n\nЧто бы удалить стикер введите команду /remove id_стикера", CurrentUser.ChatId);
                return;
            }

            sticker = GetSticker(stickerId.Value);
            if (sticker == null)
            {
                await Telegram.SendTextMessage($"Не удалось найти видео с id = {stickerId}", CurrentUser.ChatId);
                return;
            }

            await DeleteChannelPost(sticker);

            DeleteFromDataStore(sticker);

            await Telegram.SendTextMessage($"Видео успешно удалено", CurrentUser.ChatId);

        }

        private void DeleteFromDataStore(VideoSticker sticker)
        {
            sticker.Deleted = true;
            sticker.DeletedDescription = $"Удалено пользователем {TelegramUpdate.Username}";
            sticker.DeleteDate = DateTime.Now;
            DataStore.UpdateVideoSticker(sticker);

        }

        private async Task DeleteChannelPost(VideoSticker sticker)
        {
            foreach (var post in sticker.ChannelPosts)
            {
                await Telegram.DeleteMessage(Convert.ToInt64(post.ChannelId), post.MessageId);

                if (post.ReplyMessageId.HasValue)
                {
                    await Telegram.DeleteMessage(Convert.ToInt64(post.ChannelId), post.ReplyMessageId.Value);
                }

                logger.Info($"DeleteChannelPost id {post.MessageId} Success");
            }
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.REMOVE_VIDEO;
        }
    }
}
