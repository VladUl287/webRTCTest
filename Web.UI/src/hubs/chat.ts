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

export const onSendMessage = (callback: (...args: any[]) => void) => connection.on('sendMessage', callback)

export const onChatCreate = (callback: (...args: any[]) => void) => connection.on('chatCreated', callback)

export const onChatUpdate = (callback: (...args: any[]) => void) => connection.on('updateChat', callback)

export const onCalling = (callback: (...args: any[]) => void) => connection.on('calling', callback)

export const onJoinCall = (callback: (...args: any[]) => void) => connection.on('joinCall', callback)

export const onLeaveCall = (callback: (...args: any[]) => void) => connection.on('leaveCall', callback)

export default connection