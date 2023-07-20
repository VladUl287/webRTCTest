export type Message = {
    id: string,
    content: string,
    userId: number,
    chatId: string,
    date: string,
    edit: boolean
}

export type LastMessage = {
    date: string,
    content: string
}

export type CreateMessage = {
    chatId: string,
    content: string
}