import { CreateGroupDto } from '../dto'
import Request from './baseService'

const baseURL = "v1/Chats"

class ChatService {

    getUserGroup(){
        return Request.get(`${baseURL}/UserGroup`)
    }
    
    createGroup(value:CreateGroupDto){
        return Request.post(`${baseURL}/Group`,value)
    }

    getGroupInUser(groupId:string){
        return Request.get(`${baseURL}/GroupInUser`,{
            params:{
                groupId
            }
        })
    }

    getList(groupId:string,page:number,pageSize:number){
        return Request.get(`${baseURL}/List`,{
            params:{
                groupId,
                page,
                pageSize
            }
        })
    }
}

export default new ChatService()