using System;
using com.ktgame.core;

namespace com.ktgame.services.datetime
{
    public interface IDateTimeService : IInitializable, IPausable, IQuitable
    {
        DateTime Now { get; }

        DateTime NowUtc { get; }
    }
}