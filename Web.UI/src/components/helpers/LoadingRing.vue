<template>
    <div id="loading"></div>
</template>
<script setup lang="ts">

</script>

<style scoped>
#loading {
    width: 100%;
    height: 100%;
}

#loading:after {
    content: '';
    width: 100%;
    height: 100%;
    display: block;
    border-radius: 50%;
    border: 4px solid #fff;
    animation: loading 1s linear infinite;
    border-color: var(--color-border) transparent var(--color-border) transparent;
}

@keyframes loading {
    0% {
        transform: rotate(0deg);
    }

    100% {
        transform: rotate(360deg);
    }
}
</style>