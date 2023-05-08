using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	public sealed class MainThreadDispatcher : MonoBehaviour
	{
		private static readonly Queue<Action> s_executeOnMainThread = new Queue<Action>();

		public static MainThreadDispatcher Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			lock (s_executeOnMainThread)
			{
				while (s_executeOnMainThread.Count > 0)
				{
					s_executeOnMainThread.Dequeue().Invoke();
				}
			}
		}

		public void Enqueue(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			lock (s_executeOnMainThread)
			{
				s_executeOnMainThread.Enqueue(action);
			}
		}
	}
}