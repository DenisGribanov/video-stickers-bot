namespace VideoStickerBot.Services.TelegramIntegration
{
    public interface ITelegram
    {
        Task<MemoryStream> GetFile(string fileId, MemoryStream destination);

        Task AnswerInlineQueryAsync(IEnumerable<TelegramInlineQueryResultVideo> result, string inlineQueryId, string? nextOffset = null);

        Task<ITelegramUpdateMessage> SendTextMessage(string text, long destanationChatId, int? replyMessageId = null, Dictionary<string, string>? inlineData = null);

        Task EditMessageReplyMarkup(int messageId, long destinationChatId, string? text, Dictionary<string, string>? inlineData = null);

        Task<ITelegramUpdateMessage> SendVideoNote(MemoryStream video, long destinationChatId);

        Task<ITelegramUpdateMessage> SendVideoNote(string fileId, long destinationChatId);

        Task AnswerCallbackQuery(string callBackQueryId, string text);

        Task DeleteMessage(long chatId, int messageId);

        Task<ITelegramUpdateMessage> ForwardMessage(long destanationChatId, long fromChatId, int messageId);

        Task<ITelegramUpdateMessage> SendTextMessage(string text, long destanationChatId, List<List<KeyValuePair<string, string>>> inlineData);

        Task<ITelegramUpdateMessage> SendVideo(string fileId, long userFromId, List<List<KeyValuePair<string, string>>> inlineData = null);
    }
}