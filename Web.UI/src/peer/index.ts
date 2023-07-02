import Peer from "peerjs";

const peer = new Peer('', {
    host: '/',
    port: 9000,
    path: '/myapp'
})

export const createPeer = () => {
    return new Peer('', {
        host: '/',
        port: 9000,
        path: '/myapp'
    })
}

export default peer