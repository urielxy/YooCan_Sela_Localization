var fs = require('fs');
var buildConfiguration = fs.readFileSync("./BuildConfiguration.txt", "utf8").trim();

module.exports = {
    development: "Debug",
    isDevelopment: function () { return buildConfiguration === this.development; },
    isProduction: function () { return buildConfiguration !== this.development; }
};