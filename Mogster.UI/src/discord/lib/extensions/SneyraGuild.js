
const { Structures } = require('discord.js');
const AudioManager = require('../structures/AudioManger');

const winston = require('winston');
const log = winston.createLogger({
    level: 'info',
    format: winston.format.simple(),
    transports: [
        new winston.transports.Console({ format: winston.format.simple() })
    ]
});

module.exports = Structures.extend('Guild', Guild => {
	/**
	 * Sneyra's Extended Guild
	 * @extends {Guild}
	 */
	class TestGuild extends Guild {

		/**
		 * @param {...*} args Normal D.JS Guild args
		 */
		constructor(...args) {
			super(...args);

			/**
			 * The AudioManager instance for this client
			 * @since 1.0.0
			 * @type {AudioManager}
			 */

			this.audio = new AudioManager(this);
		}

	}

    return TestGuild;
});