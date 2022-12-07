<template>
  <main>
    <video autoplay id="from"></video>
    <video autoplay id="to"></video>
  </main>
</template>

<script setup lang="ts">
const video: HTMLVideoElement | null = document.querySelector('#from')

// navigator.mediaDevices.getUserMedia({ video: true })
//   .then((camera_stream) => {
//     if (!video) return

//     video.srcObject = camera_stream

//     const recorder = new MediaRecorder(camera_stream, { mimeType: 'video/webm' })

//     recorder.ondataavailable = async (event: BlobEvent) => {

//     }

//     recorder.start(1000)
//   })
</script>

<style scoped>
video {
  width: 100px;
  height: 100px;
  background-color: red;
}
</style>