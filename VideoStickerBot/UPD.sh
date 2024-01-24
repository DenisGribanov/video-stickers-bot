#!/bin/bash

cd ~/gitlab/video-stickers-bot/

git stash
git stash drop

PULL_RESULT=$(git pull)

Already="Already up to date."

cd ~

curl -X POST \
 -H 'Content-Type: application/json' \
 -d '{"chat_id": "119576696", "text": "pull commit video-stickers-bot", }' \
 https://api.telegram.org/bot826030592:AAHaOoumkfzbRZOugZ2nUYNvqqjdJBdpxtM/sendMessage

docker container stop video_sticker_bot

docker container rm video_sticker_bot

docker image rm video_sticker_bot:v.0.1

cp -f ~/gitlab/video-stickers-bot/VideoStickerBot/Dockerfile ~/gitlab/video-stickers-bot/VideoStickerBot/Dockerfile

docker build -t video_sticker_bot:v.0.1 gitlab/video-stickers-bot/VideoStickerBot

docker run --name video_sticker_bot -d -p 8586:8080 video_sticker_bot:v.0.1
