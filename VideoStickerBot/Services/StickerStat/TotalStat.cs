using VideoStickerBot.Services.DataStore;
using VideoStickerBot.Services.StickerStat.Models;

namespace VideoStickerBot.Services.StickerStat
{
    public class TotalStat : IStat
    {
        private static readonly Dictionary<long, VideoTotalStat> totalStat = new();
        private readonly IDataStore dataStore;

        public TotalStat(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public void Update(long stickerId)
        {
            if (!totalStat.ContainsKey(stickerId)) return;

            var stat = totalStat[stickerId];

            stat.TotalClick++;
        }

        public void Load()
        {
            var statisticksClick = dataStore.GetVideoStickersStats();

            foreach (var stat in statisticksClick)
            {
                if (totalStat.ContainsKey(stat.StickerId))
                    continue;

                int summClick = statisticksClick.Where(x => x.StickerId == stat.StickerId).Select(x => x.ClickCount).Sum();
                totalStat.Add(stat.StickerId, new VideoTotalStat() { TotalClick = summClick, VideoSticker = stat.Sticker });
            }
        }

        public List<VideoTotalStat> Get()
        {
            Load();
            return totalStat.Select(x => x.Value).ToList();
        }
    }
}