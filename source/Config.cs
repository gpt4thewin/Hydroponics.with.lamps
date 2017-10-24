using HugsLib.Settings;

namespace WM.AllInOnePonics
{
	class Config
	{
		public static SettingHandle<bool> InjectBuiltinSunlamps { get; internal set; }
		public static SettingHandle<bool> InjectDefaultPlantSetter { get; internal set; }
	}
}