export const debounce = (callback: (...args: any[]) => void, delay: number = 300) => {
    let timeout: number
    return (...args: any[]) => {
        clearTimeout(timeout)
        timeout = setTimeout(() => callback(args), delay)
    }
}