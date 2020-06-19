using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace net.standart.redis.locker
{
    public class Locker : ILocker
    {
        public Locker(List<ConnectionMultiplexer> connectionMultiplexers)
        {
            _connectionMultiplexers = connectionMultiplexers;
            redis_instance_count = _connectionMultiplexers.Count;
        }

        List<ConnectionMultiplexer> _connectionMultiplexers;

        const double _clockDriveFactor = 0.01;

        const string _unlockScript = @"if redis.call(""get"",KEYS[1]) == ARGV[1] then
        return redis.call(""del"",KEYS[1])
        else
            return 0
        end";

        int redis_instance_count = 0;

        string GenereateUniqueId => Guid.NewGuid().ToString();

        public enumLockStatus Lock(string key, TimeSpan ttl, out Lock lockObject, int db = 0)
        {
            Lock innerLock = null;
            lockObject = null;
            key = $"lock:{key}";
            string val = GenereateUniqueId;

            DateTime startTime = DateTime.Now;
            int n = 0;

            while (redis_instance_count>n)
            {
                if (!Lock(key, val, ttl, db))
                {
                    Unlock(key, new RedisValue(val), db);
                    return enumLockStatus.Failure;
                }
                var drift = Convert.ToInt32((ttl.TotalMilliseconds*_clockDriveFactor)+2);
                var validity_time = ttl - (DateTime.Now - startTime) - new TimeSpan(0, 0, 0, 0, drift);
                innerLock = new Lock(key, val, validity_time, db);
                lockObject = innerLock;
                redis_instance_count++;
            }
            return enumLockStatus.Succeeded;
        }

        protected virtual bool Lock(string key,string val,TimeSpan ttl,int db)
        {
            bool succeeded = false;
            try
            {
                foreach (ConnectionMultiplexer multiplexer in _connectionMultiplexers)
                {
                    succeeded = multiplexer.GetDatabase(db).StringSet(key, val, ttl, When.NotExists);
                    if (!succeeded)
                    {
                        Unlock(key, new RedisValue(val), db);
                        succeeded = false;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                succeeded = false;
            }
            return succeeded;
        }

        public void Unlock(Lock lockObject)
        {
            Unlock(lockObject.Key,lockObject.Value,lockObject.Db);
        }

        protected virtual void Unlock(string key,byte[] val,int db)
        {
            try
            {
                RedisKey[] redisKeys = { key };
                RedisValue[] redisValues = { val };
                foreach (ConnectionMultiplexer multiplexer in _connectionMultiplexers)
                    multiplexer.GetDatabase(db).ScriptEvaluate(_unlockScript, redisKeys, redisValues);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (ConnectionMultiplexer connectionMultiplexer in _connectionMultiplexers)
                    connectionMultiplexer?.Dispose();
                _connectionMultiplexers.Clear();
                _connectionMultiplexers = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
