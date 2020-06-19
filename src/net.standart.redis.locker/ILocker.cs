using System;

namespace net.standart.redis.locker
{
    public interface ILocker : IDisposable
    {
        /// <summary>
        /// Redis key locklar.
        /// </summary>
        /// <param name="key">Locklanacak redis key</param>
        /// <param name="ttl">Lock süresi</param>
        /// <param name="lockObject">Lock object</param>
        /// <param name="db">Locklacak verinin hangi db index</param>
        /// <returns></returns>
        enumLockStatus Lock(string key, TimeSpan ttl, out Lock lockObject, int db = 0);
        /// <summary>
        /// Redis key unlock eder.
        /// </summary>
        /// <param name="lockObject">Lock object</param>
        void Unlock(Lock lockObject);
    }
}
