using Telegram.Bot;
using VideoStickerBot.Database;
using Telegram.Bot.Types;
using NLog;
using System.Text;
using Telegram.Bot.Types.InlineQueryResults;
using RestSharp;
using System.Collections.Concurrent;
using VideoStickerBot.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using VideoStickerBot.Constants;

namespace VideoStickerBot.Services.TelegramIntegration
{
    public class TelegramAdapter : ITelegram
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ITelegramBotClient BotClient;

        public TelegramAdapter(ITelegramBotClient botClient)
        {
            BotClient = botClient;
        }


        public async Task AnswerInlineQueryAsync(IEnumerable<TelegramInlineQueryResultVideo> result, string inlineQueryId, string? nextOffset = null)
        {
            if (result == null || result.Count() == 0) return;

            await BotClient.AnswerInlineQueryAsync(inlineQueryId,
                result.Select(x => convertToInlineQueryResultVideo(x)),
                0,
                true, nextOffset, "Все кружочки", BotCommands.COMPILATION.Replace("/", ""));
        }


        public async Task<ITelegramUpdateMessage> SendTextMessage(string text,
            long destanationChatId,
            int? replyMessageId = null,
            Dictionary<string, string>? inlineData = null)
        {
            var msg = await BotClient.SendTextMessageAsync(destanationChatId, text,
                Telegram.Bot.Types.Enums.ParseMode.Markdown, null,
                false, false, false, replyMessageId, true,
                convertToInlineKeyboardMarkup(inlineData));

            return new TelegramUpdateMessageAdapter(msg);
        }



        public async Task<ITelegramUpdateMessage> ForwardMessage(long destanationChatId, long fromChatId, int messageId)
        {
            var msg = await BotClient.ForwardMessageAsync(destanationChatId, fromChatId, messageId, false, false);
            return new TelegramUpdateMessageAdapter(msg);
        }

        public async Task AnswerCallbackQuery(string callBackQueryId, string text)
        {
            try
            {
                await BotClient.AnswerCallbackQueryAsync(callBackQueryId, text, true);
            }
            catch
            {

            }

        }

        public async Task EditMessageReplyMarkup(int messageId,
            long destinationChatId,
            string? text = null,
            Dictionary<string, string>? inlineData = null)
        {
            try
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = convertToInlineKeyboardMarkup(inlineData);

                if (text != null)
                {
                    await BotClient.EditMessageTextAsync(destinationChatId, messageId, text,
                        Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        null, false, convertToInlineKeyboardMarkup(inlineData));
                }
                else
                {
                    await BotClient.EditMessageReplyMarkupAsync(destinationChatId, messageId, inlineKeyboardMarkup);
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        public async Task<MemoryStream> GetFile(string fileId, MemoryStream destination)
        {
            await BotClient.GetInfoAndDownloadFileAsync(fileId, destination);

            return destination;
        }

        public async Task<ITelegramUpdateMessage> SendVideoNote(MemoryStream memoryStream, long destinationChatId)
        {
            var message = await BotClient.SendVideoNoteAsync(destinationChatId,
                new Telegram.Bot.Types.InputFiles.InputTelegramFile(memoryStream));

            return new TelegramUpdateMessageAdapter(message);
        }


        public async Task<ITelegramUpdateMessage> SendVideoNote(string fileId, long destanationChatId)
        {
            var msg = await BotClient.SendVideoNoteAsync(destanationChatId,
                new Telegram.Bot.Types.InputFiles.InputTelegramFile(fileId));
            return new TelegramUpdateMessageAdapter(msg);
        }

        public async Task<ITelegramUpdateMessage> SendVideo(string fileId, long destanationChatId, List<List<KeyValuePair<string, string>>> inlineData = null)
        {
            if (inlineData == null)
            {
                return new TelegramUpdateMessageAdapter(await BotClient.SendVideoAsync(destanationChatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(fileId)));
            }

            List<List<InlineKeyboardButton>> buttons = new();

            foreach (var row in inlineData)
            {
                List<InlineKeyboardButton> lineButtons = new();

                foreach (var data in row)
                {
                    lineButtons.Add(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(data.Key, " " + data.Value));
                }

                buttons.Add(lineButtons);
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = new(buttons);

            var msg = await BotClient.SendVideoAsync(destanationChatId,
                new Telegram.Bot.Types.InputFiles.InputOnlineFile(fileId), replyMarkup: inlineKeyboardMarkup);
            return new TelegramUpdateMessageAdapter(msg);
        }

        public async Task DeleteMessage(long chatId, int messageId)
        {
            try
            {
                await BotClient.DeleteMessageAsync(chatId, messageId);
            }
            catch (Exception ex) { logger.Error(ex); }

        }

        public async Task<ITelegramUpdateMessage> SendTextMessage(string text, long destanationChatId, List<List<KeyValuePair<string, string>>> inlineData)
        {
            List<List<InlineKeyboardButton>> buttons = new();

            foreach (var row in inlineData)
            {
                List<InlineKeyboardButton> lineButtons = new();

                foreach (var data in row)
                {
                    lineButtons.Add(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(data.Key, " " + data.Value));
                }

                buttons.Add(lineButtons);
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = new(buttons);

            var msg = await BotClient.SendTextMessageAsync(destanationChatId, text, Telegram.Bot.Types.Enums.ParseMode.Markdown, null,
                 false, false, false, null, false, inlineKeyboardMarkup);
            return new TelegramUpdateMessageAdapter(msg);
        }




        private InlineKeyboardMarkup convertToInlineKeyboardMarkup(Dictionary<string, string> inlineData)
        {
            if (inlineData == null || inlineData.Count == 0) return null;

            var buttons = new List<InlineKeyboardButton>();
            foreach (var pair in inlineData)
            {
                InlineKeyboardButton button1 = new(pair.Key)
                {
                    CallbackData = pair.Value
                };

                buttons.Add(button1);
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = new(buttons);



            return inlineKeyboardMarkup;
        }

        private InlineQueryResultVideo convertToInlineQueryResultVideo(TelegramInlineQueryResultVideo resultVideo)
        {
            string videoUrl = resultVideo.VideoUrl;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(resultVideo.Caption);
            stringBuilder.AppendLine($" (id:{resultVideo.UniqueId})");

            var inline = new InlineQueryResultVideo(resultVideo.UniqueId,
                                                videoUrl,
                                                videoUrl,
                                                resultVideo.Title);
            //inline.Caption = resultVideo.Caption;
            inline.Description = stringBuilder.ToString();

            return inline;
        }

    }
}
