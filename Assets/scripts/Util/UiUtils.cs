using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DistributionPrototype.Util
{
	public static class UiUtils
	{
		/// <summary>
		/// Gets all names from <paramref name="enumType"/> and add
		/// the strings as options to <paramref name="dropdown"/>
		/// </summary>
		/// <param name="dropdown">Dropdown instance to fill</param>
		/// <param name="enumType">Enum type to be parsed</param>
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
