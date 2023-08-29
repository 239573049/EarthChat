import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';

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
                // res 为AxiosResponse 类型，含有conig\data\headers\request\status\statusText属性
                console.log("全局响应拦截的", res);
                return res.data
                // 只需要返回data即可
            },
            (error) => {
                console.log("全局响应失败拦截");
                console.log(error.request);
                console.log(error.response);
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
    baseURL: 'http://localhost:5218/api/',
    headers: {
        'Content-Type': 'application/json',
        'X-Client': 'web',
    },

 })
