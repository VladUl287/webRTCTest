export const debounce = (callback: (value: any) => void, delay: number = 300) => {
    let timeout: number
    return (value: any) => {
        clearTimeout(timeout)
        timeout = setTimeout(() => callback(value), delay)
    }
}