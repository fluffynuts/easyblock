const 
    gulp = requireModule("gulp-with-help"),
    editXml = require("gulp-edit-xml"),
    zipFolder = require("zip-folder");

function findVersionIn(propertyGroup) {
    if (propertyGroup.Version && propertyGroup.Version.length) {
        return (propertyGroup.Version[0] || "").trim();
    }
}

gulp.task("create-release-zip", () => {
    return new Promise((resolve, reject) => {
        gulp.src("source/EasyBlock.Win32Service/EasyBlock.Win32Service.csproj")
        .pipe(editXml(async (xml) => {
                const 
                    version = xml.Project.PropertyGroup.reduce(
                        (acc, cur) => acc || findVersionIn(cur),
                        null);
                const zipFile = `releases/EasyBlock-${version}.zip`;
                zipFolder(
                    "source/EasyBlock.Win32Service/bin/Release/net452", 
                    `releases/EasyBlock-${version}.zip`, err => {
                    if (err) {
                        return reject(err);
                    }
                    resolve();
                });
            })
        );
    });
});
