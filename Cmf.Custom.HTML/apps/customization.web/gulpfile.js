var localGulp = require('gulp');
var configJson = require('./config.json');

/** Bundles Global Variables */
var bundlePath = configJson.packages && configJson.packages.bundlePath || "bundles";

//place your config here
var ctx = {
    packageName: 'customization.web',
    baseDir: __dirname,
    libsFolder: 'node_modules/',
    localLibsFolder: 'local_libs/',
    tempFolder: 'temp/',
    sourceFolder: 'src/',
    project: 'customization.web.csproj',
    type: 'webApp',
    defaultPort: 7000,
    availablePackages: configJson.packages && configJson.packages.available,
    isBundleBuilderOn: true,
    isMetadataBundlerOn: configJson.packages && configJson.packages.bundles && configJson.packages.bundles.metadata,
    isi18nBundlerOn: configJson.packages && configJson.packages.bundles && configJson.packages.bundles.i18n,
    bundlePath: bundlePath
};

var gulpFunction = function (parentGulp, prefix) {
    ctx.prefix = prefix;

    var tasks = require('@criticalmanufacturing/dev-tasks')(parentGulp, ctx);

    tasks.tasks.web(tasks.plugins.gulpWrapper, ctx);

    return tasks;
};


if (process.cwd() === __dirname) {
    gulpFunction(localGulp);
}

module.exports = gulpFunction;
