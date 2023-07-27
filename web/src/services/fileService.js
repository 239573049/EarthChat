import Http from '../utils/request';

function uploadBase64(base64, fileName) {
    return Http.post('/Files/UploadBase64',{
        value: base64,
        fileName: fileName
    });
}


export {
    uploadBase64,
}