<template>
    <div class="main">
        <div class="circle"></div>
        <section class="group">
            <div class="line short"></div>
            <div class="line"></div>
        </section>
    </div>
</template>

<style scoped>
.main {
    display: flex;
    width: 100%;
    height: 100%;
    position: relative;
    align-items: center;
    border-radius: var(--border-radius);
    border: 1px solid var(--color-border-dark);

    --border-radius: 5px;
}

.circle {
    width: 12%;
    height: 100%;
    clip-path: circle(20px);
    background-color: var(--color-border-dark);
}

.group {
    width: 85%;
}

.line {
    height: .7em;
    border-radius: var(--border-radius);
    background-color: var(--color-border-dark);
}

.line:not(:last-child) {
    margin-bottom: .5em;
}

.short {
    width: 50%;
}
</style>