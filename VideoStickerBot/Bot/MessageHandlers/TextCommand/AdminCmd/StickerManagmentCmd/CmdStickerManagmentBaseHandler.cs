using System.Collections.Concurrent;
using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;

namespace VideoStickerBot.Bot.MessageHandlers.TextCommand.AdminCmd.StickerManagmentCmd
{
    public abstract class CmdStickerManagmentBaseHandler : BaseMessageHandler
    {
        protected long? stickerId;
        protected VideoSticker sticker;
        private static readonly ConcurrentDictionary<BotState, ConcurrentDictionary<long, long>> states = new ConcurrentDictionary<BotState, ConcurrentDictionary<long, long>> { };

        public CmdStickerManagmentBaseHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
        }

        protected VideoSticker GetSticker(long id)
        {
            return DataStore.GetVideoStickers().Where(x => x.Id == id).FirstOrDefault();
        }

        protected void SaveVideoStickerIdFromState(BotState botState, long stickerId)
        {
            if (!states.ContainsKey(botState))
            {
                states.TryAdd(botState, new ConcurrentDictionary<long, long>());
            }

            var dict = states[botState];

            if (!dict.ContainsKey(CurrentUser.ChatId))
            {
                dict.TryAdd(CurrentUser.ChatId, stickerId);
            }
            else
            {
                dict[CurrentUser.ChatId] = stickerId;
            }
        }

        protected long? GetVideoStickerIdFromState(BotState botState)
        {
            if (!states.ContainsKey(botState))
            {
                return null;
            }

            var dict = states[botState];

            return dict.ContainsKey(CurrentUser.ChatId) ? dict[CurrentUser.ChatId] : null;
        }

        protected bool ResetFromState(BotState botState)
        {
            if (!states.ContainsKey(botState))
            {
                return false;
            }

            var dict = states[botState];

            return dict.TryRemove(CurrentUser.ChatId, out _);
        }
    }
}