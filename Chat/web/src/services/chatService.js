import Http from '../utils/request';

function getOnlineUsers() {
    return Http.get('/Chats/OnlineUsers');
}


function GetList(page,pageSize) {
    return Http.get('/Chats/List?page=' + page + '&pageSize=' + pageSize);
}

export {
    getOnlineUsers,
    GetList
}