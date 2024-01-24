using VideoStickerBot.Database;

namespace VideoStickerBot.Services.DataStore
{
    public interface IDataStore 
    {
        List<VideoSticker> GetVideoStickers();

        List<VideoStickersStat> GetVideoStickersStats();

        int AddVideoSticker(VideoSticker videoStickers);

        int UpdateVideoSticker(VideoSticker videoStickers);

        int AddVideoStat(VideoStickersStat videoStickers);

        int UpdateVideoStickersStat(VideoStickersStat videoStickersStat);

        int AddUser(TgUser tgUser);

        int UpdateUser(TgUser tgUser);
        List<TgUser> GetUsers();
        bool IsExistUsers(long chatId);
        int AddCheckingVideo(CheckingVideoSticker checkingVideoSticker);
        int AddChannelPost(ChannelPost channelPost);
        List<Channel> GetChannels();
    }
}
