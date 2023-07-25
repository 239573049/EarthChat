import axios from "axios";

class Http {

  constructor(baseURL) {
    var token = localStorage.getItem("token");
    this.axiosInstance = axios.create({
      baseURL,
      timeout: 60000 * 5,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    this.axiosInstance.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem("token");
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        // 处理请求错误
        return Promise.reject(error);
      }
    );

    this.axiosInstance.interceptors.response.use(
      (response) => {
        return response;
      },
      (error) => {
        try {
          if (error.response.status === 401) {
            localStorage.removeItem("token");
            window.location.href = "/login";
          }
        } catch {
          console.log(error);
        }
        // 处理响应错误
        return Promise.reject(error);
      }
    );
  }

  async get(url, params) {
    const response = await this.axiosInstance.get(url, { params });
    return response.data;
  }

  async post(url, data) {
    const response = await this.axiosInstance.post(url, data);
    return response.data;
  }

  async put(url, data) {
    const response = await this.axiosInstance.put(url, data);
    return response.data;
  }

  async delete(url) {
    const response = await this.axiosInstance.delete(url);
    return response.data;
  }

  async fetchAsStream(
    url,
    data
  ) {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${localStorage.getItem("token")}`
      },
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const reader = response.body?.getReader();
      const { done, value } = await reader.read();
      if (done) {
        throw new Error("ReadableStream not supported in this browser." + value);
      }
      throw new Error(
        `Failed to fetch ${url}: ${response.status} ${response.statusText}`
      );
    }
    if (!response.body) {
      throw new Error("ReadableStream not supported in this browser.");
    }
    const reader = response.body.getReader();
    return {
      [Symbol.asyncIterator]() {
        return {
          async next() {
            const { done, value } = await reader.read();
            if (done) {
              return { done: true, value: null };
            }
            return {
              done: false,
              value: new TextDecoder("utf-8").decode(value),
            };
          },
        };
      },
    };

  }
}

// 判断当前环境是否开发环境



export default new Http('http://124.222.89.53/api/v1/');
