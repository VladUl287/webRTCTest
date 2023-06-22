import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr"

class HubClient {
    private static instance: HubClient | undefined

    public static get Instance() {
        return HubClient.instance
    }

    static async build(token: string): Promise<void> {

        if (HubClient.instance) throw new Error('Already created')

        const connection = new HubConnectionBuilder()
            .withUrl("https://localhost:7010/hubs/chat", {
                accessTokenFactory: () => token
            })
            // .withHubProtocol(new signalRMsgpack.MessagePackHubProtocol())
            .build()

        HubClient.instance = new HubClient(connection)
    }

    private connection: HubConnection

    private constructor(connection: HubConnection) {
        if (!connection || !(connection instanceof HubConnection)) {
            throw new Error()
        }

        this.connection = connection
    }

    public connect() {
        
    }

    public call(method: string, args: any[]) {
        this.connection.send(method, args)
    }

    public subscribe(method: string, callback: () => void) {
        this.connection.on(method, callback)
    }
}

export default HubClient