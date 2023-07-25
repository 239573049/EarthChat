import Http from '../utils/request';

function get() {
    return Http.get('/Users');
}

export {
    get
}