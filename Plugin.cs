using System;
using System.Reflection;
using HarmonyLib;
using VRage.Plugins;

namespace ChatFilter
{
	public class Plugin : IDisposable, IPlugin
	{
		public void Dispose()
		{
		}

		public void Init(object gameInstance)
		{
			new Harmony("ChatFilter").PatchAll(Assembly.GetExecutingAssembly());
		}

		public void Update()
		{
		}
	}
}