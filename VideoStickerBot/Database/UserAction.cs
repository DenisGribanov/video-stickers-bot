using System;
using System.Collections.Generic;

namespace VideoStickerBot.Database
{
    public partial class UserAction
    {
        public long Id { get; set; }
        public string TelegramUpdate { get; set; } = null!;
        public string? UserState { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
