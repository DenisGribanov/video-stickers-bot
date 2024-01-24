using System;
using System.Collections.Generic;

namespace VideoStickerBot.Database
{
    public partial class ClickedInfo
    {
        public long Id { get; set; }
        public long StickerId { get; set; }
        public long UserChatId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? QueryText { get; set; }
    }
}
