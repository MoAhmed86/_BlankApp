/// <binding ProjectOpened='watch-All' />
var gulp = require('gulp');
const utility = require('front-end-assistant');
const babel = require('gulp-babel');
const sourcemaps = require('gulp-sourcemaps');
const chmod = require('gulp-chmod');

/////////////////////////////////////////// variable ///////////////////////////////////////////



var config = {
    destnation: "./UI/wwwroot/css/",
    target: "./UI/SCSS/",
    scriptTarget: "./UI/wwwroot/es6/**/*.js",
    scriptDest: "./UI/wwwroot/scripts/build",
}



// for multiple files []
gulp.task("sass-all", function () {
    return utility.sass(config, ["**/**.scss"]);

});

// for watching and update effected file only 
gulp.task("WATCH-SCSS", function () {
    return gulp.watch([config.target + "**/**.scss"])
        .on("change", function (file) {
            return utility.sass(config, ['**/**.scss']);
        });
});

