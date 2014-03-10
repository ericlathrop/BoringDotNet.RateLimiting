using BoringDotNet.Caching;
using System;

namespace BoringDotNet.RateLimiting
{
    public class TokenBucketRepository : ITokenBucketRepository
    {
        private readonly ICache _cache;
        private readonly CacheKeyBuilder<TokenBucket> _keyBuilder;

        public TokenBucketRepository(ICache cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            _cache = cache;
            _keyBuilder = new CacheKeyBuilder<TokenBucket>(cache);
        }

        public TokenBucket FindByNameAndPeriod(string name, int tokensPerPeriod, TimeSpan period)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var key = _keyBuilder.GetKey(name, period.ToString());
            return _cache.GetOrCreate(key, () => new TokenBucket(tokensPerPeriod));
        }

        public void Save(string name, TimeSpan period, TokenBucket bucket)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (bucket == null)
                throw new ArgumentNullException("bucket");

            var key = _keyBuilder.GetKey(name, period.ToString());
            _cache.Put(key, bucket);
        }
    }
}
