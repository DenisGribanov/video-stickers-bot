namespace VideoStickerBot.Services.VideoResize
{
    public interface IVideoResize
    {
        Task<byte[]> ConvertToSquareAsync(MemoryStream memoryStream);

    }
}
