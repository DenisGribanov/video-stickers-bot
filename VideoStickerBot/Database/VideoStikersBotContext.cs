using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VideoStickerBot.Database
{
    public partial class VideoStikersBotContext : DbContext
    {
        public VideoStikersBotContext()
        {
        }

        public VideoStikersBotContext(DbContextOptions<VideoStikersBotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Channel> Channels { get; set; } = null!;
        public virtual DbSet<ChannelPost> ChannelPosts { get; set; } = null!;
        public virtual DbSet<CheckingVideoSticker> CheckingVideoStickers { get; set; } = null!;
        public virtual DbSet<ClickedInfo> ClickedInfos { get; set; } = null!;
        public virtual DbSet<TgUser> TgUsers { get; set; } = null!;
        public virtual DbSet<UserAction> UserActions { get; set; } = null!;
        public virtual DbSet<VideoSticker> VideoStickers { get; set; } = null!;
        public virtual DbSet<VideoStickersStat> VideoStickersStats { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=gribanov93.ru;Port=5432;Database=VideoStikersBot;Username=postgres;Password=vHrRkiSIK0wj");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Channel>(entity =>
            {
                entity.ToTable("channels");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CanDeleteMessages).HasColumnName("can_delete_messages");

                entity.Property(e => e.CanEditMessages).HasColumnName("can_edit_messages");

                entity.Property(e => e.CanPostMessages).HasColumnName("can_post_messages");

                entity.Property(e => e.ChannelType).HasColumnName("channel_type");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnType("character varying")
                    .HasColumnName("title");
            });

            modelBuilder.Entity<ChannelPost>(entity =>
            {
                entity.ToTable("channel_post");

                entity.HasIndex(e => e.Id, "channel_post_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChannelId).HasColumnName("channel_id");

                entity.Property(e => e.DateAdd)
                    .HasColumnType("timestamp(0) without time zone")
                    .HasColumnName("date_add")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.MessageId).HasColumnName("message_id");

                entity.Property(e => e.PostText).HasColumnName("post_text");

                entity.Property(e => e.ReplyMessageId).HasColumnName("reply_message_id");

                entity.Property(e => e.VideoStickerId).HasColumnName("video_sticker_id");

                entity.Property(e => e.VideoUrl).HasColumnName("video_url");

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.ChannelPosts)
                    .HasForeignKey(d => d.ChannelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("channel_channels_post_fk");

                entity.HasOne(d => d.VideoSticker)
                    .WithMany(p => p.ChannelPosts)
                    .HasForeignKey(d => d.VideoStickerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("channel_post_fk");
            });

            modelBuilder.Entity<CheckingVideoSticker>(entity =>
            {
                entity.ToTable("checking_video_stickers");

                entity.HasIndex(e => e.Id, "checking_video_stickers_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateAdd)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_add");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ModeratorChatId).HasColumnName("moderator_chat_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StatusUpdateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("status_update_time");

                entity.Property(e => e.VideoStickerId).HasColumnName("video_sticker_id");

                entity.HasOne(d => d.ModeratorChat)
                    .WithMany(p => p.CheckingVideoStickers)
                    .HasForeignKey(d => d.ModeratorChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checking_video_stickers_tg_users_chat_id_fk");

                entity.HasOne(d => d.VideoSticker)
                    .WithMany(p => p.CheckingVideoStickers)
                    .HasForeignKey(d => d.VideoStickerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checking_video_stickers_video_stickers_id_fk");
            });

            modelBuilder.Entity<ClickedInfo>(entity =>
            {
                entity.ToTable("clicked_info");

                entity.HasIndex(e => e.Id, "clicked_info_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.QueryText).HasColumnName("query_text");

                entity.Property(e => e.StickerId).HasColumnName("sticker_id");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("timestamp");

                entity.Property(e => e.UserChatId).HasColumnName("user_chat_id");
            });

            modelBuilder.Entity<TgUser>(entity =>
            {
                entity.HasKey(e => e.ChatId)
                    .HasName("tg_users_pk");

                entity.ToTable("tg_users");

                entity.HasIndex(e => e.ChatId, "tg_users_chat_id_uindex")
                    .IsUnique();

                entity.Property(e => e.ChatId)
                    .ValueGeneratedNever()
                    .HasColumnName("chat_id");

                entity.Property(e => e.CreateData)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_data")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(500)
                    .HasColumnName("first_name");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .HasColumnName("language_code");

                entity.Property(e => e.LastName)
                    .HasMaxLength(500)
                    .HasColumnName("last_name");

                entity.Property(e => e.SortedType).HasColumnName("sorted_type");

                entity.Property(e => e.UploadDisabled).HasColumnName("upload_disabled");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .HasColumnName("user_name");

                entity.Property(e => e.UserRole).HasColumnName("user_role");
            });

            modelBuilder.Entity<UserAction>(entity =>
            {
                entity.ToTable("user_actions");

                entity.HasIndex(e => e.Id, "user_actions_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TelegramUpdate)
                    .HasColumnType("jsonb")
                    .HasColumnName("telegram_update");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("timestamp");

                entity.Property(e => e.UserState).HasColumnName("user_state");
            });

            modelBuilder.Entity<VideoSticker>(entity =>
            {
                entity.ToTable("video_stickers");

                entity.HasComment("видео стикеры для бота");

                entity.HasIndex(e => e.Id, "video_stickers_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AuthorChatId).HasColumnName("author_chat_id");

                entity.Property(e => e.DateAdd)
                    .HasColumnType("timestamp(0) without time zone")
                    .HasColumnName("date_add")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("delete_date");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.DeletedDescription).HasColumnName("deleted_description");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FileId).HasColumnName("file_id");

                entity.Property(e => e.FileUniqueId).HasColumnName("file_unique_id");

                entity.Property(e => e.Hashtags).HasColumnName("hashtags");

                entity.Property(e => e.MessageId).HasColumnName("message_id");

                entity.Property(e => e.VideoDuration).HasColumnName("video_duration");

                entity.HasOne(d => d.AuthorChat)
                    .WithMany(p => p.VideoStickers)
                    .HasForeignKey(d => d.AuthorChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("video_stickers_fk");
            });

            modelBuilder.Entity<VideoStickersStat>(entity =>
            {
                entity.ToTable("video_stickers_stat");

                entity.HasIndex(e => e.Id, "video_stickers_stat_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.StickerId, "video_stickers_stat_sticker_id_index");

                entity.HasIndex(e => e.UserChatId, "video_stickers_stat_user_chat_id_index");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClickCount).HasColumnName("click_count");

                entity.Property(e => e.StickerId).HasColumnName("sticker_id");

                entity.Property(e => e.UserChatId).HasColumnName("user_chat_id");

                entity.HasOne(d => d.Sticker)
                    .WithMany(p => p.VideoStickersStats)
                    .HasForeignKey(d => d.StickerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("video_stickers_stat_video_stickers_id_fk");

                entity.HasOne(d => d.UserChat)
                    .WithMany(p => p.VideoStickersStats)
                    .HasForeignKey(d => d.UserChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("video_stickers_stat_tg_users_chat_id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
