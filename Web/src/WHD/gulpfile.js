/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var rimraf = require("rimraf");
var fs = require("fs");
var less = require("gulp-less");

var bower_path = './bower_components/';
var webroot = './wwwroot';

gulp.task('watch', function () {
    gulp.watch('Style/*.less', ['less']);
})

gulp.task('less', function () {
    return gulp.src('Style/main.less')
        .pipe(less())
        .pipe(gulp.dest(webroot + '/css'));
})

//gulp.task('copy', function () {
//    var bower = {
//        'angular': 'angular/angular*.{js,map}',
//        'angular-route': 'angular-route/angular-route.js',
//        'bootstrap': 'bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}',
//        'jquery': 'jquery/dist/jquery*.{js,map}',
//        'font-awesome': 'Font-Awesome/**/*.{css,otf,eot,svg,ttf,woff,woff2}',
//        'dirPagination': 'dirPagination/*.{js,html}'
//    };

//    for (var destinationDir in bower) {
//        gulp.src(bower_path + bower[destinationDir])
//          .pipe(gulp.dest(webroot + '/lib/' + destinationDir));
//    }
//})