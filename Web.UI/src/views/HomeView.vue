<template>
  <main>
    <button @onclick="login">Login</button>
    <!-- <video autoplay id="from"></video>
    <video autoplay id="to"></video> -->
  </main>
</template>

<script setup lang="ts">
import { Peer } from 'peerjs'
import { onMounted } from 'vue';
import * as signalR from '@microsoft/signalr'
import * as signalRMsgpack from '@microsoft/signalr-protocol-msgpack'
import { UserManager, WebStorageStateStore } from 'oidc-client';

const login = () => {
  const config = {
    userStore: new WebStorageStateStore({ store: window.localStorage }),
    authority: "https://localhost:7250",
    client_id: 'vue-client',
    // client_secret: 'vue_secret',
    offline_access: true,
    redirect_uri: 'http://localhost:5173/callback.html',
    popup_redirect_uri: 'http://localhost:5173/callback.html',
    // automaticSilentRenew: true,
    // silent_redirect_uri: 'http://localhost:5173/silent-renew.html',
    response_type: 'code',
    scope: 'openid profile api1 offline_access',
    post_logout_redirect_uri: 'http://localhost:5173/',
    filterProtocolClaims: true,
  };

  const manager = new UserManager(config);

  manager.signinRedirect();
}

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
</script>

<style scoped>
video {
  width: 100px;
  height: 100px;
  background-color: red;
}
</style>