namespace VideoStickerBot.Services.UserActionHistory
{
    public interface IUserActionHistory
    {
         Task Write(string userState, string update);
    }
}
