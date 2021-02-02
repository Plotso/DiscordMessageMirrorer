# DiscordMessageMirrorer
Very simple Discord bot written in C#. Its main idea is to mirror messages from one to one channel to N other channels.

Since the bot was requested almost immediately, it has very simple structure and it's very easy and intuitive to use.

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
In order to run/host the bot, you need to have valid token from [Discord Developer Applications Page](https://discord.com/developers/applications). 
You can check this [detailed guide on how to obtain bot token](https://www.writebots.com/discord-bot-token/)

In order to run the project, make sure you have the following framework(s):
* **NET Core 3.1** - should be already installed by VisualStudio, Rider or any other application that uses it. Just in case, check to be sure you have it.

It's good to have the following software products installed in order to be sure the project is running as expected:
* **VisualStudio 2019 / Rider 2020** - built and tested on both of those IDEs, the project should also be running on any newer version as long as it supports the above mentioned frameworks

### Installation / Usage

If you want to run the bot, you need to adjust it's configuration. This can be done in [config.json](MessageMirrorerBot/config.json) file. There you have 3 main sections:

* **Token** - Here you should insert the token mentioned in the prerequisites section
* **CommandPrefix** - The prefix to be used for commands. By default it's '~'. Example usage: ~help
* **Rules** - Here you can set manually the rules that are to be used for mirroring. The bot will dynamically update them from comands on later stage.

And here is the structure of the config:
```
{
  "Token": "TOKEN_PLACEHOLDER", 
  "CommandPrefix": "~",  
  "Rules": [
    {
      "SourceChannelId": 123,  // NOTE: The channel id FROM which you want to mirror messages to below channels
      "DestinationChannelIds": [  // NOTE: The ids of channel TO which you want to mirror messages from above channel
        654,
        321
      ]
    }
  ]
}
```

### Bot Commands
Brief overview of default commands that can trigger some bot action. They can be changed from [here](MessageMirrorerBot/CommandConstants.cs)

**Each command should be prefixed by the prefix from the configuration!**  Example: ~help / ~channelId and so on.

* **help** -> shows dynamic commands template.
* **channelId** ->  shows the id of the current channel.
* **channelDestinations** ->  shows a list of all ids of channels, to which messages from current one are being mirrored.
* **addDestination destionationChannelId** -> adds new destination channel for messages from current one. In place of destionationChannelId you must write the id of the desired channel.
* **removeDestination destionationChannelId** -> if the channel is configured as a destionation for message from current one, it will be removed from destinations list. In place of destionationChannelId you must write the id of the desired channel.

## Support

<a href="https://www.buymeacoffee.com/i.ganchosov" target="_blank">☕ Buying me a coffee is great way to show support if you find this project useful.</a>
