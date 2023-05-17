using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	public sealed class MainThreadDispatcher : MonoBehaviour
	{
		private static readonly Queue<Action> s_executeOnMainThread = new();

		public static MainThreadDispatcher Instance { get; private set; }

		/// <summary>
		/// MUST BE CALLED BY UNITY ONLY
		/// </summary>
		private void Awake()
		{
			Instance = this;
		}

		/// <summary>
		/// MUST BE CALLED BY UNITY ONLY
		/// </summary>
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

		/// <summary>
		/// Enqueue an action to be executed on the main thread.
		/// </summary>
		/// <param name="action">Action to be executed</param>
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