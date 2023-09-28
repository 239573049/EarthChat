import { CreateGroupDto, FriendRegistrationInput } from '../dto'
import Request from './baseService'
import connection from './chatHubService'


const baseURL = "v1/Friends"

class FriendService {

    FriendState(id: string) {
        return Request.post(`${baseURL}/FriendState?friendId=` + id)
    }

    FriendRegistration(input: FriendRegistrationInput) {
        return Request.post(`${baseURL}/FriendRegistration`, input)
    }

    List(page: number, pageSize: number) {
        return Request.get(`${baseURL}/List`, {
            params: {
                page,
                pageSize
            }
        })
    }

    FriendHandle(id: string, state: number) {
        return Request.postConfig(`${baseURL}/FriendHandle/${id}`, {
            params: {
                state
            }
        })
    }
}

export default new FriendService()