import { computed } from 'vue'
import { defineStore } from 'pinia'
import { User, UserManager, WebStorageStateStore, type UserManagerSettings } from 'oidc-client'

const config: UserManagerSettings = {
    userStore: new WebStorageStateStore({ store: window.localStorage }),

    authority: 'https://localhost:7250',
    client_id: 'vue-client',

    redirect_uri: 'http://127.0.0.1:5173/callback',
    post_logout_redirect_uri: 'http://127.0.0.1:5173/logout',

    response_type: 'code',
    scope: 'openid profile api1 offline_access',

    automaticSilentRenew: true,
    filterProtocolClaims: true
}

export const useAuthStore = defineStore('auth', () => {
    let user: User | null = null

    const profile = computed(() => user?.profile)

    const userManager = new UserManager(config)

    userManager.events.addSilentRenewError(() => {
        userManager.signoutRedirect()
    })

    userManager.events.addAccessTokenExpired(() => {
        silentRenew()
    })

    const getUser = async () => {
        if (!user) {
            user = await userManager.getUser()
        }
        return user
    }

    const silentRenew = async () => {
        try {
            await userManager.signinSilent()

            window.location.href = '../'
        } catch {
            logout()
        }

    }

    const login = () => userManager.signinRedirect()

    const logout = () => userManager.signoutRedirect()

    const signingCallback = (callback: () => void) => {
        userManager.signinRedirectCallback()
            .then((userValue) => {
                user = userValue
                callback()
            }).catch((error) => {
                console.log(error)
            })
    }

    return { profile, silentRenew, getUser, login, logout, signingCallback }
})
