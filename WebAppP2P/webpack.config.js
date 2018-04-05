/// <binding ProjectOpened='Watch - Development' />
"use strict";
var webpack = require("webpack");
var production = process.env.NODE_ENV === 'production' ? true : false;
var version = JSON.stringify(require('./package.json').version + "." + (() => {
    var date = new Date();
    return "" + date.getDate() + (date.getMonth() + 1) + date.getFullYear() + date.getHours() + date.getMinutes();
})());

console.info("\n")
console.info("Webapp P2P Production mode: " + production);
console.info("Webapp P2P Version: " + version);
console.info("\n")

module.exports = {
    entry: {
        main: __dirname + "/wwwroot/index.js"
    },
    output: {
        path: __dirname + "/wwwroot",
        filename: "bundle.js",
        publicPath: production ? '/' : '/hmr/'
    },
    externals: {
        'version': version
    },
    devtool: production ? "#hidden-source-map" : "#eval-source-map",
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                query: {
                    presets: ["es2015", "react", "stage-0"]
                }
            },
            {
                test: /\.json$/,
                loader: 'json-loader'
            }
        ]
    },
    resolve: {
        extensions: ['*', '.js', '.jsx'],
    }
};