<template>
    <dialog ref="dialog">
        <div class="content-wrap">
            <section id="body">
                <div id="videos"></div>
            </section>

            <section id="footer">
                <button @click="collapse">
                    <span class="material-symbols-outlined">collapse_all</span>
                </button>
                <button @click="discard">
                    <span class="material-symbols-outlined">call_end</span>
                </button>
            </section>
        </div>
    </dialog>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref, watch } from 'vue'
import { sendJoinCall, sendLeaveCall, onJoinCall, onLeaveCall } from '@/hubs/chat'
import { createPeer } from '@/peer'
import type Peer from 'peerjs'
import { normalizeNodeId } from '@/helpers/nodes'

const peer = ref<Peer>()
const dialog = ref<HTMLDialogElement>()

const props = defineProps({
    modelValue: Boolean,
    chatId: {
        type: String,
        required: true
    }
})

const emit = defineEmits<{
    (e: 'update:modelValue', value: Boolean): void
}>()

onMounted(() => toggleModal(props.modelValue))

watch(
    () => props.modelValue,
    toggleModal
)

function toggleModal(visible: boolean) {
    if (visible) {
        dialog.value?.showModal()

        setPeer()
    }
    else {
        dialog.value?.close()
    }
}

const setPeer = () => {
    if (!peer.value || peer.value.destroyed) {
        peer.value = createPeer('')

        peer.value.on('open', async (id: string) => {
            const stream = await getStream()

            appendVideo(stream, id, true)

            sendJoinCall({
                chatId: props.chatId,
                peerId: id
            })
        })

        peer.value.on('call', async (call) => {
            const stream = await getStream()
            call.answer(stream)
            call.on('stream', stream => appendVideo(stream, call.peer))
        })

        peer.value.on('error', (err) => console.log('error', err))
        peer.value.on('close', () => console.log('close'))
        peer.value.on('disconnected', () => discard())
    }
}

onJoinCall(async ({ peerId, chatId }) => {
    if (peer.value && peerId === peer.value.id && chatId === props.chatId) {
        const stream = await getStream()
        const call = peer.value.call(peerId, stream)
        call.on('stream', stream => appendVideo(stream, peerId))
    }
})

onLeaveCall(({ peerId }) => {
    if (peer.value && peer.value.id === peerId) {
        return discard()
    }
    removeVideo(peerId)
})

const getStreamMake = () => {
    let camera_stream: MediaStream

    const getCameraStream = async () => {
        if (camera_stream) {
            return camera_stream
        }

        camera_stream = await navigator.mediaDevices.getUserMedia({ audio: true })

        return camera_stream
    }

    return getCameraStream
}

const getStream = getStreamMake()

const stopStream = async () => {
    const stream = await getStream()
    stream.getTracks()
        .forEach(track => track.stop())
}

const collapse = () => emit('update:modelValue', false)

const discard = () => {
    stopStream()

    if (peer.value?.id && props.chatId) {
        sendLeaveCall({
            chatId: props.chatId,
            peerId: peer.value.id
        })
    }

    peer.value?.destroy()
    peer.value = undefined

    clearVideos()

    collapse()
}

const clearVideos = () => {
    const videos = document.querySelector('#videos')

    if (videos) {
        videos.innerHTML = ''
    }
}

const appendVideo = (stream: MediaStream, id: string, muted: boolean = false) => {
    const videos = document.querySelector('#videos')

    if (!videos) return

    const videoExists = videos.querySelector('#' + normalizeNodeId(id))

    if (videoExists) return

    const video = document.createElement('video')
    video.srcObject = stream
    video.autoplay = true
    video.muted = muted
    video.id = normalizeNodeId(id)

    videos.appendChild(video)
}

const removeVideo = (id: string) => {
    document.querySelector('#' + normalizeNodeId(id))?.remove()
}

window.onbeforeunload = discard

onUnmounted(discard)
</script>

<style scoped>
dialog {
    width: 90%;
    height: 80%;
    overflow: hidden;
    border-radius: .5em;
    border: 1px solid var(--color-border-dark);
    background-color: var(--color-background-dark);
}

dialog::backdrop {
    background-color: #000000cc;
}

.content-wrap {
    height: 100%;
    display: grid;
    grid-template-rows: 1fr auto;
}

#body {
    display: flex;
    overflow: hidden;
    align-items: center;
}

#videos {
    width: 100%;
    display: flex;
    row-gap: .5em;
    overflow: hidden;
    column-gap: .5em;
    align-items: center;
    justify-content: center;
}

#videos>>>video {
    flex-basis: 400px;
    background-color: #000;
}

#footer {
    margin: 0 auto;
    padding: var(--section-gap) 0;
}

#footer button {
    display: inline-flex;
    border-radius: 50%;
    background-color: transparent;
    color: var(--color-placeholder);
    margin: 0 var(--section-gap);
    padding: var(--section-gap-medium);
    border: 1px solid var(--color-border-dark);
}
</style>