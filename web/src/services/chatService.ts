import { CreateGroupDto } from '../dto'
import Request from './baseService'
import connection from './chatHubService'


const baseURL = "v1/Chats"

class ChatService {

    getUserGroup(group:boolean) {
        return Request.get(`${baseURL}/UserGroup`,{
            params:{
                group
            }
        })
    }

    createGroup(value: CreateGroupDto) {
        return Request.post(`${baseURL}/Group?connections=` + connection.connectionId, value)
    }

    getGroupInUser(groupId: string,page:number,pageSize:number) {
        return Request.get(`${baseURL}/GroupInUser`, {
            params: {
                groupId,
                page,
                pageSize,
            }
        })
    }

    getList(groupId: string, page: number, pageSize: number) {
        return Request.get(`${baseURL}/List`, {
            params: {
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
    getGroup(id: string) {
        return Request.get(`${baseURL}/Group/${id}`)
    }

    /**
     * 获取群聊在线用户id
     */
    getOnLineUserIds(groupId: string) {
        return Request.get(`${baseURL}/OnLineUserIds`, {
            params: {
                groupId
            }
        })
    }

    /**
     * 加入群聊
     * @param id 
     * @returns 
     */
    invitationGroup(id: string) {
        return Request.post(`${baseURL}/InvitationGroup/${id}`)
    }

    /**
     * 撤回消息
     * @param id 
     * @returns 
     */
    countermandMessage(id:string){
        return Request.post(`${baseURL}/CountermandMessage/`+id)
    }
}

export default new ChatService()