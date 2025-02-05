using RestSharp;

namespace VideoStickerBot.Services.VideoResize
{
    public class VideoResizeApi : IVideoResize
    {
        private readonly RestClient restClient = new RestClient();
        private readonly RestRequest restRequest;

        public VideoResizeApi(string apiUrl)
        {
            restRequest = new RestRequest(apiUrl, Method.Post);
        }

        public async Task<byte[]> ConvertToSquareAsync(MemoryStream sourceVideo)
        {
            restRequest.AddFile("file", sourceVideo.ToArray(), null, ContentType.Binary, null);
            var resp = await restClient.ExecuteAsync(restRequest);

            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Ошибка при обработке видео!");
            }

            return resp.RawBytes;
        }
    }
}