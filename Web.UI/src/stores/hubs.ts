import { useAuthStore } from '@/stores/auth';
import { defineStore } from 'pinia'
import { HubConnectionState, type HubConnection } from '@microsoft/signalr'
import { HubConnectionBuilder } from '@microsoft/signalr'

export const useHubsStore = defineStore('hubs', () => {
    const connection: HubConnection = new HubConnectionBuilder()
        .withUrl("https://localhost:7010/hubs/chat", {
            accessTokenFactory: async () => {
                const user = await useAuthStore().getUser()

                return user!.access_token
            }
        })
        // .withHubProtocol(new signalRMsgpack.MessagePackHubProtocol())
        .build()

    const connect = async () => {
        if (connection.state !== HubConnectionState.Disconnected) {
            return;
        }

        return await connection.start()
    }

    return { connection, connect }
})

// import { Peer } from 'peerjs'
// import { onMounted } from 'vue';
// import * as signalR from '@microsoft/signalr'
// import * as signalRMsgpack from '@microsoft/signalr-protocol-msgpack'

// onMounted(async () => {
//   const connection = new signalR.HubConnectionBuilder()
//     .withUrl("http://localhost:5194/room")
//     .withHubProtocol(new signalRMsgpack.MessagePackHubProtocol())
//     .build()

//   const peer = new Peer('', {
//     host: '/',
//     port: 9000,
//     path: '/myapp'
//   })

//   await connection.start()

//   peer.on('open', async (id: string) => {
//     await connection.invoke("Join", {
//       RoomId: 'ce620881-4dd7-418c-979c-543394147aef',
//       UserId: id
//     })
//   })

//   const video: HTMLVideoElement | null = document.querySelector('#from')

//   const camera_stream = await navigator.mediaDevices.getUserMedia({ video: true })

//   video!.srcObject = camera_stream

//   peer.on('call', (call) => {
//     call.answer(camera_stream)
//     call.on('stream', userVideoStream => {
//       const video: HTMLVideoElement | null = document.querySelector('#to')
//       if (video) {
//         video.srcObject = userVideoStream
//       }
//     })
//   })

//   connection.on("connected", (userId: string) => {
//     peer.call(userId, camera_stream)
//   })
// })