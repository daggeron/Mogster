const { Client, util } = require('klasa');
const path = require('path');

const winston = require('winston');

var token = process.argv[2];
var audioFolder = process.argv[3];
var channelID = process.argv[4];
var guildID = process.argv[5];

const log = winston.createLogger({
    level: 'info',
    format: winston.format.simple(),
    transports: [
        new winston.transports.Console({ format: winston.format.simple() })
    ]
});

require('./lib/extensions/SneyraGuild');

Client.defaultPermissionLevels
    .add(5, (msg) => msg.member && msg.guild.settings.dj && msg.member.roles.has(msg.guild.settings.dj), { fetch: true })
    .add(6, (msg) => msg.member
        && ((msg.guild.settings.administrator && msg.member.roles.has(msg.guild.settings.administrator))
            || msg.member.permissions.has('MANAGE_GUILD')), { fetch: true });

const client = new Client({
    disabledEvents: [
        'GUILD_BAN_ADD',
        'GUILD_BAN_REMOVE',
        'TYPING_START',
        'CHANNEL_PINS_UPDATE',
        'PRESENCE_UPDATE',
        'USER_UPDATE',
        'MESSAGE_REACTION_ADD',
        'MESSAGE_REACTION_REMOVE',
        'MESSAGE_REACTION_REMOVE_ALL'
    ],
    prefix: '!',
    presence: { activity: { name: 'Final Fantasy 14', type: 'WATCHING' } },
    ownerID: 96064760688148480
});

var guild2 = {};

client.addListener("klasaReady", async () => {

    var channel = client.channels.get(channelID)
    
    guild2 = client.guilds.get(guildID);

    guild2.audio.join(channel);
});

process.on('message', message => {
    log.info(message);
    if (message.hasOwnProperty("event")) {
        try {
            let guild2 = client.guilds.get(guildID);

            guild2.audio.play(path.resolve(audioFolder, message.payload));
        } catch (error) {
            log.error(error);
        }
    }
});


client.login(token);
