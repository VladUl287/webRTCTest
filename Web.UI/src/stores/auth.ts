import { defineStore } from 'pinia'
import { User, UserManager, WebStorageStateStore } from 'oidc-client';

const config = {
    userStore: new WebStorageStateStore({ store: window.localStorage }),
    authority: "https://localhost:7250",
    client_id: 'vue-client',
    offline_access: true,
    redirect_uri: 'http://127.0.0.1:5173/callback',
    popup_redirect_uri: 'http://127.0.0.1:5173/callback.html',
    automaticSilentRenew: true,
    silent_redirect_uri: 'http://127.0.0.1:5173/silent-renew.html',
    response_type: 'code',
    scope: 'openid profile api1 email offline_access',
    post_logout_redirect_uri: 'http://127.0.0.1:5173/',
    filterProtocolClaims: true,

    loadUserInfo: false,
}

export const useAuthStore = defineStore('auth', () => {
    let userInstance: User | null

    const userManager = new UserManager(config)

    const getUser = async () => {
        if (!userInstance) {
            console.log('getUser')
            console.time()
            userInstance = await userManager.getUser()
            console.timeEnd()
        }

        return userInstance
    }

    const login = () => userManager.signinRedirect()

    const logout = () => userManager.signoutRedirect()

    const signingRedirectCallback = (callback: () => void) => {
        userManager.signinRedirectCallback()
            .then((user) => {
                userInstance = user
                callback()
            }).catch((err) => {
                console.log(err)
            })
    }

    return { getUser, login, logout, signingRedirectCallback }
})
