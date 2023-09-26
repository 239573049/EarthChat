import { CreateGroupDto, FriendRegistrationInput } from '../dto'
import Request from './baseService'
import connection from './chatHubService'


const baseURL = "v1/Friends"

class FriendService {

    FriendState(id:string){
        return Request.post(`${baseURL}/FriendState?friendId=`+id)
    }

    FriendRegistration(input:FriendRegistrationInput){
        return Request.post(`${baseURL}/FriendRegistration`,input)
    }
}

export default new FriendService()