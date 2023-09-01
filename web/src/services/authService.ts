import Request from './baseService'

const baseURL = "v1/Auths"

class AuthService {

    public Login(value: { account: string, password: string, agree: boolean }) {
        return Request.postConfig(`${baseURL}`,{
            params: value
        })
    }

    public gitHubAuth(accessToken: string) {
        return Request.postConfig(`${baseURL}/GithubAuth`,{
            params:{
                accessToken
            }
        } )
    }

    public giteeAuth(accessToken: string) {
        return Request.postConfig(`${baseURL}/GiteeAuth`, {
            params:{
                accessToken
            }
        } )
    }
}

export default new AuthService()