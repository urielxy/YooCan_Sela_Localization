/// <binding AfterBuild='default' Clean='clean' ProjectOpened='watch-sass, watch-scripts' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    rename = require('gulp-rename'),
    uglify = require("gulp-uglify"),
    sass = require("gulp-sass"),
    plumber = require('gulp-plumber'),
    postcss = require('gulp-postcss'),
    //sourcemaps = require('gulp-sourcemaps'),
    autoprefixer = require('autoprefixer');

gulp.task('css', function () {
    return gulp.src('./src/*.css')
        .pipe(postcss())
        .pipe(gulp.dest('./dest'));
});

var webroot = "./wwwroot/";

var paths = {
    js: webroot + "js/**/*.js",
    ts: [webroot + "scripts/**/*.ts", "!" + webroot + "scripts/**/*.d.ts", "!" + webroot + "scripts/Utils/*.ts"],
    minJs: webroot + "js/**/*.min.js",
    css: [webroot + "css/**/*.css", webroot + "lib/**/*.css",
        "!" + webroot + "css/**/*.min.css", "!" + webroot + "lib/**/*.min.css"],
    scss: webroot + "css/**/*.scss",
    concatJsDest: webroot + "js/site.min.js",
    concatCssDest: webroot + "css/site.min.css"
};

gulp.task('sass', function() {
    gulp.src(paths.scss)
        //.pipe(sourcemaps.init())
        .pipe(plumber())
        .pipe(sass())
        //.pipe(sourcemaps.write('.'))
        .pipe(postcss([autoprefixer()]))
        .pipe(gulp.dest(webroot + "css"));
});

gulp.task('watch-sass', function () {
    gulp.watch(paths.scss, { interval: 1000 }, ['sass']);

});

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);
gulp.task("default",
    [
        //"scripts",
        "sass"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css:singleFile", function () {
    return gulp.src(paths.css, { base: "." })
        .pipe(cssmin())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest("."));
});
//gulp.task("min:css", function () {
//    return gulp.src(paths.css)
//        .pipe(concat(paths.concatCssDest))
//        .pipe(cssmin())
//        .pipe(gulp.dest("."));
//});

gulp.task("min", ["min:js", "min:css:singleFile" /*"min:css"*/]);

//var webpack = require('webpack-stream');
//var named = require('vinyl-named');
//var path = require('path');

//var webpackConfig = {
//    output: {
//        filename: '[name].js'
//    },
//    entry: {},
//    resolve: {
//        extensions: ['', '.webpack.js', '.web.js', '.ts', '.tsx', '.js']
//    },
//    module: {
//        loaders: [
//          { test: /\.ts$/, loader: "ts-loader" }
//        ]
//    }
//};

////https://github.com/shama/webpack-stream/issues/62
//function repath(target, removal, pathOnly) {
//    return target.replace(removal, '')
//      .replace(pathOnly ? /\.[^/.]+$/ : '', '')
//      .replace(new RegExp('\\' + path.sep, 'g'), '/');
//}
//gulp.task('init-script-files-list', function () {
//    return gulp.src(paths.ts)
//      .pipe(named(function (file) {
//          webpackConfig.entry[repath(file.path, file.base, true)]
//            = '.' + repath(file.path, file.cwd);
//          this.queue(file);
//      }));
//});

//gulp.task('watch-scripts', ['init-script-files-list'], function () {
//    return gulp.src(paths.ts)
//      .pipe(plumber())
//      .pipe(webpack(Object.assign({ watch: true }, webpackConfig)))
//      .pipe(gulp.dest(webroot + 'dist'));
//});

//gulp.task('scripts', ['init-script-files-list'], function () {
//    return gulp.src(paths.ts)
//      .pipe(webpack(webpackConfig))
//      .pipe(gulp.dest(webroot + 'dist'));
//});
