using System;
using System.Globalization;
using com.ktgame.datetime;
using com.ktgame.core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ktgame.services.datetime
{
	[Service(typeof(IDateTimeService))]
	public class DateTimeService : MonoBehaviour, IDateTimeService
	{
		public int Priority => -1;
		public bool Initialized { get; set; }

		public DateTime Now
		{
			get
			{
#if UNITY_EDITOR || DEVELOPMENT
				return DateTime.Now;
#else
				if (_synchronized)
				{
					return _networkTime.AddSeconds(Time.realtimeSinceStartup - _receivedTime);
				}

				return _unbiasedTime.Now;
#endif
			}
		}

		public DateTime NowUtc
		{
			get
			{
#if UNITY_EDITOR || DEVELOPMENT
				return DateTime.UtcNow;
#else
				if (_synchronized)
				{
					return _networkTime.ToUniversalTime().AddSeconds(Time.realtimeSinceStartup - _receivedTime);
				}

				return _unbiasedTime.Now.ToUniversalTime();
#endif
			}
		}

		private const string NetworkServer = "https://www.microsoft.com";
		private UnbiasedTime _unbiasedTime;
		private DateTime _networkTime;
		private float _receivedTime;
		private bool _synchronized;

		public UniTask OnInitialize(IArchitecture architecture)
		{
			Debug.Log("System time: " + DateTime.Now);
			_unbiasedTime = new UnbiasedTime();
			_unbiasedTime.SessionStart();
			RequestNetworkTime().Forget();
			Initialized = true;
			return UniTask.CompletedTask;
		}

		public void OnAppPause(bool pause)
		{
			if (pause)
			{
				_unbiasedTime?.SessionEnd();
			}
			else
			{
				_unbiasedTime?.SessionStart();
			}
		}

		public void OnAppQuit()
		{
			_unbiasedTime?.SessionEnd();
		}

		private async UniTask RequestNetworkTime()
		{
			using var webRequest = UnityWebRequest.Get(NetworkServer);

			try
			{
				await webRequest.SendWebRequest();

				if (webRequest.result == UnityWebRequest.Result.Success)
				{
					var dateHeader = webRequest.GetResponseHeader("Date");

					if (DateTime.TryParseExact(
							dateHeader,
							"ddd, dd MMM yyyy HH:mm:ss 'GMT'",
							CultureInfo.InvariantCulture,
							DateTimeStyles.AssumeUniversal,
							out var parsedTime))
					{
						_synchronized = true;
						_receivedTime = Time.realtimeSinceStartup;
						_networkTime = parsedTime;
						Debug.Log($"Network Time: {parsedTime}");
					}
					else
					{
						_synchronized = false;
					}
				}
				else
				{
					_synchronized = false;
				}
			}
			catch (Exception ex)
			{
				_synchronized = false;
				Debug.LogError($"Request network time failed with exception: {ex.Message}");
			}
		}
	}
}
