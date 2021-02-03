using HarmonyLib;

using PlayerControl = FFGALNAPKCD;
using PlayerTab = MAOILGPNFND;
using GameData = EGLJNOMOGNP;
using Palette = LOCPGOACAJF;
using PingTracker = ELDIDNABIPI;
using ShipStatus = HLBNNHFCNAJ;

namespace CrowdedMod.Patches {
	static class GenericPatches {
		// patched because 10 is hardcoded in `for` loop
        [HarmonyPatch(typeof(GameData), nameof(GameData.GetAvailableId))]
		static class GameDataAvailableIdPatch {
			public static bool Prefix(ref GameData __instance, out sbyte __result) {
				for (sbyte i = 0; i <= 127; i++)
					if (checkId(__instance, i)) {
						__result = i;
						return false;
					}
				__result = -1;
				return false;
			}

			static bool checkId(GameData __instance, int id) {
				foreach (var p in __instance.AllPlayers)
					if (p.JKOMCOJCAID == id)
						return false;
				return true;
			}
		}

		[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor), typeof(byte))]
		static class PlayerControlCheckColorPatch {
			public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte colorId) {
				__instance.RpcSetColor(colorId);
				return false;
			}
		}

		[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
		static class PlayerTabUpdateAvailableColorsPatch {
			public static bool Prefix(PlayerTab __instance) {
				PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.NDGFFHMFGIG.EHAHBDFODKC, __instance.DemoImage);
				for (int i = 0; i < Palette.OPKIKLENHFA.Length; i++)
					__instance.LGAIKONLBIG.Add(i);
				return false;
			}
		}
		
		[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.GetSpawnLocation))]
        	public static class ShipStatusGetSpawnLocationPatch
		{
		    public static void Prefix(ShipStatus __instance, [HarmonyArgument(0)] ref int playerId, [HarmonyArgument(1)] ref int numPlayer)
		    {
			playerId %= 10;
			if (numPlayer > 10) numPlayer = 10;
		    }
		}
	
        [HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        static class PingShowerPatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.Text += "\n[FFB793FF]> CrowdedMod <";
            }
        }
		
	[HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Start))]
	public static class CreateOptionsPickerStartPatch
	{
		public static void Postfix(CreateOptionsPicker __instance)
		{
			var offset = __instance.MaxPlayerButtons[1].transform.position.x -
			             __instance.MaxPlayerButtons[0].transform.position.x;
			var playerButtons = __instance.MaxPlayerButtons.ToList();
				
			var plusButton = Object.Instantiate(playerButtons.Last(), playerButtons.Last().transform.parent);
			plusButton.GetComponentInChildren<TextRenderer>().Text = "+";
			plusButton.name = "255";
			plusButton.transform.position = playerButtons.Last().transform.position + new Vector3(offset*2, 0, 0);
			var passiveButton = plusButton.GetComponent<PassiveButton>();
			passiveButton.OnClick.m_PersistentCalls.Clear();
			passiveButton.OnClick.AddListener((UnityAction)plusListener);
			void plusListener()
			{
				var targetOptions = __instance.GetTargetOptions();
				targetOptions.MaxPlayers = Mathf.Clamp(targetOptions.MaxPlayers + 1, 4, 20);
				plusButton.name = targetOptions.MaxPlayers.ToString();
				__instance.SetMaxPlayersButtons(targetOptions.MaxPlayers);
			}
			var minusButton = Object.Instantiate(playerButtons.Last(), playerButtons.Last().transform.parent);
			minusButton.GetComponentInChildren<TextRenderer>().Text = "-";
			minusButton.name = "255";
			minusButton.transform.position = playerButtons.First().transform.position;
			var minusPassiveButton = minusButton.GetComponent<PassiveButton>();
			minusPassiveButton.OnClick.m_PersistentCalls.Clear();
			minusPassiveButton.OnClick.AddListener((UnityAction)listener);
			void listener()
			{
				var targetOptions = __instance.GetTargetOptions();
				targetOptions.MaxPlayers = Mathf.Clamp(targetOptions.MaxPlayers - 1, 4, 20);
				minusButton.name = targetOptions.MaxPlayers.ToString();
				__instance.SetMaxPlayersButtons(targetOptions.MaxPlayers);
			}
			playerButtons.ForEach(x => x.transform.position += new Vector3(offset, 0, 0));
			playerButtons.Insert(0, minusButton);
			playerButtons.Add(plusButton);
			__instance.MaxPlayerButtons = playerButtons.ToArray();
		}
	}
    }
}
