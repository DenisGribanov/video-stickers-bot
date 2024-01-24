using Microsoft.EntityFrameworkCore;
using VideoStickerBot.Database;


namespace VideoStickerBot.Services.DataStore
{
    public class DataStore : IDataStore
    {
        private readonly VideoStikersBotContext context;

        public DataStore(VideoStikersBotContext context)
        {
            this.context = context;
        }

        public int AddVideoStat(VideoStickersStat stat)
        {
            context.Entry(stat).State = EntityState.Added;
            return context.SaveChanges();
        }

        public int AddVideoSticker(VideoSticker videoStickers)
        {
            context.Entry(videoStickers).State = EntityState.Added;

            return context.SaveChanges();
        }


        public List<VideoSticker> GetVideoStickers()
        {
            return context.VideoStickers.Where(x => !x.Deleted)
                .Include(x => x.ChannelPosts)
                .ThenInclude(x => x.Channel)
                .Include(x => x.VideoStickersStats)
                .Include(x => x.CheckingVideoStickers)
                .ThenInclude(x => x.ModeratorChat)
                .ToList();

        }

        public List<VideoStickersStat> GetVideoStickersStats()
        {
            return context.VideoStickersStats.Include(x => x.Sticker).ToList();
        }

        public int UpdateVideoSticker(VideoSticker sticker)
        {
            context.Entry(sticker).State = EntityState.Modified;
            return context.SaveChanges();
        }

        public int UpdateVideoStickersStat(VideoStickersStat stat)
        {
            context.Entry(stat).State = EntityState.Modified;
            return context.SaveChanges();
        }


        public int AddUser(TgUser tgUser)
        {
            context.Entry(tgUser).State = EntityState.Added;
            return context.SaveChanges();
        }

        public int UpdateUser(TgUser tgUser)
        {
            context.Entry(tgUser).State = EntityState.Modified;
            return context.SaveChanges();
        }

        public List<TgUser> GetUsers()
        {
            return context.TgUsers.ToList();
        }


        public bool IsExistUsers(long chatId)
        {
            return context.TgUsers.Any(x => x.ChatId == chatId);
        }

        public int AddCheckingVideo(CheckingVideoSticker checkingVideoSticker)
        {
            context.Entry<CheckingVideoSticker>(checkingVideoSticker).State = EntityState.Added;
            return context.SaveChanges();
        }


        public int AddChannelPost(ChannelPost channelPost)
        {
            context.Entry(channelPost).State = EntityState.Added;
            var result = context.SaveChanges();
            return result;
        }


        public List<Channel> GetChannels()
        {
            return context.Channels.Where(x => !x.Deleted).ToList();
        }

    }
}
