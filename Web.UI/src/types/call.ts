export type StartCall = {
    chatId: string,
    userId: number
}

export type JoinCall = {
    chatId: string,
    peerId: string
}

export type LeaveCall = JoinCall