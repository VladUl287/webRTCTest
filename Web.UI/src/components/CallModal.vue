<template>
    <dialog ref="dialog">
        <div class="wrap">
            <section id="header">
                <p>Lorem ipsum, dolor sit amet consectetur adipisicing elit!</p>
            </section>
            <slot></slot>
            <section id="footer">
                <button @click="turn">turn</button>
                <button @click="discard">discard</button>
            </section>
        </div>
    </dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

const dialog = ref<HTMLDialogElement>()

const props = defineProps({
    modelValue: Boolean
})

const emits = defineEmits<{
    (e: 'update:modelValue', value: Boolean): void,
    (e: 'endcall'): void
}>()

watch(
    () => props.modelValue,
    (visible) => {
        visible ?
            dialog.value?.showModal() :
            dialog.value?.close()
    }
)

const turn = () => emits('update:modelValue', false)

const discard = () => emits('endcall')
</script>

<style scoped>
dialog {
    width: 90%;
    height: 70%;
    border-radius: .5em;
    color: var(--color-text);
    border: 1px solid var(--color-border-dark);
    background-color: var(--color-background-dark);
}

dialog::backdrop {
    background-color: #000000cc;
}

.wrap {
    height: 100%;
    row-gap: 1em;
    display: grid;
    grid-template-rows: 1fr auto 1fr;
}

#header {
    margin: 0;
    text-align: center;
}

#footer {
    margin: 0 auto;
    width: fit-content;
}

#footer button {
    padding: .5em;
}
</style>