﻿// Copyright (c) 2017-2019 dirigeants. All rights reserved. MIT license.
const { Command } = require('klasa');
const { MessageAttachment } = require('discord.js');
const fetch = require('node-fetch');

module.exports = class extends Command {

	constructor(...args) {
		super(...args, {
			cooldown: 5,
			description: 'Your DPS from the last fight',
			usageDelim: ' '
		});
	}

	async run(msg) {
        throw 'Not implemented yet.'
	}

};