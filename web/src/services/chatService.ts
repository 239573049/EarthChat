import { CreateGroupDto } from '../dto'
import Request from './baseService'
import connection from './chatHubService'


const baseURL = "v1/Chats"

class ChatService {

    getUserGroup(){
        return Request.get(`${baseURL}/UserGroup`)
    }
    
    createGroup(value:CreateGroupDto){
        return Request.post(`${baseURL}/Group?connections=`+connection.connectionId,value)
    }

    getGroupInUser(groupId:string){
        return Request.get(`${baseURL}/GroupInUser`,{
            params:{
                groupId,
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

    /**
     * 获取群聊详细信息
     * @param id 
     * @returns 
     */
    getGroup(id:string){
        return Request.get(`${baseURL}/Group/${id}`)
    }

    /**
     * 加入群聊
     * @param id 
     * @returns 
     */
    invitationGroup(id:string){
        return Request.post(`${baseURL}/InvitationGroup/${id}`)
    }

}

export default new ChatService()