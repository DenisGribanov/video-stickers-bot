using System.Collections.Concurrent;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot
{
    public class StateData : IStateData
    {
        private readonly IDataStore DataStore;

        private readonly ITelegramUpdateMessage TelegramUpdate;

        private static readonly ConcurrentDictionary<long, BotState> usersState = new();

        public TgUser? CurrentUser { get; private set; }

        public BotState? StateCurrentUser { get; private set; }

        public StateData(IDataStore dataStore, ITelegramUpdateMessage telegramUpdate)
        {
            DataStore = dataStore;
            TelegramUpdate = telegramUpdate;
        }

        public void LoadData()
        {
            StateCurrentUser = GetStateForCurrentUser();

            CurrentUser = GetUserInStore(TelegramUpdate.UserFromId);

            if (CurrentUser == null && TelegramUpdate.UserFromId > 0)
            {
                CurrentUser = RegisterUser(new TgUser
                {
                    ChatId = TelegramUpdate.UserFromId,
                    CreateData = DateTime.Now,
                    UserName = TelegramUpdate.Username,
                    UserRole = (int)UserRoleEnum.USER,
                    SortedType = (int)SortEnum.PERSON_RANKING
                });
            }
        }

        private BotState? GetStateForCurrentUser()
        {
            return GetCurrentState(TelegramUpdate.UserFromId);
        }

        private BotState? GetCurrentState(long userChatId)
        {
            return usersState.GetValueOrDefault(userChatId);
        }

        private TgUser? GetUserInStore(long chatId)
        {
            return DataStore.GetUsers().FirstOrDefault(x => x.ChatId == chatId);
        }

        private TgUser RegisterUser(TgUser user)
        {
            DataStore.AddUser(user);
            return user;
        }

        public BotState? UpdateState(BotState value)
        {
            if (!usersState.ContainsKey(CurrentUser.ChatId))
            {
                usersState.TryAdd(CurrentUser.ChatId, value);
            }
            else
            {
                usersState[CurrentUser.ChatId] = value;
            }

            return value;
        }
    }
}