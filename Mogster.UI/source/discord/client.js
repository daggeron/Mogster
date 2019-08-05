const Discord = require('discord.js');
const { resolve } = require('path');
const { AudioManager } = require('./AudioManager');

class AdjutantDiscord {
    constructor(token, guildid, channelid, audioPath) {
        this.token = token;
        this.guildid = guildid;

        this.channelid = channelid;
        this.audioPath = audioPath;

        this.client = new Discord.Client();
        this.AudioManager = {};
    }

    async login() {
        await this.client.login(this.token);
    }

    async play(path) {
        this.AudioManager.play(resolve(this.audioPath, path));
    }

    async stop() {
        this.AudioManager.stop();
    }

    async join() {
        await this.client.fetchApplication();

        let guild = this.client.guilds.get(this.guildid);
        let channel = guild.channels.get(this.channelid);

        this.AudioManager = new AudioManager(guild);
        await this.AudioManager.join(channel);
    }

    async disconnect() {
        this.client.destroy();
    }
    
}

module.exports.AdjutantDiscord = AdjutantDiscord;
