using System;
using System.Threading;
using com.ktgame.core;
using Cysharp.Threading.Tasks;

namespace com.ktgame.services.datetime
{
	public interface IDateTimeService : IInitializable, IPausable, IQuitable
	{
		DateTime Now { get; }
		DateTime NowUtc { get; }
		bool IsSynchronized => false;
		bool IsUsingSystemTime => true;
		event Action Synchronized
		{
			add { }
			remove { }
		}
		
		UniTask<bool> WaitForSynchronizationAsync(float timeoutSeconds = 5f, CancellationToken cancellationToken = default)
		{
			return UniTask.FromResult(IsSynchronized);
		}
	}
}
