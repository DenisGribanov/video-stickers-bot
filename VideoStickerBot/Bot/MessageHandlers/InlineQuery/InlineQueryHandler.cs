using VideoStickerBot.Bot.Handlers;
using VideoStickerBot.Bot.Interfaces;
using VideoStickerBot.Database;
using VideoStickerBot.Enums;
using VideoStickerBot.Services.Search;
using VideoStickerBot.Services.StickerSorted;
using VideoStickerBot.Services.TelegramIntegration;

namespace VideoStickerBot.Bot.MessageHandlers.InlineQuery
{
    public class InlineQueryHandler : BaseMessageHandler
    {
        private const int MAX_RESULT = 50;
        private readonly List<IStickerSort> StickerSortsStrategy = new List<IStickerSort>();
        private IEnumerable<VideoSticker> videoStickersSearchResult;

        private ISearchSticker searchSticker { get; set; }

        public InlineQueryHandler(IBotSubSystems botSubSystems) : base(botSubSystems)
        {
            StickerSortsStrategy.Add(new NewestSorted(botSubSystems.DataStore));
            StickerSortsStrategy.Add(new PopularSorted(botSubSystems.DataStore));
            StickerSortsStrategy.Add(new PersonSorted(botSubSystems.DataStore, CurrentUser.ChatId));
            StickerSortsStrategy.Add(new OwnSorted(botSubSystems.DataStore, CurrentUser.ChatId));
        }

        public override bool Match()
        {
            if (isMatchForTelegramUpdate.HasValue)
                return isMatchForTelegramUpdate.Value;

            isMatchForTelegramUpdate = TelegramUpdate.IsInlineQuery;

            return isMatchForTelegramUpdate.Value;
        }

        public override async Task Handle()
        {
            if (!Match()) return;

            int? offset = DigitParse(TelegramUpdate.InlineQueryOffset);
            int currentPage = offset.HasValue ? offset.Value : 0;

            searchSticker = new SearchSticker(GetStickerSortStrategy());
            videoStickersSearchResult = searchSticker.Search(TelegramUpdate.InlineQueryText).Where(x => x.IsPublished() && !x.Deleted);

            int searhResultCount = videoStickersSearchResult.Count();

            if (searhResultCount == 0) return;

            int indexStart = GetStartIndexRange(currentPage, MAX_RESULT);
            int rangeCount = GetRangeCount(indexStart, searhResultCount);
            int? nextPageIndex = GetNextPageIndex(currentPage, MAX_RESULT, searhResultCount);

            var result = videoStickersSearchResult.ToList().GetRange(indexStart, rangeCount).Take(MAX_RESULT);

            string nextOffset = nextPageIndex.HasValue ? nextPageIndex.ToString() : null;

            await Telegram.AnswerInlineQueryAsync(result.Select(x => ConvertToInlineResult(x)), TelegramUpdate.InlineQueryId, nextOffset);
        }

        private int GetStartIndexRange(int? currentPage, int pageSize)
        {
            if (!currentPage.HasValue)
            {
                return 0;
            }
            else
            {
                return currentPage.Value * pageSize;
            }
        }

        private int GetRangeCount(int indexStart, int arraySize)
        {
            return indexStart == 0 ? arraySize - indexStart : arraySize - indexStart - 1;
        }

        private int? GetNextPageIndex(int currentPageIndex, int pageSize, int arraySize)
        {
            if (currentPageIndex * pageSize + pageSize >= arraySize)
            {
                return null;
            }
            else
            {
                return currentPageIndex += 1;
            }
        }

        private static TelegramInlineQueryResultVideo ConvertToInlineResult(VideoSticker videoSticker)
        {
            string videoUrl = videoSticker.GetPublicChannelPost().VideoUrl;
            return new TelegramInlineQueryResultVideo(videoSticker.Id.ToString(),
                videoUrl, videoUrl, videoSticker.Hashtags, videoSticker.Description);
        }

        private IStickerSort GetStickerSortStrategy()
        {
            SortEnum defaultSort = SortEnum.POPULAR;

            var user = CurrentUser;

            IStickerSort strategySort = StickerSortsStrategy.Where(x => x.SortType == defaultSort).FirstOrDefault();

            if (user != null && user.SortedType != null)
            {
                strategySort = StickerSortsStrategy.Where(x => (int)x.SortType == user.SortedType).First();
            }

            return strategySort;
        }

        protected override BotState GetHandlerStateName()
        {
            return BotState.INLINE_QUERY;
        }
    }
}