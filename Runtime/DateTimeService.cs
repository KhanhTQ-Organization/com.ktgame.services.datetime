using System;
using com.ktgame.datetime;
using com.ktgame.unbiased_time;
using com.ktgame.core;
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

		public DateTime Now => _ntpDateTime.Now;
		public DateTime NowUtc => _ntpDateTime.NowUtc;

		public UniTask OnInitialize(IArchitecture architecture)
		{
			Debug.Log("[DateTimeService] Initializing. System time: " + DateTime.Now);
			
			// NtpDateTime will automatically init UnbiasedTime and start background NTP sync
			_ntpDateTime = new NtpDateTime();
			
			Initialized = true;
			return UniTask.CompletedTask;
		}

		public void OnAppPause(bool pause)
		{
			if (pause)
			{
				_ntpDateTime?.UnbiasedTime?.SessionEnd();
			}
			else
			{
				_ntpDateTime?.UnbiasedTime?.SessionStart();
			}
		}

		public void OnAppQuit()
		{
			_ntpDateTime?.UnbiasedTime?.SessionEnd();
		}
	}
}
