export const Configuration = Object.assign({},
    window.config.env,
    {
        version: require('version')
    }
);

export const HttpFetchHeaders = {
    'Content-Type': 'application/json; charset=utf-8',
    'Accept': 'application/json',
    'Cache-Control': 'no-store'
};

export const deepCopy = (obj) => JSON.parse(JSON.stringify(obj));