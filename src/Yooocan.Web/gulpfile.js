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
    autoprefixer = require('autoprefixer');

var webroot = "./wwwroot/";

var environment = require("./environment.js");

var paths = {
    js: webroot + "js/**/*.js",
    ts: [webroot + "scripts/**/*.ts", "!" + webroot + "scripts/**/*.d.ts", "!" + webroot + "scripts/Utils/*.ts"],
    minJs: webroot + "js/**/*.min.js",
    css: webroot + "css/**/*.css",
    scss: webroot + "css/**/*.scss",
    minCss: webroot + "css/**/*.min.css",
    concatJsDest: webroot + "js/site.min.js",
    concatCssDest: webroot + "css/site.min.css"
};

gulp.task('sass', function () {
    gulp.src(paths.scss)
        .pipe(plumber())
        .pipe(sass())
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
gulp.task("default", ["scripts", "sass"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css:singleFile", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(cssmin())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest(webroot + "css"));
});
//gulp.task("min:css", function () {
//    return gulp.src([paths.css, "!" + paths.minCss])
//        .pipe(concat(paths.concatCssDest))
//        .pipe(cssmin())
//        .pipe(gulp.dest("."));
//});

gulp.task("min", ["min:js", "min:css:singleFile" /*"min:css"*/]);

var webpackStream = require('webpack-stream');
var webpack = require('webpack');
var named = require('vinyl-named');
var path = require('path');

var webpackConfig = {
    output: {
        filename: '[name].js'
    },
    entry: {
        layout: [webroot + "/scripts/Shared/Layout"]
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.tsx', '.js']
    },
    module: {
        rules: [
            { test: /\.ts$/, use: ["babel-loader", "ts-loader"] },
            { test: /\.css/, use: ["style-loader", "css-loader"] }
        ]
    },
    plugins: [
        new webpack.optimize.UglifyJsPlugin({
            sourceMap: true
            //, mangle: false
            //, compress: false
            //, beautify: true
            //, comments: true
        }),
        new webpack.optimize.CommonsChunkPlugin({
            name: "layout",
            minChunks: Infinity
        })
    ]
    , devtool: environment.isProduction() ? "source-map" : "eval-source-map"
    // using CommonsChunkPlugin instead of externals because it's cleaner
    //, externals: {
    //    "./Shared/Layout": "window",
    //    "../Shared/Layout": "window",
    //    "./Layout": "window"
    //}
};

//https://github.com/shama/webpack-stream/issues/62
function repath(target, removal, pathOnly) {
    return target.replace(removal, '')
        .replace(pathOnly ? /\.[^/.]+$/ : '', '')
        .replace(new RegExp('\\' + path.sep, 'g'), '/');
}
gulp.task('init-script-files-list', function () {
    return gulp.src(paths.ts)
        .pipe(named(function (file) {
            webpackConfig.entry[repath(file.path, file.base, true)]
                = '.' + repath(file.path, file.cwd);
            this.queue(file);
        }));
});

gulp.task('watch-scripts', ['init-script-files-list'], function () {
    return gulp.src(paths.ts)
        .pipe(plumber())
        .pipe(webpackStream(Object.assign({ watch: true }, webpackConfig)))
        .pipe(gulp.dest(webroot + 'dist'));
});

gulp.task('scripts', ['init-script-files-list', 'copy-npm-dependencies'], function () {
    return gulp.src(paths.ts)
        .pipe(webpackStream(webpackConfig))
        .pipe(gulp.dest(webroot + 'dist'));
});

gulp.task('copy-npm-dependencies', function () {
    var npmConfig = require('./package.json');
    var npmRoot = "./node_modules/";
    var deps = Object.keys(npmConfig.dependencies)
        .map(function (key) { return npmRoot + key + "/**/*" });
    return gulp.src(deps, { base: npmRoot })
        .pipe(gulp.dest(webroot + "npm_lib"));
});
