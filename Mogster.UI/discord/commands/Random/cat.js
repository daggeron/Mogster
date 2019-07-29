// Copyright (c) 2017-2019 dirigeants. All rights reserved. MIT license.
const { Command } = require('klasa');
const { MessageAttachment } = require('discord.js');
const fetch = require('node-fetch');

module.exports = class extends Command {

	constructor(...args) {
		super(...args, {
			cooldown: 5,
			requiredPermissions: ['ATTACH_FILES'],
			description: 'Random cat image',
			usageDelim: ' '
		});
	}

	async run(msg) {
        const image = await fetch('https://cataas.com/cat')
			.then(response => response.buffer())
			.catch(() => {
				throw 'I could not download the file. Can you try again with another image?';
			});

		return msg.sendMessage(new MessageAttachment(image, `${Math.round(Math.random() * 10000)}.jpg`));
	}

};
