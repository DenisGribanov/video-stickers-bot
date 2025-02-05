using VideoStickerBot.Database;

namespace VideoStickerBot.Services.UserActionHistory
{
    public class UserActionHistoryImpl : IUserActionHistory
    {
        private readonly VideoStikersBotContext context;

        public UserActionHistoryImpl(VideoStikersBotContext context)
        {
            this.context = context;
        }

        public async Task Write(string userState, string update)
        {
            try
            {
                UserAction userAction = new UserAction
                {
                    TelegramUpdate = update,
                    Timestamp = DateTime.Now,
                    UserState = string.IsNullOrEmpty(userState) ? string.Empty : userState,
                };

                context.Entry(userAction).State = Microsoft.EntityFrameworkCore.EntityState.Added;

                await context.SaveChangesAsync();
            }
            catch
            {
            }
        }
    }
}