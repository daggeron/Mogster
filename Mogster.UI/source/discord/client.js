const Discord = require('discord.js');
const { AudioManager } = require('./AudioManager');

class AdjutantDiscord {
    constructor(token, guildid, channelid, audioPath) {
        this.token = token;
        this.guildid = guildid;
        this.channelid = channelid;
        this.audioPath = audioPath;
        this.client = new Discord.Client();
        this.connection = {};
        this.AudioManager = {};
    }

    async login() {
        await this.client.login(this.token);
    }

    async play(path) {
        this.AudioManager.play(path.resolve(this.audioPath, path));
    }

    async stop() {
        this.AudioManager.stop();
    }

    async join() {
        await this.client.fetchApplication();
        let guild = this.client.guilds.get(this.guildid);
        this.AudioManager = new AudioManager(this.guildid);
        let channel = this.client.guilds.get(this.guildid).channels.get(this.channelid);
        await this.AudioManager.join(channel);
    }

    async disconnect() {
        this.client.destroy();
    }
    
}

module.exports.AdjutantDiscord = AdjutantDiscord;
