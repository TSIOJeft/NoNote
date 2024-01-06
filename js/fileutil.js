const fs = require('browserify-fs');
function getFile(filePath) {
 
            fs.readFile('/home/hello-world.txt', 'utf-8', function(err, data) {
                console.log(data);
            });
}
function saveFile(filePath, text) {

    fs.writeFile(filePath, text, (err) => {
        if (err) {
            console.error('Error writing file:', err);
        } else {
            console.log('File saved successfully!');
        }
    });
}
exports.saveFile = saveFile
exports.getFile = getFile