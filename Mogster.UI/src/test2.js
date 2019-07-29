const { NodeVM } = require('vm2');
const fs = require('fs')
const path = require('path');

const vm = new NodeVM({
    require: {
        builtin: ['path'],
        resolve: moduleName => path.resolve(directory, moduleName),
        external: true,
        root: './'
       
    }
});

/*vm.run(`
    var request = require('request');
    request('http://www.google.com', function (error, response, body) {
        console.error(error);
        if (!error && response.statusCode == 200) {
            console.log(body) // Show the HTML for the Google homepage.
        }
    })
`, 'vm.js');*/

console.log(path.resolve(__dirname, 'vm2.js'));

vm.run(`
    const path = require('path');
//var request = require('request');    
//const fs = require('fs');
    const test = require('test4');

    function listDirectories(source) {
        return fs.readdirSync(source, { withFileTypes: true })
            .filter(dirent => dirent.isDirectory())
            .map(dirent => dirent.name);
    }
    listDirectories(path.resolve('../../'));
    console.log(path.resolve('../../'));
`, path.resolve(__dirname, 'vm2.js'));
