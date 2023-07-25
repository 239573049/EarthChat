import Http from '../utils/request';

function githubAuth(accessToken) {
    return Http.post('/Auths/GithubAuth?accessToken=' + accessToken);
}


function giteeAuth(accessToken, redirect_uri) {
    return Http.post('/Auths/GiteeAuth?accessToken=' + accessToken + '&redirect_uri=' + redirect_uri);
}

export {
    githubAuth,
    giteeAuth
}