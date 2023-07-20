// export const debounce = (callback: (value: any) => void, delay: number = 300) => {
//     let timeout: number
//     return (value: any) => {
//         clearTimeout(timeout)
//         timeout = setTimeout(() => callback(value), delay)
//     }
// }

export function debounce<T extends (...args: any) => any>(callback: T, delay: number): (...args: Parameters<T>) => Promise<ReturnType<T>> {
    let timeout: number

    return (...args: Parameters<T>): Promise<ReturnType<T>> => {
        clearTimeout(timeout)

        return new Promise((resolve) =>
            timeout = setTimeout(() => {
                const result = callback(args)
                resolve(result)
            }, delay)
        )
    }
}