using StackExchange.Redis;
using System;
using System.Collections.Generic;
using Xunit;


namespace net.standart.redis.locker.test
{
    public class LockerTest
    {
        [Fact]
        public void Lock_Test()
        {
            string connString = "";
            using (ILocker locker = new Locker(new List<ConnectionMultiplexer> { ConnectionMultiplexer.Connect(connString)}))
            {
                Lock lockObject;
                enumLockStatus lockStatus =  locker.Lock("key", TimeSpan.FromSeconds(30), out lockObject);
                if (lockStatus == enumLockStatus.Succeeded)
                    Assert.True(true);
                else
                    Assert.True(false);
            }
        }
    }
}
