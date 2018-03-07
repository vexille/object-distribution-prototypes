using System;
using System.Collections.Generic;
using DistributionPrototype.Config;
using UnityEngine.UI;

namespace DistributionPrototype.Util
{
	public static class UiUtils
	{
		public static void PopulateDropboxWithEnum(Dropdown dropdown, Type enumType)
		{
			var options = new List<Dropdown.OptionData>();
			foreach (string valueName in Enum.GetNames(enumType))
			{
				options.Add(new Dropdown.OptionData(valueName));
			}

			dropdown.AddOptions(options);
		}
	}
}
