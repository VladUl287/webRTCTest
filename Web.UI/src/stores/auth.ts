import { defineStore } from 'pinia'
import { UserManager, WebStorageStateStore } from 'oidc-client';

export const useAuthStore = defineStore('auth', () => {
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

    const userManager = new UserManager(config)

    const getUser = () => userManager.getUser()

    const login = () => userManager.signinRedirect()

    const logout = () => userManager.signoutRedirect()

    const signingRedirectCallback = (callback: () => void) => {
        userManager.signinRedirectCallback()
            .then((user) => {
                console.log(user)
                callback()
            }).catch((err) => {
                console.log(err)
            })
    }

    return { getUser, login, logout, signingRedirectCallback }
})
