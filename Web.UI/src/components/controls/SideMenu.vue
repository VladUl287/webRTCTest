<template>
    <div class="side-menu" :class="{ 'active': active }">
        <section class="control">
            <slot name="control"></slot>
        </section>
        <slot name="content"></slot>
    </div>
</template>

<script setup lang="ts">
defineProps({
    active: Boolean
})
</script>

<style scoped>
.side-menu {
    position: absolute;
    z-index: 1;
    inset: 0;
    transform: translateX(-100%);
    transition: transform 150ms linear;
    background-color: var(--color-background-dark)
}

.side-menu.active {
    transform: unset;
}

.side-menu .control {
    width: fit-content;
    position: absolute;
    right: .5em;
    top: .5em;
}
</style>