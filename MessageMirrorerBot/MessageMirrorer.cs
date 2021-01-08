namespace MessageMirrorerBot
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class MessageMirrorer
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private BotConfig _config;

        public MessageMirrorer()
        {
            _client = new DiscordSocketClient();
            _commandService = new CommandService();
        }

        public async Task RunAsync()
        {
            await LoadConfigAsync();
            ValidateConfig();

            _client.Log += OnLogMessage;
            _client.MessageReceived += OnMessageReceived;

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private void ValidateConfig()
        {
            if (string.IsNullOrEmpty(_config.Token) || _config.Token == "TOKEN_PLACEHOLDER")
            {
                throw new InvalidOperationException("Token not provided!");
            }
        }

        private async Task LoadConfigAsync()
        {
            using FileStream openStream = File.OpenRead("config.json");
            _config = await JsonSerializer.DeserializeAsync<BotConfig>(openStream);
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (_config.Rules.Any(r => r.SourceChannelId == message.Channel.Id) &&
                !message.Author.IsBot)
            {
                var socketUserMessage = message as SocketUserMessage;
                int argPos = 0;
                //If message is recognised as command, execute it. Otherwise, echo it.
                if (socketUserMessage.HasCharPrefix(_config.CommandPrefix, ref argPos) && !socketUserMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    await HandleCommand(socketUserMessage);
                    return;
                }

                var destionationChannels = _config.Rules.FirstOrDefault(c => c.SourceChannelId == message.Channel.Id).DestinationChannelIds;
                // Echo the message to all destination channels
                if (!destionationChannels.Any())
                {
                    await message.Channel.SendMessageAsync("Destionation channel is not configured!");
                    return;
                }
                foreach (var channelId in destionationChannels)
                {
                    var channel = _client.GetChannel(channelId) as ISocketMessageChannel;
                    await channel.SendMessageAsync($"[{message.Author}]{message}");
                }
            }
            else if(!_config.Rules.Any(r => r.SourceChannelId == message.Channel.Id))
            {
                var rules = _config.Rules.ToList();
                rules.Add(new MirrorRule
                {
                    SourceChannelId = message.Channel.Id,
                    DestinationChannelIds = new ulong[0]
                });
            }
        }

        private async Task HandleCommand(SocketUserMessage message)
        {
            const char spaceChar = ' ';
            var lengthOfCommand = -1;
            if (message.Content.Contains(spaceChar))
            {
                lengthOfCommand = message.Content.IndexOf(spaceChar);
            }
            else
            {
                lengthOfCommand = message.Content.Length;
            }

            var command = message.Content.Substring(1, lengthOfCommand - 1);
            if (command.StartsWith(CommandConstants.Help))
            {              
                await message.Channel.SendMessageAsync(GetHelpMessage());
            }
            if (command.StartsWith(CommandConstants.GetChannelId))
            {
                await message.Channel.SendMessageAsync($"Current channel id is: {message.Channel.Id}");
            }
            var rule = _config.Rules.FirstOrDefault(c => c.SourceChannelId == message.Channel.Id);

            if (command.StartsWith(CommandConstants.GetDestionationForCurrentChannel))
            {
                if (rule != null)
                {
                    await message.Channel.SendMessageAsync($"Current channel is mirrored to following channels: {string.Join(", ", rule.DestinationChannelIds)}");
                }
            }

            if (command.StartsWith(CommandConstants.AddDestinationCommands) && lengthOfCommand != message.Content.Length)
            {
                var destination = message.Content.Substring(lengthOfCommand + 1);
                if (ulong.TryParse(destination, out ulong channelId))
                {
                    var destinationChannels = rule.DestinationChannelIds.ToList();
                    destinationChannels.Add(channelId);
                    rule.DestinationChannelIds = destinationChannels.ToArray();

                    await message.Channel.SendMessageAsync($"I will now mirror messages to {channelId} ^^");
                }
                else
                {
                    await message.Channel.SendMessageAsync($"Invalid Channel Id");
                }
            }
            if (command.StartsWith(CommandConstants.RemoveDestinationCommands) && lengthOfCommand != message.Content.Length)
            {
                var destination = message.Content.Substring(lengthOfCommand + 1);
                if (ulong.TryParse(destination, out ulong channelId))
                {
                    var destinationChannels = rule.DestinationChannelIds.ToList();
                    if (destinationChannels.Contains(channelId))
                    {
                        destinationChannels.Remove(channelId);
                        rule.DestinationChannelIds = destinationChannels.ToArray();
                        await message.Channel.SendMessageAsync($"I will no longer mirror messages to {channelId}...");
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync($"The Channel Id you provided me is not valid :(");
                }
            }
        }

        private Task OnLogMessage(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private string GetHelpMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Hello, I'm {nameof(MessageMirrorer)} and I can send messages from one channel to another. Here are my commands: ");
            sb.AppendLine("```");
            sb.AppendLine($"{_config.CommandPrefix}{CommandConstants.Help} -> I'll show commands template");
            sb.AppendLine();
            sb.AppendLine($"{_config.CommandPrefix}{CommandConstants.GetChannelId} ->  I'll show the Id of this channel");
            sb.AppendLine();
            sb.AppendLine($"{_config.CommandPrefix}{CommandConstants.GetDestionationForCurrentChannel} ->  I'll show the list of channel ids to which messages from here are being mirrored.");
            sb.AppendLine();
            sb.AppendLine($"{_config.CommandPrefix}{CommandConstants.AddDestinationCommands} destionationChannelId -> I'll add new channel to which messages can be mirrored. In place of destionationChannelId you must write the id of the desired channel.");
            sb.AppendLine();
            sb.AppendLine($"{_config.CommandPrefix}{CommandConstants.RemoveDestinationCommands} destionationChannelId -> I'll remove the channel if I'm currently mirroring messages to it. In place of destionationChannelId you must write the id of the desired channel.");
            sb.AppendLine("```");
            return sb.ToString();
        }
    }
}
