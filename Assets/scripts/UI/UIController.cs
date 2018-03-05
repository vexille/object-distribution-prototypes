﻿using DistributionPrototype.Config;
using DistributionPrototype.Util;
using System;
using System.Collections.Generic;
using DistributionPrototype.Messages;
using Frictionless;
using UnityEngine;

namespace DistributionPrototype.UI
{
	public class UIController : MonoBehaviour
	{
		public void OnGenerateClick()
		{
			ServiceFactory.Instance.Resolve<MessageRouter>()
				.RaiseMessage(new GenerateRequestedMessage());
		}
	}
}