import Peer from "peerjs";

const peer = new Peer('', {
    host: '/',
    port: 9000,
    path: '/chat'
})

export default peer