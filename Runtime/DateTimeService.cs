using System;
using System.Threading;
using com.ktgame.core;
using com.ktgame.unbiased_time;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.ktgame.services.datetime
{
	[Service(typeof(IDateTimeService))]
	public class DateTimeService : MonoBehaviour, IDateTimeService
	{
		public int Priority => -1;
		public bool Initialized { get; set; }

		private NtpDateTime _ntpDateTime;
		private bool _destroyed;

		public event Action Synchronized;
		
		private NtpDateTime Ntp
		{
			get
			{
				if (_ntpDateTime == null)
				{
					_ntpDateTime = new NtpDateTime();
					_ntpDateTime.Synchronized += OnNtpSynchronized;
				}

				return _ntpDateTime;
			}
		}

		public DateTime Now => Ntp.Now;
		public DateTime NowUtc => Ntp.NowUtc;
		public bool IsSynchronized => Ntp.IsSynchronized;
		public bool IsUsingSystemTime => Ntp.UnbiasedTime.IsUsingSystemTime();

		public UniTask OnInitialize(IArchitecture architecture)
		{
			Debug.Log("[DateTimeService] Initializing. System time: " + DateTime.Now);
			_ = Ntp;

			Initialized = true;
			return UniTask.CompletedTask;
		}

		public UniTask<bool> WaitForSynchronizationAsync(float timeoutSeconds = 5f, CancellationToken cancellationToken = default)
		{
#if UNITY_EDITOR || DEVELOPMENT
		
			return UniTask.FromResult(true);
#else
			if (Ntp.IsSynchronized)
			{
				return UniTask.FromResult(true);
			}
			
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return UniTask.FromResult(false);
			}

			return Ntp.WaitForSynchronizationAsync(timeoutSeconds, cancellationToken);
#endif
		}

		public void OnAppPause(bool pause)
		{
			if (_ntpDateTime == null || _destroyed)
			{
				return;
			}

			if (pause)
			{
				_ntpDateTime.UnbiasedTime?.SessionEnd();
			}
			else
			{
				_ntpDateTime.UnbiasedTime?.SessionStart();
				_ntpDateTime.Resynchronize();
			}
		}

		public void OnAppQuit()
		{
			_ntpDateTime?.UnbiasedTime?.SessionEnd();
		}

		private void OnDestroy()
		{
			_destroyed = true;
			if (_ntpDateTime != null)
			{
				_ntpDateTime.Synchronized -= OnNtpSynchronized;
				_ntpDateTime.Dispose();
			}
		}

		private void OnNtpSynchronized()
		{
			try
			{
				Synchronized?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
