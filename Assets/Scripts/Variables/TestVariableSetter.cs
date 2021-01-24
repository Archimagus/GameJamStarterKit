using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Variables
{
	public class TestVariableSetter: MonoBehaviour
	{
		public GameEvent InvokeOnStartEvent;
		public IntReference IntVariable;
		public int targetValue;

		private void Start()
		{
			InvokeOnStartEvent.Raise();
			IntVariable.Value = targetValue;
		}
	}
}
