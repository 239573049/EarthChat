import Request from './baseService'

const baseURL = "v1/Files"

class FileService {

    upload(form:FormData){
        return Request.post(`${baseURL}/Upload`,form,{
            headers:{
                'Content-Type': 'multipart/form-data'
            }
        });
    }

    /**
     * 上传文件
     * @param base64 
     * @param fileName 
     * @returns 
     */
    uploadBase64(base64: string, fileName: string) {
        return Request.post(`${baseURL}/UploadBase64`, {
            value: base64,
            fileName: fileName
        });
    }

    /**
     * 删除文件
     * @param uri 
     * @returns 
     */
    deleteFile(uri: string) {
        return Request.delete(`${baseURL}`,{
            params:{
                uri
            }
        });
    }
}

export default new FileService()