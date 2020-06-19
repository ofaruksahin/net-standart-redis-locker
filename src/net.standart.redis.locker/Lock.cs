using StackExchange.Redis;
using System;

namespace net.standart.redis.locker
{
    /// <summary>
    /// Locklama işlemi başarılı ise oluşturulacak lock object modelidir.
    /// </summary>
    public class Lock
    {
        public Lock(RedisKey key, RedisValue val, TimeSpan validity,int db)
        {
            _key = key;
            _val = val;
            _validity = validity;
            _db = db;
        }

        RedisKey _key;
        RedisValue _val;
        TimeSpan _validity;
        int _db;

        public RedisKey Key
        {
            get
            {
                return _key;
            }
        }

        public RedisValue Value
        {
            get
            {
                return _val;
            }
        }

        public TimeSpan Validity
        {
            get
            {
                return _validity;
            }
        }

        public int Db
        {
            get
            {
                return _db;
            }
        }
    }
}
