import { defineStore } from 'pinia'
import { HubConnectionBuilder } from '@microsoft/signalr'
import type { HubConnection } from '@microsoft/signalr'
import { useAuthStore } from '@/stores/auth';

export const useCallsStore = defineStore('calls', () => {
    const connection: HubConnection = new HubConnectionBuilder()
        .withUrl("https://localhost:7010/hubs/call", {
            accessTokenFactory: async () => {
                const user = await useAuthStore().getUser()

                return user!.access_token
            }
        })
        .build()

    return { connection }
})