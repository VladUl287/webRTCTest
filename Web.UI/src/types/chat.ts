import type { LastMessage } from "./message"

export type Chat = {
    id: string,
    name: string,
    image: string,
    date: string,
    lastRead: string,
    lastMessage: LastMessage,
    unread: number
}

export type ChatUser = {
    userId: number,
    lastRead: string
}

export enum ChatType {
    monolog,
    dialog
}

export type ChatUpdate = {
    chatId: string,
    lastRead: string
}