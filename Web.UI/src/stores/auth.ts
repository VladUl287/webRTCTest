import { defineStore } from 'pinia'
import { UserManager, WebStorageStateStore } from 'oidc-client';

export const useAuthStore = defineStore('auth', () => {
    const config = {
        userStore: new WebStorageStateStore({ store: window.localStorage }),
        authority: "https://localhost:5001",
        client_id: 'vue',
        client_secret: 'vue_secret',
        offline_access: true,
        redirect_uri: 'http://localhost:8080/callback.html',
        popup_redirect_uri: 'http://localhost:8080/callback.html',
        automaticSilentRenew: true,
        silent_redirect_uri: 'http://localhost:8080/silent-renew.html',
        response_type: 'code',
        scope: 'openid profile api1 offline_access',
        post_logout_redirect_uri: 'http://localhost:8080/',
        filterProtocolClaims: true,
    };

    const userManager = new UserManager(config);

    const getUser = () => userManager.getUser()

    const login = () => userManager.signinRedirect();

    const logout = () => userManager.signoutRedirect();

    return { getUser, login, logout }
})
