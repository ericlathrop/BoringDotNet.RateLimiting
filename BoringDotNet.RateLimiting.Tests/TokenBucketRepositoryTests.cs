using BoringDotNet.Caching;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BoringDotNet.RateLimiting
{
    [TestFixture]
    public class TokenBucketRepositoryTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullCache_ThrowsException()
        {
            // ReSharper disable ObjectCreationAsStatement
            new TokenBucketRepository(null);
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindByNameAndPeriod_WithNullName_ThrowsException()
        {
            var cache = Substitute.For<ICache>();
            var repo = new TokenBucketRepository(cache);
            repo.FindByNameAndPeriod(null, 1, TimeSpan.FromSeconds(1));
        }

        [Test]
        public void FindByNameAndPeriod_WithNewBucketName_ReturnsBucketWithInitialTokensLeft()
        {
            var cache = Substitute.For<ICache>();
            var repo = new TokenBucketRepository(cache);
            TimeSpan period = TimeSpan.FromSeconds(1);
            const int tokensPerPeriod = 45;
            var bucket = repo.FindByNameAndPeriod("123", tokensPerPeriod, period);
            Assert.AreEqual(tokensPerPeriod, bucket.TokensLeft(tokensPerPeriod, period));
        }

        [Test]
        public void FindByNameAndPeriod_WithNewBucketName_StoresBucketInCache()
        {
            var data = new Dictionary<string, object>();
            var cache = new DictionaryCache(data);
            var repo = new TokenBucketRepository(cache);
            TimeSpan period = TimeSpan.FromSeconds(1);
            const int tokensPerPeriod = 45;
            var bucket = repo.FindByNameAndPeriod("123", tokensPerPeriod, period);
            Assert.AreEqual(2, data.Count);
        }

        [Test]
        public void FindByNameAndPeriod_WithCachedBucket_ReturnsCachedBucket()
        {
            var cache = new DictionaryCache();
            var cacheKeyBuilder = new CacheKeyBuilder<TokenBucket>(cache);
            TimeSpan period = TimeSpan.FromSeconds(1);
            const string name = "123";
            var key = cacheKeyBuilder.GetKey(name, period.ToString());
            const int tokensPerPeriod = 45;
            var tokenBucket = new TokenBucket(tokensPerPeriod);
            cache.Put(key, tokenBucket);

            var repo = new TokenBucketRepository(cache);
            var bucket = repo.FindByNameAndPeriod(name, tokensPerPeriod, period);
            Assert.AreEqual(tokenBucket, bucket);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_WithNullName_ThrowsException()
        {
            var cache = Substitute.For<ICache>();
            var repo = new TokenBucketRepository(cache);
            repo.Save(null, TimeSpan.FromSeconds(1), new TokenBucket(1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_WithNullBucket_ThrowsException()
        {
            var cache = Substitute.For<ICache>();
            var repo = new TokenBucketRepository(cache);
            repo.Save("123", TimeSpan.FromSeconds(1), null);
        }

        [Test]
        public void Save_WithBucket_IsReturnedOnFind()
        {
            const string name = "123";
            const int tokensPerPeriod = 45;
            TimeSpan period = TimeSpan.FromSeconds(1);

            var cache = new DictionaryCache();
            var tokenBucket = new TokenBucket(tokensPerPeriod);

            var repo = new TokenBucketRepository(cache);
            repo.Save(name, period, tokenBucket);
            var bucket = repo.FindByNameAndPeriod(name, tokensPerPeriod, period);
            Assert.AreEqual(tokenBucket, bucket);
        }
    }
}
