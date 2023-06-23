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

export default connection