/*import { fromEvent } from 'rxjs';
import { interval, merge } from 'rxjs';

const source = fromEvent(emitter, 'OnStartCasting');


const example = merge(
    source,
    second.pipe(mapTo('SECOND!')),
    third.pipe(mapTo('THIRD')),
    fourth.pipe(mapTo('FOURTH'))
);

example.subscribe((event) => {
    switch (event.event) {
        case 'OnStartCasting':
            break;
    }
});



var test = new Map();
test.set(0.00, "");


Timeline.run(event);

run(event) {

    cevent = map.get(current);
    cevent.compare(event);

}*/

//const { observable } = require('mobx')
const rootPath = require('electron-root-path').rootPath;


let arr = ['react', 'angular', 'vue']

// Correct
if (arr.includes('react')) {
    console.log('Can use React')
}