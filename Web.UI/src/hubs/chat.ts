import { HubConnectionBuilder } from "@microsoft/signalr"
import { useAuthStore } from '@/stores/auth'
import type { JoinCall, LeaveCall, StartCall } from '@/types/call'
import type { Message } from '@/types/message'
import type { ChatUpdate } from "@/types/chat"

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

const SendMessageMethod = 'SendMessage'
export const sendMessage = (body: { chatId: string, content: string }) => connection.send(SendMessageMethod, body)
export const onSendMessage = (callback: (result: Message) => void) => connection.on(SendMessageMethod, callback)

const ChatCreatedMethod = 'ChatCreated'
export const sendChatCreated = (chatId: string) => connection.send(ChatCreatedMethod, chatId)
export const onChatCreated = (callback: (chatId: string) => void) => connection.on(ChatCreatedMethod, callback)

const ChatUpdateMethod = 'UpdateChat'
export const updateChat = (body: ChatUpdate) => connection.send(ChatUpdateMethod, body)
export const onUpdateChat = (callback: (chatId: string) => void) => connection.on(ChatUpdateMethod, callback)

const StartCallMethod = 'StartCall'
export const sendStartCall = (body: StartCall) => connection.send(StartCallMethod, body)
export const onStartCall = (callback: (result: StartCall) => void) => connection.on(StartCallMethod, callback)

const JoinCallMethod = 'JoinCall'
export const sendJoinCall = (body: JoinCall) => connection.send(JoinCallMethod, body)
export const onJoinCall = (callback: (result: JoinCall) => void) => connection.on(JoinCallMethod, callback)

const LeaveCallMethod = 'LeaveCall'
export const sendLeaveCall = (body: LeaveCall) => connection.send(LeaveCallMethod, body)
export const onLeaveCall = (callback: (result: LeaveCall) => void) => connection.on(LeaveCallMethod, callback)

const EndCallMethod = 'EndCall'
export const sendEndCall = (chatId: string) => connection.send(EndCallMethod, chatId)
export const onEndCall = (callback: (chatId: string) => void) => connection.on(EndCallMethod, callback)

export default connection