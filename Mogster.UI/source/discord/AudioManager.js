const fs = require('fs');

const winston = require('winston');
const log = winston.createLogger({
    level: 'info',
    format: winston.format.simple(),
    transports: [
        new winston.transports.Console({ format: winston.format.simple() })
    ]
});

class AudioManager {

	/**
	 * @typedef {Object} MusicManagerSong
	 * @property {string} url The video id
	 * @property {string} title The title of the video
	 * @property {number} loudness The loudness for this song, reserved for future
	 * @property {number} seconds The seconds this video lasts
	 * @property {boolean} opus Whether this video has an Opus stream available or not
	 */

    constructor(guild) {
		/**
		 * The Client that manages this instance
		 * @since 1.0.0
		 * @type {Sneyra}
		 * @name MusicManager#client
		 */
        Object.defineProperty(this, 'client', { value: guild.client });

		/**
		 * The SneyraGuild instance that manages this instance
		 * @since 1.0.0
		 * @type {SneyraGuild}
		 * @name MusicManager#guild
		 */
        Object.defineProperty(this, 'guild', { value: guild });
    }


	/**
	 * The VoiceChannel Sneyra is connected to
	 * @since 1.0.0
	 * @type {?VoiceChannel}
	 * @readonly
	 */
    /*get voiceChannel() {
        return this.guild.me.voice.channel;
    }*/
    get voiceChannel() {
        return this.guild.me.voice.channel;
    }

	/**
	 * The VoiceChannel's connection
	 * @since 1.0.0
	 * @type {?VoiceConnection}
	 * @readonly
	 */
    get connection() {
        const { voiceChannel } = this;
        return (voiceChannel && this.guild.me.voice.connection) || null;
    }

	/**
	 * The VoiceConnection's dispatcher
	 * @since 1.0.0
	 * @type {?StreamDispatcher}
	 * @readonly
	 */
    get dispatcher() {
        const { connection } = this;
        return (connection && connection.dispatcher) || null;
    }

	/**
	 * Whether Sneyra is playing a song or not
	 * @since 2.0.0
	 * @type {boolean}
	 * @readonly
	 */
    get playing() {
        return !this.paused && !this.idling;
    }

	/**
	 * Whether Sneyra has the queue paused or not
	 * @since 2.0.0
	 * @type {?boolean}
	 * @readonly
	 */
    get paused() {
        const { dispatcher } = this;
        return dispatcher ? dispatcher.paused : null;
    }

	/**
	 * Whether Sneyra is doing nothing
	 * @since 2.0.0
	 * @type {boolean}
	 * @readonly
	 */
    get idling() {
        return !this.queue.length || !this.dispatcher;
    }

	/**
	 * Join a voice channel, handling ECONNRESETs
	 * @since 1.0.0
	 * @param {VoiceChannel} voiceChannel Join a voice channel
	 * @returns {Promise<VoiceConnection>}
	 */
    join(voiceChannel) {
        return voiceChannel.join().catch((err) => {
            if (String(err).includes('ECONNRESET')) throw 'There was an issue connecting to the voice channel, please try again.';
            this.client.emit('error', err);
            throw err;
        });
    }

    setConnection(connection) {
        this.connection = connection;
    }
	/**
	 * Leave the voice channel, reseating all the current data
	 * @since 1.0.0
	 * @returns {Promise<this>}
	 */
    async leave() {
        if (!this.voiceChannel) throw 'I already left the voice channel! You might want me to be in one in order to leave it...';
        await this.voiceChannel.leave();
        if (this.voiceChannel) this.forceDisconnect();
    }

    async play(path) {
        if (!this.connection.channel) throw 'Where am I supposed to play the music? I am not in a voice channel!';
        if (!this.connection) {
            await this.channel.send(`This dj table isn't connected! Let me unplug and plug it again`)
                .catch(error => this.client.emit('error', error));

            const { voiceChannel } = this;
            this.forceDisconnect();
            await this.join(voiceChannel);
            if (!this.connection) throw 'This dj table is broken! Try again later...';
        }

        this.connection.play(path, {
            bitrate: this.voiceChannel.bitrate / 1000,
            passes: 2,
            volume: 0.5
        });

        return this.dispatcher;
    }

    pause() {
        const { dispatcher } = this;
        if (dispatcher) dispatcher.pause();
        return this;
    }

    resume() {
        const { dispatcher } = this;
        if (dispatcher) dispatcher.resume();
        return this;
    }

    stop() {
        const { dispatcher } = this;
        if (dispatcher) dispatcher.end();
        return this;
    }

    forceDisconnect() {
        const { connection } = this;
        if (connection) {
            connection.disconnect();
        } else {
            /* eslint-disable camelcase */
            this.guild.shard.send({
                op: 4,
                shard: this.client.shard ? this.client.shard.id : 0,
                d: {
                    guild_id: this.guild.id,
                    channel_id: null,
                    self_mute: false,
                    self_deaf: false
                }
            });
            /* eslint-enable camelcase */
        }
    }

}

module.exports.AudioManager = AudioManager;
