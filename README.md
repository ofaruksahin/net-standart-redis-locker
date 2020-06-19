# net-standart-redis-locker
StackExchange Redis Locker

```C#
 public void Sample()
        {
            string connString = "127.0.0.1:7000,password=123";
            List<ConnectionMultiplexer> distrubutedCaches = new List<ConnectionMultiplexer>
            {
                ConnectionMultiplexer.Connect(connString)
            };
            using (ILocker locker = new Locker(distrubutedCaches))
            {
                Lock lockObject;
                enumLockStatus lockStatus =  locker.Lock("key",TimeSpan.FromSeconds(10),out lockObject);
                if (lockStatus == enumLockStatus.Succeeded)
                {
                    // Do Work
                }
                locker.Unlock(lockObj);
            }
        }
```
