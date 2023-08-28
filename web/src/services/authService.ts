import Request from './baseService'

const baseURL = "v1/Auths"

class AuthService {

    public Login(value: { account: string, password: string, agree: boolean }) {
        return Request.post(`${baseURL}`,{},{
            params: value
        })
    }

    public gitHubAuth(accessToken: string) {
        return Request.post(`${baseURL}/GithubAuth`, { accessToken })
    }

    public giteeAuth(accessToken: string) {
        return Request.post(`${baseURL}/GiteeAuth`, { accessToken })
    }
}

export default new AuthService()