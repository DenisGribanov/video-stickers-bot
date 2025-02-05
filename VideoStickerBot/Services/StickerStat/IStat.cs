namespace VideoStickerBot.Services.StickerStat
{
    public interface IStat
    {
        void Load();

        void Update(long stickerId);
    }
}