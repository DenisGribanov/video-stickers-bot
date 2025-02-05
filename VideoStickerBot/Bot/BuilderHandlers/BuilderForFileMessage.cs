using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.MessageHandlers;
using VideoStickerBot.Bot.MessageHandlers.FileMessage;

namespace VideoStickerBot.Bot.BuilderHandlers
{
    public class BuilderForFileMessage : BuilderHandlerBase
    {
        public BuilderForFileMessage(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
            AddAll();
        }

        private void AddAll()
        {
            handlers.Add(ReceivedVideoFileHandler());
            handlers.Add(ReceivedVideoNoteFileHandler());
            handlers.Add(ReceivedDocumentFileHandler());
        }

        private IMessageHandler ReceivedVideoFileHandler()
        {
            return new ReceivedVideoFileHandler(botSubSystems);
        }

        private IMessageHandler ReceivedVideoNoteFileHandler()
        {
            return new ReceivedVideoNoteFileHandler(botSubSystems);
        }

        private IMessageHandler ReceivedDocumentFileHandler()
        {
            return new ReceivedDocumentFileHandler(botSubSystems);
        }
    }
}