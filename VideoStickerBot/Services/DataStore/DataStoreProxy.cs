using NLog;
using System.Collections.Concurrent;
using VideoStickerBot.Database;

namespace VideoStickerBot.Services.DataStore
{
    public class DataStoreProxy : IDataStore
    {
        private readonly IDataStore DataStore;

        private static readonly List<VideoSticker> videoStickers = new List<VideoSticker>();

        private static readonly List<VideoStickersStat> videoStickersStats = new List<VideoStickersStat>();

        private static readonly ConcurrentDictionary<long, TgUser> Users = new();

        private static readonly List<Channel> channels = new List<Channel>();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DataStoreProxy(VideoStikersBotContext context)
        {
            DataStore = new DataStore(context);
        }

        public int AddVideoStat(VideoStickersStat videoSticker)
        {
            var result = DataStore.AddVideoStat(videoSticker);

            if (result > 0)
                videoStickersStats.Add(videoSticker);

            return result;
        }

        public int AddVideoSticker(VideoSticker videoSticker)
        {
            if (videoSticker == null)
                throw new ArgumentNullException(nameof(videoSticker));

            int resultSave = DataStore.AddVideoSticker(videoSticker);

            if (resultSave > 0)
            {
                videoStickers.Add(videoSticker);
                logger.Info($"AddVideoSticker fileId =  {videoSticker.FileId} | duration = {videoSticker.VideoDuration}");
            }

            return resultSave;

        }


        public List<VideoSticker> GetVideoStickers()
        {
            if (videoStickers.Count > 0)
                return videoStickers;

            foreach (var item in DataStore.GetVideoStickers())
            {
                videoStickers.Add(item);
            }

            return videoStickers;
        }

        public List<VideoStickersStat> GetVideoStickersStats()
        {
            if (videoStickersStats.Count > 0)
                return videoStickersStats;

            foreach (var item in GetVideoStickers())
            {
                foreach (var stat in item.VideoStickersStats)
                {
                    videoStickersStats.Add(stat);
                }

            }

            return videoStickersStats;
        }

        public int UpdateVideoSticker(VideoSticker videoStickers)
        {
            var cacheItem = GetStickerById(videoStickers.Id);

            if (cacheItem == null)
                throw new Exception($"VideoSticker id {videoStickers.Id} Not Found");


            return DataStore.UpdateVideoSticker(videoStickers);
        }

        public int UpdateVideoStickersStat(VideoStickersStat videoStickersStat)
        {
            return DataStore.UpdateVideoStickersStat(videoStickersStat);
        }

        public int AddUser(TgUser tgUser)
        {
            if (IsExistUsers(tgUser.ChatId))
            {
                return -1;
            }

            int saveResult = DataStore.AddUser(tgUser);

            if (saveResult > 0)
            {
                Users.TryAdd(tgUser.ChatId, tgUser);
                logger.Info($"AddUser Id = {tgUser.ChatId}, userName = {tgUser.UserName}");
            }

            return saveResult;
        }

        public int UpdateUser(TgUser tgUser)
        {
            if (IsExistUsers(tgUser.ChatId))
            {
                return DataStore.UpdateUser(tgUser);
            }
            else
            {
                return AddUser(tgUser);
            }
        }

        public List<TgUser> GetUsers()
        {
            if (!Users.IsEmpty)
                return Users.Select(x => x.Value).ToList();

            var users = DataStore.GetUsers();

            foreach (var user in users)
            {
                Users.TryAdd(user.ChatId, user);
            }

            return Users.Select(x => x.Value).ToList();
        }

        public bool IsExistUsers(long chatId)
        {
            if (Users.IsEmpty)
            {
                GetUsers();
            }

            return Users.ContainsKey(chatId);
        }


        public int AddCheckingVideo(CheckingVideoSticker checkingVideoSticker)
        {
            var res = DataStore.AddCheckingVideo(checkingVideoSticker);

            var sticker = GetStickerById(checkingVideoSticker.VideoStickerId);

            sticker.CheckingVideoStickers.Add(checkingVideoSticker);

            return res;
        }


        public int AddChannelPost(ChannelPost channelPost)
        {
            var resultSave = DataStore.AddChannelPost(channelPost);

            var sticker = GetStickerById(channelPost.VideoStickerId);

            sticker.ChannelPosts.Add(channelPost);

            return resultSave;
        }


        public List<Channel> GetChannels()
        {
            if (channels.Count > 0) return channels;

            foreach (var ch in DataStore.GetChannels())
            {
                channels.Add(ch);
            }

            return channels;
        }

        private VideoSticker GetStickerById(long id)
        {
            return videoStickers.First(x => x.Id == id);
        }
    }
}
