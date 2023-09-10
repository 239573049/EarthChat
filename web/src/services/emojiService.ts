import Request from './baseService'

const baseURL = "v1/Emojis"

class EmojiService {
    create(path:string){
        return Request.post(`${baseURL}?path=`+path)
    }

    delete(id:string){
        return Request.delete(`${baseURL}/`+id)
    }

    get(){
        return Request.get(`${baseURL}/List`)
    }
}

export default new EmojiService()