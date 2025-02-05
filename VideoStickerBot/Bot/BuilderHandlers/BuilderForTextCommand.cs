using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Bot.MessageHandlers;
using VideoStickerBot.Bot.MessageHandlers.TextCommand;
using VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd;
using VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd;

namespace VideoStickerBot.Bot.BuilderHandlers
{
    public class BuilderForTextCommand : BuilderHandlerBase
    {
        public BuilderForTextCommand(IBotSubSystems botSubSystems)
            : base(botSubSystems)
        {
            AddAll();
        }

        private void AddAll()
        {
            handlers.Add(CmdAddHandler());
            handlers.Add(CmdAllHandler());
            handlers.Add(CmdSettingsHandler());
            handlers.Add(CmdStartHandler());
            handlers.Add(CmdRemoveVideoHandler());
            handlers.Add(CmdStickerManagmentHandler());
            handlers.Add(CmdStickerDescriptionEditHandler());
            handlers.Add(CmdStickerHashTagEditHandler());
            handlers.Add(StickerDescriptionInputHandler());
            handlers.Add(StickerHashTagInputHandler());
            handlers.Add(InputVideoDescriptionHandler());
            handlers.Add(CmdCompilationHandler());
            handlers.Add(CmdCompilationByTextQueryHandler());
            handlers.Add(CmdHelpHandler());
            handlers.Add(CmdFreshHandler());
            handlers.Add(CmdBestHandler());
        }

        private IMessageHandler CmdAddHandler()
        {
            return new CmdAddHandler(botSubSystems);
        }

        private IMessageHandler CmdAllHandler()
        {
            return new CmdAllHandler(botSubSystems);
        }

        private IMessageHandler CmdSettingsHandler()
        {
            return new CmdSettingsHandler(botSubSystems);
        }

        private IMessageHandler CmdStartHandler()
        {
            return new CmdStartHandler(botSubSystems);
        }

        private IMessageHandler CmdRemoveVideoHandler()
        {
            return new CmdRemoveStickerHandler(botSubSystems);
        }

        private IMessageHandler CmdStickerManagmentHandler()
        {
            return new CmdStickerInfoHandler(botSubSystems);
        }

        private IMessageHandler CmdStickerDescriptionEditHandler()
        {
            return new CmdStickerDescriptionEditHandler(botSubSystems);
        }

        private IMessageHandler CmdStickerHashTagEditHandler()
        {
            return new CmdStickerHashTagEditHandler(botSubSystems);
        }

        private IMessageHandler StickerDescriptionInputHandler()
        {
            return new StickerDescriptionInputHandler(botSubSystems);
        }

        private IMessageHandler StickerHashTagInputHandler()
        {
            return new StickerHashTagInputHandler(botSubSystems);
        }

        private IMessageHandler InputVideoDescriptionHandler()
        {
            return new InputVideoDescriptionHandler(botSubSystems);
        }

        private IMessageHandler CmdCompilationHandler()
        {
            return new CmdCompilationHandler(botSubSystems);
        }

        private IMessageHandler CmdCompilationByTextQueryHandler()
        {
            return new CmdCompilationByTextQueryHandler(botSubSystems);
        }

        private IMessageHandler CmdHelpHandler()
        {
            return new CmdHelpHandler(botSubSystems);
        }

        private IMessageHandler CmdFreshHandler()
        {
            return new CmdFreshHandler(botSubSystems);
        }

        private IMessageHandler CmdBestHandler()
        {
            return new CmdBestHandler(botSubSystems);
        }
    }
}