using HarmonyLib;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.GameSystems.Chat;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace ChatFilter
{
	[HarmonyPatch]
	public class ChatFilter
	{
		static ChatFilter()
		{
			Settings = new CFSettings();
		}

		public ChatFilter()
		{
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(MyChatCommandSystem), MethodType.Constructor)]
		public static void AddCommands(MyChatCommandSystem __instance)
		{
			IMyChatCommand myChatCommand = (IMyChatCommand)Activator.CreateInstance(typeof(ChatCmd_Mute).GetTypeInfo());
			if (myChatCommand != null)
			{
				__instance.ChatCommands.Add(myChatCommand.CommandText, myChatCommand);
			}
			IMyChatCommand myChatCommand2 = (IMyChatCommand)Activator.CreateInstance(typeof(ChatCmd_Unmute).GetTypeInfo());
			if (myChatCommand2 != null)
			{
				__instance.ChatCommands.Add(myChatCommand2.CommandText, myChatCommand2);
			}
			IMyChatCommand myChatCommand3 = (IMyChatCommand)Activator.CreateInstance(typeof(ChatCmd_Config).GetTypeInfo());
			if (myChatCommand3 != null)
			{
				__instance.ChatCommands.Add(myChatCommand3.CommandText, myChatCommand3);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(MyHudChat), "OnMultiplayer_ChatMessageReceived")]
		public static bool ChatRecievedPrefix(ulong steamUserId, string messageText, ChatChannel channel, long targetId, string customAuthorName = null)
		{
			if (MySession.Static.IsUserAdmin(steamUserId))
			{
				return true;
			}
			if (!Settings.HideServer && steamUserId == MyMultiplayer.Static.ServerId)
			{
				return true;
			}
			if (Settings.HideServer && steamUserId == MyMultiplayer.Static.ServerId)
			{
				return false;
			}
			if (Settings.HideGlobal && channel == ChatChannel.Global)
			{
				return false;
			}
			string item = MySession.Static.Players.TryGetIdentityNameFromSteamId(steamUserId);
			return !Settings.BlockedNames.Contains(item) && (!Settings.HideFaction || channel != ChatChannel.Faction) && (!Settings.HidePrivate || channel != ChatChannel.Private);
		}

		public static void LoadSettings()
		{
			if (!File.Exists(path))
			{
				Settings = new CFSettings();
				SaveSettings();
				return;
			}
			StreamReader streamReader = new StreamReader(path);
			try
			{
				Settings = (CFSettings)serializer.Deserialize(streamReader);
			}
			catch (Exception ex)
			{
				MyLog.Default.WriteLine(ex);
				Settings = new CFSettings();
				SaveSettings();
			}
			if (streamReader != null)
			{
				streamReader.Close();
			}
		}

		public static void SaveSettings()
		{
			if (Settings == null)
			{
				Settings = new CFSettings();
			}
			FileStream fileStream = File.Create(path);
			try
			{
				serializer.Serialize(fileStream, Settings);
			}
			catch (Exception ex)
			{
				MyLog.Default.WriteLine(ex);
			}
			fileStream.Close();
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(MyHudChat), "multiplayer_ScriptedChatMessageReceived")]
		public static bool ScriptedChatRecievedPrefix(string message, string author, string font, Color color)
		{
			if (color == Color.Purple)
			{
				return true;
			}
			if (Settings.HideGlobal && author != "Server" && author != "Good.bot")
			{
				return false;
			}
			if (Settings.HideServer && (author == "Server" || author == "Good.bot"))
			{
				return false;
			}
			foreach (string value in Settings.BlockedNames)
			{
				if (author.Contains(value))
				{
					return false;
				}
			}
			return true;
		}

		private static readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineers\\ChatFilter.cfg");

		private static readonly XmlSerializer serializer = new XmlSerializer(typeof(CFSettings));

		public static CFSettings Settings;
	}

		public class CFSettings
		{
			public CFSettings()
			{
			}

			public List<string> BlockedNames = new List<string>();

			public bool HideFaction;

			public bool HideGlobal;

			public bool HidePrivate;

			public bool HideServer;
		}
	

	public class ChatCmd_Config : IMyChatCommand
	{
		public ChatCmd_Config()
		{
		}

		public void Handle(string[] args)
		{
			MyGuiSandbox.AddScreen(new CFConfig());
		}

		public string CommandText
		{
			get
			{
				return "/cfconfig";
			}
		}

		public string HelpSimpleText
		{
			get
			{
				return "cfconfig";
			}
		}

		public string HelpText
		{
			get
			{
				return "Open Chat Filter configuration screen.";
			}
		}

		public MyPromoteLevel VisibleTo
		{
			get
			{
				return MyPromoteLevel.None;
			}
		}
	}

	public class ChatCmd_Mute : IMyChatCommand
	{
		public ChatCmd_Mute()
		{
		}

		public void Handle(string[] args)
		{
			if (args != null && args.Length != 0)
			{
				string text = args[0];
				for (int i = 1; i < args.Length; i++)
				{
					text = text + " " + args[i];
				}
				if (!string.IsNullOrEmpty(text) && !ChatFilter.Settings.BlockedNames.Contains(text))
				{
					ChatFilter.Settings.BlockedNames.Add(text);
					MyHud.Chat.ShowMessage("ChatFilter", "You will no longer see messages from " + text, Color.SlateGray);
					ChatFilter.SaveSettings();
					return;
				}
			}
			else
			{
				string text2 = "Muted players: ";
				foreach (string str in ChatFilter.Settings.BlockedNames)
				{
					text2 = text2 + str + ", ";
				}
				MyHud.Chat.ShowMessage("ChatFilter", text2, Color.SlateGray);
			}
		}

		public string CommandText
		{
			get
			{
				return "/mute";
			}
		}

		public string HelpSimpleText
		{
			get
			{
				return "/mute [player name]";
			}
		}

		public string HelpText
		{
			get
			{
				return "/mute [player name] - Mutes a players chat messages.";
			}
		}

		public MyPromoteLevel VisibleTo
		{
			get
			{
				return MyPromoteLevel.None;
			}
		}
	}

	public class ChatCmd_Unmute : IMyChatCommand
	{
		public ChatCmd_Unmute()
		{
		}

		public void Handle(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				string text = "Muted players: ";
				foreach (string str in ChatFilter.Settings.BlockedNames)
				{
					text = text + str + ", ";
				}
				MyHud.Chat.ShowMessage("ChatFilter", text, Color.SlateGray);
				return;
			}
			string text2 = args[0];
			for (int i = 1; i < args.Length; i++)
			{
				text2 = text2 + " " + args[i];
			}
			if (ChatFilter.Settings.BlockedNames.Remove(text2))
			{
				MyHud.Chat.ShowMessage("ChatFilter", text2 + " has been unmuted.", Color.SlateGray);
				ChatFilter.SaveSettings();
				return;
			}
			MyHud.Chat.ShowMessage("ChatFilter", "That player is not currently muted.", Color.SlateGray);
		}

		public string CommandText
		{
			get
			{
				return "/unmute";
			}
		}

		public string HelpSimpleText
		{
			get
			{
				return "/unmute [player name]";
			}
		}

		public string HelpText
		{
			get
			{
				return "/unmute [player name] - Unmutes a player.";
			}
		}

		public MyPromoteLevel VisibleTo
		{
			get
			{
				return MyPromoteLevel.None;
			}
		}
	}
}
