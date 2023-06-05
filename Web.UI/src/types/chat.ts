export type Chat = {
    id: string,
    name: string,
    image: string,
    message: LastMessage,
    unread: number
}

export type Message = {
    id: string,
    content: string,
    userId: number,
    date: string,
    edit: boolean
}

export type LastMessage = {
    date: string,
    content: string
}