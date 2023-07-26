// vite判断当前环境变量是development还是production

const config = {
    API_URL: import.meta.env.MODE === 'development' ? 'http://localhost:5218' : '',

};

export default config;