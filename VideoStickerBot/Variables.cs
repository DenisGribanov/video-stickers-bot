namespace VideoStickerBot
{
    public class Variables
    {
        private static Variables instance;

        public const string TELESCOPE_URL = "https://telesco.pe";
        public const string T_ME_URL = "https://t.me";
        public long BOT_OWNER_CHAT_ID { get; private set; }
        public string BOT_DOMAIN_NAME { get; private set; }
        public long BOT_CHAT_ID { get; private set; }
        public string BOT_TOKEN { get; private set; }

        public string CACHE_FOLDER { get; private set; }

        public string PUBLIC_CHANNEL_URL { get; private set; }

        public string VIDEO_HELP_FILE_ID { get; private set; }

        public string SUPPORT_CHAT { get; private set; }

        private Variables()
        {
        }

        private void Init(IConfiguration configuration)
        {
            BOT_TOKEN = configuration["VIDEOSTICK_BOT_TOKEN"];
            BOT_OWNER_CHAT_ID = Convert.ToInt64(configuration["BOT_OWNER_CHAT_ID"]);
            BOT_CHAT_ID = Convert.ToInt64(configuration["BOT_CHAT_ID"]);
            BOT_DOMAIN_NAME = configuration["BOT_DOMAIN_NAME"];
            CACHE_FOLDER = configuration["CACHE_FOLDER"];
            PUBLIC_CHANNEL_URL = configuration["PUBLIC_CHANNEL_URL"];
            VIDEO_HELP_FILE_ID = configuration["VIDEO_HELP_FILE_ID"];
            SUPPORT_CHAT = configuration["SUPPORT_CHAT"];
        }

        public static Variables InitInstance(IConfiguration configuration)
        {
            if (instance == null)
            {
                instance = new Variables();
                instance.Init(configuration);
            }

            return instance;
        }

        public static Variables GetInstance()
        {
            return instance;
        }
    }
}