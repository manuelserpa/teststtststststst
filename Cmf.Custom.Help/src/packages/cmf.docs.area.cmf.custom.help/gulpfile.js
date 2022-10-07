var localGulp = require('gulp');
var path = require('path');

//place your config here
var ctx = {
  packageName: 'cmf.docs.area.cmf.custom.help',
  baseDir: __dirname,
  libsFolder: 'node_modules/',
  localLibsFolder: 'local_libs/',
  tempFolder: 'temp/',
  sourceFolder: 'src/',
  project: 'cmf.docs.area.cmf.custom.help.csproj',
  type: 'documentation'
};

var gulpFunction = function(parentGulp, prefix){
    ctx.prefix = prefix;

    var tasks = require('@criticalmanufacturing/dev-tasks')(parentGulp, ctx);
    var gulp = tasks.gulp;
    var seq = tasks.plugins.seq;

    return tasks;
  };


if (process.cwd() === __dirname) {
    gulpFunction(localGulp);
}

module.exports = gulpFunction;
