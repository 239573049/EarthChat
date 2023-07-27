// vite判断当前环境变量是development还是production

const config = {
    API_URL: import.meta.env.MODE === 'development' ? 'http://124.222.89.53' : '',

};

export default config;