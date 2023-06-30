export type Chat = {
    id: string,
    name: string,
    image: string,
    date: string,
    lastRead: string,
    lastMessage: LastMessage,
    unread: number
}

export type Message = {
    id: string,
    content: string,
    userId: string,
    chatId: string,
    date: string,
    edit: boolean
}

export type LastMessage = {
    date: string,
    content: string
}