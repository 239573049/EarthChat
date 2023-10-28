
var url;

if (import.meta.env.MODE === "development") {
    url = "http://localhost:5218"
} else {
    url = 'https://api.tokengo.top';
}

const config = {
    API: url
}

export default config