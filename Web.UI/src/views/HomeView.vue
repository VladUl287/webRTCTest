<template>
  <main>
    <video autoplay id="from"></video>
    <video autoplay id="to"></video>
  </main>
</template>

<script setup lang="ts">
import { Peer } from 'peerjs'
import signalR from '@microsoft/signalr'
import signalRMsgpack from '@microsoft/signalr-protocol-msgpack'

const peer = new Peer('', {
  host: '/',
  port: 9000
})

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5194/room")
  .withHubProtocol(new signalRMsgpack.MessagePackHubProtocol())
  .build()

async function start() {
  try {
    await connection.start();
  } catch (err) {
    console.log(err);
  }
}

start()

peer.on('open', async (id: string) => {
  await connection.invoke("Join", {
    RoomId: 'ce620881-4dd7-418c-979c-543394147aef',
    UserId: id
  })
})

const video: HTMLVideoElement | null = document.querySelector('#from')

navigator.mediaDevices.getUserMedia({ video: true })
  .then((camera_stream) => {
    if (!video) return

    video.srcObject = camera_stream

    peer.on('call', (call) => {
      call.answer(camera_stream)
      call.on('stream', userVideoStream => {
        const video: HTMLVideoElement | null = document.querySelector('#to')
        if (video) {
          video.srcObject = userVideoStream
        }
      })
    })

    connection.on("connected", (userId: any) => {
      const call = peer.call(userId, camera_stream)
      call.on('stream', userVideoStream => {
        const video: HTMLVideoElement | null = document.querySelector('#to')
        if (video) {
          video.srcObject = userVideoStream
        }
      })
    })
  })
</script>

<style scoped>
video {
  width: 100px;
  height: 100px;
  background-color: red;
}
</style>