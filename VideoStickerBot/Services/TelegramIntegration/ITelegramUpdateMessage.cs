using VideoStickerBot.Enums;

namespace VideoStickerBot.Services.TelegramIntegration
{
    public interface ITelegramUpdateMessage
    {
        string? Username { get; }

        long UserFromId { get; }

        bool IsInlineQuery { get; }

        bool IsChannelPost { get; }

        string? InlineQueryText { get; }

        string InlineQueryOffset { get; }

        string? MessageText { get; }

        int? MessageId { get; }
        int? ReplyMessageId { get; }

        bool IsChosenInlineResult { get; }
        string? ChosenInlineResultId { get; }
        public TelegramFileType? FileType { get; }

        string FileMimeType { get; }
        string? FileId { get; }
        string? FileUniqueId { get; }
        long? FileSize { get; }
        int? VideoDuration { get; }
        bool IsBotCommand { get; }
        bool IsCallBackQuery { get; }
        string? CallBackData { get; }
        string? InlineQueryId { get; }
        string? CallBackQueryId { get; }
        long? ChatId { get; }

        long? BotAddedToChatId { get; }
    }
}