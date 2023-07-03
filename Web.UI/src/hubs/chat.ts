import { useAuthStore } from './../stores/auth';
import { HubConnectionBuilder } from "@microsoft/signalr"

const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:7010/hubs/chat", {
        accessTokenFactory: async () => {
            const { getUser } = useAuthStore()
            const user = await getUser()

            return user?.access_token || ''
        }
    })
    // .withHubProtocol(new signalRMsgpack.MessagePackHubProtocol())
    .build()

export const sendMessage = (body: { chatId: string, content: string }) => connection.send('sendMessage', body)

export const onSendMessage = (callback: (...args: any[]) => void) => connection.on('sendMessage', callback)

export const сhatCreated = (chatId: string) => connection.send('chatCreated', chatId)

export const onChatCreated = (callback: (...args: any[]) => void) => connection.on('chatCreated', callback)

export const onChatUpdate = (callback: (...args: any[]) => void) => connection.on('updateChat', callback)

export const calling = (body: { chatId: string }) => connection.invoke('calling', body)

export const onCalling = (callback: (...args: any[]) => void) => connection.on('calling', callback)

export const joinCall = (body: { chatId: string, peerUserId: string }) => connection.send('joinCall', body)

export const onJoinCall = (callback: (...args: any[]) => void) => connection.on('joinCall', callback)

export const leaveCall = (body: { peerId: string, chatId: string, userId: number }) =>
    connection.send('leaveCall', body)

export const onLeaveCall = (callback: (...args: any[]) => void) => connection.on('leaveCall', callback)

export default connection