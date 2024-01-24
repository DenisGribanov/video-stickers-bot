using VideoStickerBot.Database;
using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.StickerStat.Models;

namespace VideoStickerBot.Services.StickerStat
{
    public class PersonsStat : IStat
    {
        private static readonly Dictionary<long, List<UserVideoClickedInfo>> clicked = new();
        private readonly IDataStore dataStore;

        private readonly long UserChatId;

        public PersonsStat(IDataStore dataStore, long UserChatId)
        {
            this.dataStore = dataStore;
            this.UserChatId = UserChatId;
        }

        public void Update(long stickerId)
        {
            var sticker = dataStore.GetVideoStickers().Where(x => x.Id == stickerId).FirstOrDefault();

            if (sticker == null) return;


            if (!clicked.ContainsKey(UserChatId))
                clicked.Add(UserChatId, new List<UserVideoClickedInfo>());


            var sorted = clicked[UserChatId].Where(x => x.Video.Id == stickerId).FirstOrDefault();

            if (sorted != null && sorted.Video != null)
                sorted.UserClickCount++;
            else
                clicked[UserChatId].Add(new UserVideoClickedInfo { UserClickCount = 1, Video = sticker });


            var stat = dataStore.GetVideoStickersStats().Where(x => x.StickerId == stickerId && x.UserChatId == UserChatId).FirstOrDefault();
            if (stat != null)
            {
                stat.ClickCount++;
                dataStore.UpdateVideoStickersStat(stat);
            }
            else
            {
                dataStore.AddVideoStat(new VideoStickersStat
                {
                    ClickCount = 1,
                    UserChatId = UserChatId,
                    StickerId = stickerId,
                });
            }
        }

        public List<UserVideoClickedInfo> GetByUser()
        {
            if (clicked.ContainsKey(UserChatId))
                return clicked[UserChatId];
            else
                return dataStore.GetVideoStickers().Select(x => new UserVideoClickedInfo(x, 0)).ToList();

        }

        public void Load()
        {
            if (clicked.Count > 0) return;

            foreach (var stat in dataStore.GetVideoStickersStats())
            {
                if (!clicked.ContainsKey(stat.UserChatId))
                    clicked.Add(stat.UserChatId, new List<UserVideoClickedInfo>());


                clicked[stat.UserChatId].Add(new UserVideoClickedInfo { UserClickCount = stat.ClickCount, Video = stat.Sticker });

            }

        }

    }
}
