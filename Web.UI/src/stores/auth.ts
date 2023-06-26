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
    let user: User | null = null

    const userManager = new UserManager(config)


    const getUser = async () => {
        if (!user) {
            console.log('getUser')
            console.time()
            user = await userManager.getUser()
            console.timeEnd()
        }

        return user
    }

    const login = () => userManager.signinRedirect()

    const logout = () => userManager.signoutRedirect()

    const signingRedirectCallback = (callback: () => void) => {
        userManager.signinRedirectCallback()
            .then((u) => {
                user = u
                callback()
            }).catch((err) => {
                console.log(err)
            })
    }

    return { user, getUser, login, logout, signingRedirectCallback }
})
