import { defineStore } from 'pinia'
import { User, UserManager, WebStorageStateStore, type UserManagerSettings } from 'oidc-client';

const config: UserManagerSettings = {
    userStore: new WebStorageStateStore({ store: window.localStorage }),

    authority: 'https://localhost:7250',
    client_id: 'vue-client',

    redirect_uri: 'http://127.0.0.1:5173/callback',
    post_logout_redirect_uri: 'http://127.0.0.1:5173/logout',

    response_type: 'code',
    scope: 'openid profile api1 offline_access',

    automaticSilentRenew: true,
    filterProtocolClaims: true,

    loadUserInfo: false,
}

export const useAuthStore = defineStore('auth', () => {
    let user: User | null = null

    const userManager = new UserManager(config)

    userManager.events.addSilentRenewError(() => {
        userManager.signoutRedirect()
    })

    userManager.events.addAccessTokenExpired(() => {
        renew()
    })

    const getUser = async () => {
        if (!user) {
            console.log('get user');

            user = await userManager.getUser()
        }
        return user
    }

    const login = () => userManager.signinRedirect()

    const logout = () => userManager.signoutRedirect()

    const renew = async () => {
        await userManager.signinSilent()

        window.location.href = '../'
    }

    const signingCallback = (callback: () => void) => {
        userManager.signinRedirectCallback()
            .then((userValue) => {
                user = userValue
                callback()
            }).catch((error) => {
                console.log(error)
            })
    }

    return { user, renew, getUser, login, logout, signingCallback }
})
