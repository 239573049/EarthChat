import Request from './baseService'

const baseURL = "v1/Users"

class UserService {

    /**
     * 获取用户信息
     * @returns 
     */
    get() {
        return Request.get(`${baseURL}`)
    }
    
}

export default new UserService()