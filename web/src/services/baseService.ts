import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';
import config from '../config';


class Request {
    private instance: AxiosInstance | undefined

    constructor(requeseConfig: AxiosRequestConfig) {
        this.instance = axios.create(requeseConfig)
        // 全局请求拦截
        this.instance.interceptors.request.use(
            (config) => {
                const token = localStorage.getItem("token")
                if (token) {
                    config.headers.Authorization = `Bearer ${token}`
                }

                return config
            },
            (error) => {
                console.log("全局请求拦截失败", error);
            },
        )

        // 全局响应拦截
        this.instance.interceptors.response.use(
            (res) => {
                console.log(res);
                return res.data
                // 只需要返回data即可
            },
            (error) => {
                if(error.response.status === 401){
                    localStorage.clear();
                    location.href = '/login';
                }
                
                return error
            },
        )
    }

    public request<T>(config: AxiosRequestConfig<T>): Promise<T> {
        return new Promise<T>((resolve, reject) => {
            /* eslint-disable */
            this.instance?.request<any, T>(config)
                .then((res) => {
                    resolve(res)
                })
                .catch((err) => {
                    reject(err)
                })
        })
    }

    public get(url: string, config?: AxiosRequestConfig) {
        return this.request({
            method: "GET",
            url,
            ...config,
        })
    }

    public post(url: string, data?: any, config?: AxiosRequestConfig) {
        return this.request({
            method: "POST",
            url,
            data,
            ...config,
        })
    }

    public postConfig(url: string, config?: AxiosRequestConfig) {
        return this.request({
            method: "POST",
            url,
            ...config,
        })
    }

    public put(url: string, data?: any, config?: AxiosRequestConfig) {

        return this.request({
            method: "PUT",
            url,
            data,
            ...config,
        })
    }

    public delete(url: string, config?: AxiosRequestConfig) {
        return this.request({
            method: "DELETE",
            url,
            ...config,
        })
    }
}

export default new Request({
    timeout: 60000,
    baseURL: config.API + "/api/",
    headers: {
        'Content-Type': 'application/json',
        'X-Client': 'web',
    },
})
