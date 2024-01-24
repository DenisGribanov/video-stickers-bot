namespace VideoStickerBot.Enums
{
    public enum SortEnum
    {
        /// <summary>
        /// Персональное ранжирование. Самые популярные для тек. пользователя (то что он выбирал чаще всего)
        /// </summary>
        PERSON_RANKING = 1,
        /// <summary>
        /// По дате добавления. Самые свежие
        /// </summary>
        NEWEST = 2,
        /// <summary>
        /// Популярные на осонове статистики всех пользователей
        /// </summary>
        POPULAR = 3,

        /// <summary>
        /// Свои будут вверху списка
        /// </summary>
        OWN = 4,
    }
}
