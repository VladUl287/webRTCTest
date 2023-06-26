export const debounce = (callback: () => void, delay: number = 300) => {
    let timeout: number
    return () => {
        clearTimeout(timeout)
        timeout = setTimeout(() => callback(), delay)
    }
}