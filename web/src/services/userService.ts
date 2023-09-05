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
    
    /**
     * 新建用户
     */
    create(value:any){
        return Request.post(`${baseURL}`,value)
    }

    /**
     * 编辑用户信息
     * @param value 
     * @returns 
     */
    update(value:any){
        return Request.put(`${baseURL}`,value)
    }
}

export default new UserService()