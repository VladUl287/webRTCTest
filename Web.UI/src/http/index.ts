import axios from 'axios';
import router from '@/router';

const instance = axios.create({
    baseURL: 'https://localhost:7242/',
    withCredentials: true
});

// instance.interceptors.request.use((config) => {
//     if (store.getters.StateToken) {
//         config.headers = {
//             Authorization: 'Bearer ' + store.getters.StateToken
//         };
//     }
//     return config;
// });

// let refresh = false;
// instance.interceptors.response.use(undefined, async (error) => {
//     if (error.response.status === 401 && error.config && !refresh) {
//         refresh = true;
//         try {
//             await store.dispatch('Refresh');
//             return instance.request(error.config);
//         } catch {
//             await store.dispatch('Logout');
//             router.push('/login');
//         } finally {
//             refresh = false;
//         }
//     }
//     return Promise.reject(error);
// });

export default instance;