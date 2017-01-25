using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextSentenceBot
{
    public class MyBot
    {
        // VARIABLES
        DiscordClient discord;
        CommandService commands;
        string Player1;
        string Player2;
        string Player3;
        string Player4;
        string Player5;
        string Player6;
        string Player7;
        string Player8;
        string Player9;
        string Player10;
        string CurrentPlayer;
        List<string> PlayerList = new List<string>();
        int NumberOfPlayers;
        int PlayersTurn;
        Discord.Server GameServer;
        Discord.Channel StoryChannel;

        // Main function
        public MyBot()
        {
            // Creates client and enables basic Logging
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Debug;
                x.LogHandler = Log;
            });

            // Sets bot's prefix and allows it to react to mention
            discord.UsingCommands(x =>
            {
                x.PrefixChar = '~';
                x.AllowMentionPrefix = true;
            });

            // Defines something 
            commands = discord.GetService<CommandService>();

            // Loads other funtctions
            //DeleteMessages();
            NextSentencePlayers();
            StartNextSentence();
            WriteStory();
            ClearCache();

            // Here comes bot's token
            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("DiscordBotTokenHere", TokenType.Bot);
            });
        }

        /*private void DeleteMessages()
        {
            commands.CreateCommand("delete")
                .Do(async (e) =>
                {
                    Message[] messagesToDelete;
                    messagesToDelete = await e.Channel.DownloadMessages(1);

                    await e.Channel.DeleteMessages(messagesToDelete);
                });
        }*/

        // Define players by mentioning them. E.g.: ~playerlist @Updooted#123 @WoahWoah#6582 or ~playerlist which returns playerlist
        private void NextSentencePlayers()
        {
            commands.CreateCommand("playerlist")
                .Parameter("Player1", ParameterType.Optional)
                .Parameter("Player2", ParameterType.Optional)
                .Parameter("Player3", ParameterType.Optional)
                .Parameter("Player4", ParameterType.Optional)
                .Parameter("Player5", ParameterType.Optional)
                .Parameter("Player6", ParameterType.Optional)
                .Parameter("Player7", ParameterType.Optional)
                .Parameter("Player8", ParameterType.Optional)
                .Parameter("Player9", ParameterType.Optional)
                .Parameter("Player10", ParameterType.Optional)
                .Do(async (e) =>
                {
                    if (e.Message.MentionedUsers.Count() == 0)
                    {
                        await e.Channel.SendMessage("Number of players is " + NumberOfPlayers);
                        await e.Channel.SendMessage("Participating players are " + Player1 + " " + Player2 + " " + Player3 + " " + Player4 + " " + Player5 + " " + Player6 + " " + Player7 + " " + Player8 + " " + Player9 + " " + Player10);
                        return;
                    }
                    NumberOfPlayers = e.Message.MentionedUsers.Count();
                    if (NumberOfPlayers >= 1) { Player1 = e.GetArg("Player1"); PlayerList.Add(Player1); }
                    if (NumberOfPlayers >= 2) { Player2 = e.GetArg("Player2"); PlayerList.Add(Player2); }
                    if (NumberOfPlayers >= 3) { Player3 = e.GetArg("Player3"); PlayerList.Add(Player3); }
                    if (NumberOfPlayers >= 4) { Player4 = e.GetArg("Player4"); PlayerList.Add(Player4); }
                    if (NumberOfPlayers >= 5) { Player5 = e.GetArg("Player5"); PlayerList.Add(Player5); }
                    if (NumberOfPlayers >= 6) { Player6 = e.GetArg("Player6"); PlayerList.Add(Player6); }
                    if (NumberOfPlayers >= 7) { Player7 = e.GetArg("Player7"); PlayerList.Add(Player7); }
                    if (NumberOfPlayers >= 8) { Player8 = e.GetArg("Player8"); PlayerList.Add(Player8); }
                    if (NumberOfPlayers >= 9) { Player9 = e.GetArg("Player9"); PlayerList.Add(Player9); }
                    if (NumberOfPlayers >= 10) { Player10 = e.GetArg("Player10"); PlayerList.Add(Player10); }
                    await e.Channel.SendMessage("Number of players is " + NumberOfPlayers);
                    await e.Channel.SendMessage("Participating players are " + e.GetArg("Player1") + " " + e.GetArg("Player2") + " " + e.GetArg("Player3") + " " + e.GetArg("Player4") + " " + e.GetArg("Player5") + " " + e.GetArg("Player6") + " " + e.GetArg("Player7") + " " + e.GetArg("Player8") + " " + e.GetArg("Player9") + " " + e.GetArg("Player10"));
                });
        }

        // Starts the game itself
        private void StartNextSentence()
        {
            Discord.User MessagedPlayer;
            Discord.User PreviousPlayer;
            commands.CreateCommand("start")
                .Do(async (e) =>
                {
                    //Checks from which cahnnel the message was sent. Not sure why I made this just yet.
                    if (e.Channel.Name.Equals("story"))
                    {
                        await e.Channel.SendMessage("Everything's set up, game is starting. Get ready and get out of this channel.");
                    }
                    else
                    {
                        try
                        {
                            var DoesStoryExist = e.Server.FindChannels("story", ChannelType.Text).FirstOrDefault().ToString();
                        }
                        catch
                        {
                            await e.Channel.SendMessage("Story Channel does not exist, attempting to create it.");
                            await e.Server.CreateChannel("story", ChannelType.Text);

                        }
                        await e.Channel.SendMessage("Yeah boyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy game is starting!");
                    }
                    StoryChannel = e.Server.FindChannels("story", ChannelType.Text).FirstOrDefault();
                    GameServer = e.Server;

                    //Sends PM to all players informing them of rules.
                    PlayersTurn = 0;
                    var FirstPlayer = PlayerList[PlayersTurn];
                    CurrentPlayer = FirstPlayer;
                    MessagedPlayer = e.Server.FindUsers(FirstPlayer).FirstOrDefault();
                    await e.Channel.SendMessage(MessagedPlayer.ToString());
                    var LastPlayer = PlayerList[NumberOfPlayers - 1];
                    PreviousPlayer = e.Server.FindUsers(LastPlayer).FirstOrDefault();
                    await MessagedPlayer.SendMessage("You are going first! Write your first sentence with command ~story [Your text] and then wait until you get another sentence from " + PreviousPlayer + ". If you wish to end the story, type last sentence with ~end [Your Text]. Have fun!");
                    for (int i = 1; i <= NumberOfPlayers; i++)
                    {
                        var MessagedPlayerFromList = PlayerList[i];
                        var PreviousPlayerFromList = PlayerList[i - 1];
                        MessagedPlayer = e.Server.FindUsers(MessagedPlayerFromList).FirstOrDefault();
                        PreviousPlayer = e.Server.FindUsers(PreviousPlayerFromList).FirstOrDefault();
                        await MessagedPlayer.SendMessage("Get in here! Wait until I send you message from " + PreviousPlayer + ". Then write another sentence with ~story [Your text] or end story with ~end [Your text]. Have fun!");
                    }
                });
        }

        //continue with story to story channel and pms the next player last sentence.
        private void WriteStory()
        {
            commands.CreateCommand("story")
                .Parameter("sentence", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    var AuthorOfCommand = e.User.Mention;
                    if (AuthorOfCommand == CurrentPlayer)
                    {
                        if (PlayersTurn < NumberOfPlayers - 1)
                        {
                            var PlayerToMessageFromList = PlayerList[PlayersTurn + 1];
                            var PlayerToMessage = GameServer.FindUsers(PlayerToMessageFromList).FirstOrDefault();
                            await PlayerToMessage.SendMessage(e.GetArg("sentence"));
                            await StoryChannel.SendMessage(e.GetArg("sentence"));
                            PlayersTurn++;
                        }
                        else
                        {
                            PlayersTurn = 0;
                            var FirstToMessageFromList = PlayerList[PlayersTurn];
                            var FirstToMessage = GameServer.FindUsers(FirstToMessageFromList).FirstOrDefault();
                            await FirstToMessage.SendMessage(e.GetArg("sentence"));
                            await StoryChannel.SendMessage(e.GetArg("sentence"));
                        }
                        CurrentPlayer = PlayerList[PlayersTurn];
                        await e.User.SendMessage("Message sent to story channel and to " + CurrentPlayer);
                    }
                    else
                    {
                        await e.User.SendMessage("It is not your turn! Please wait until you receive another Message!");
                        return;
                    }
                });
        }

        private void ClearCache()
        {
            commands.CreateCommand("clear")
                .Do(async (e) =>
                {
                    //Doesnt work just yet? WHY :(
                    PlayerList.Clear();
                    await e.Channel.SendMessage("Cache cleared!");
                });
        }

        /*private void GetUzivatele()
        {
            commands.CreateCommand("greet") //create command greet
        .Alias(new string[] { "gr", "hi" }) //add 2 aliases, so it can be run with ~gr and ~hi
        .Description("Greets a person.") //add description, it will be shown when ~help is used
        .Parameter("GreetedPerson", ParameterType.Required) //as an argument, we have a person we want to greet
        .Do(async e =>
        {
            await e.Channel.SendMessage(e.User.Name + " greets " + e.GetArg("GreetedPerson")); //sends a message to channel with the given text
        });
        }*/

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

