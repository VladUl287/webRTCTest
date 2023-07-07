import Peer from "peerjs";

const options = {
    host: '/',
    port: 9000,
    path: '/myapp'
}

export const createPeer = (id: string) => new Peer(id, options)