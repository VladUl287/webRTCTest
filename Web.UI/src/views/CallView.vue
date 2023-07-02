<template>
    <dialog ref="dialog">
        <div class="content-wrap">
            <section id="header">
                <p>{{ header }}</p>
            </section>

            <section id="body">
                <div id="videos">
                    <video></video>
                    <video></video>
                    <video></video>
                    <video></video>
                    <video></video>
                </div>
                <!-- <video></video> -->
            </section>

            <section id="footer">
                <button @click="turn">turn</button>
                <button @click="discard">discard</button>
            </section>
        </div>
    </dialog>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref, watch } from 'vue'
import { createPeer } from '@/peer'
import type Peer from 'peerjs'

const peer = ref<Peer>()
const dialog = ref<HTMLDialogElement>()

const props = defineProps({
    modelValue: Boolean,
    header: String
})

const emit = defineEmits<{
    (e: 'update:modelValue', value: Boolean): void
}>()

onMounted(() => {

})

watch(
    () => props.modelValue,
    (visible) => {
        if (visible && (!peer.value || peer.value.destroyed)) {
            dialog.value?.showModal()

            peer.value = createPeer()

            initialize(peer.value.id)
        }
        else {
            dialog.value?.close()
        }
    }
)

const initialize = async (id: string) => {
    const stream = await getStream()

    // appendVideo(stream, id, true)
}

let camera_stream: MediaStream

const getStream = async () => {
    if (camera_stream) {
        return camera_stream
    }

    camera_stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true })
    return camera_stream
}

const turn = () => {
    emit('update:modelValue', false)
}

const discard = () => {
    peer.value?.destroy()
}

const appendVideo = (stream: MediaStream, id: string, muted: boolean = false) => {
    const body = document.querySelector('#body')

    if (!body) return

    const videoExists = body.querySelector('#' + normilizeId(id))

    if (videoExists) return

    const video = document.createElement('video')
    video.srcObject = stream
    video.autoplay = true
    video.muted = muted
    video.id = 'a' + id

    body.appendChild(video)
}

const removeVideo = (id: string) => {
    document.querySelector('#' + normilizeId(id))?.remove()
}

const normilizeId = (value: string) => 'a' + value

</script>

<style scoped>
dialog {
    padding: 0;
    width: 90%;
    height: 80%;
    overflow: hidden;
    border-radius: .5em;
    color: var(--color-text);
    border: 1px solid var(--color-border-dark);
    background-color: var(--color-background-dark);
}

dialog::backdrop {
    background-color: #000000cc;
}

.content-wrap {
    height: 100%;
    display: grid;
    grid-template-rows: auto 1fr auto;
    background-color: red;
}

#header {
    margin: 0;
    text-align: center;
}

#body {
    padding: 50px;
    overflow: hidden;
    background-color: blue;
}

#videos {
    row-gap: .5em;
    display: flex;
    flex-wrap: wrap;
    column-gap: .5em;
    align-items: center;
    justify-content: center;
    background-color: yellow;
}

#videos > * {
    min-width: 250px;
    min-height: 150px;
    background-color: lawngreen;
}

#footer {
    padding: .5em 0;
    margin: 0 auto;
    width: fit-content;
}

#footer button {
    padding: .5em;
}
</style>