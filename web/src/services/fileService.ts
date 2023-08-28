import Request from './baseService'

const baseURL = "v1/Files"

class FileService {

    uploadBase64(base64: string, fileName: string) {
        return Request.post(`${baseURL}/UploadBase64`, {
            value: base64,
            fileName: fileName
        });
    }
}

export default new FileService()