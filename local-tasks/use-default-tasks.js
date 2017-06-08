const gulp = requireModule("gulp-with-help");

gulp.task("install-tools", ["default-tools-installer"]);
gulp.task("generate-reports", ["default-report-generator"]);