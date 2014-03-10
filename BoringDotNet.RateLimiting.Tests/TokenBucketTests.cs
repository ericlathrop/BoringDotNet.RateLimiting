using NUnit.Framework;
using System;

namespace BoringDotNet.RateLimiting
{
    [TestFixture]
    public class TokenBucketTests
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(42)]
        public void TokensLeft_WithTokens_ReturnsInitialTokens(int initialTokens)
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(initialTokens, time);
            var left = bucket.TokensLeft(initialTokens, TimeSpan.FromSeconds(1), time);
            Assert.AreEqual(initialTokens, left);
        }

        [Test]
        public void TokensLeft_WithNoTokensButTimePassed_ReturnsOne()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(0, time);
            var left = bucket.TokensLeft(1, TimeSpan.FromSeconds(1), time.AddSeconds(1));
            Assert.AreEqual(1, left);
        }

        [Test]
        public void TokensLeft_WithNoTokensAndNotEnoughTimePassed_ReturnsZero()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(0, time);
            var future = time.AddMilliseconds(500);
            Assert.AreEqual(0, bucket.TokensLeft(1, TimeSpan.FromSeconds(1), future));
        }

        [Test]
        public void TokensLeft_WithNoTokensAndEnoughTimePassedAfterTwoCalls_ReturnsOne()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(0, time);
            var future = time.AddMilliseconds(500);
            bucket.TokensLeft(1, TimeSpan.FromSeconds(1), future);
            future = future.AddMilliseconds(500);
            Assert.AreEqual(1, bucket.TokensLeft(1, TimeSpan.FromSeconds(1), future));
        }

        [Test]
        public void TokensLeft_WithTwoCalls_DoesntLeavePartialToken()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(0, time);
            var future = time.AddMilliseconds(500);
            bucket.TokensLeft(1, TimeSpan.FromSeconds(1), future);
            future = future.AddMilliseconds(400);
            Assert.AreEqual(0, bucket.TokensLeft(1, TimeSpan.FromSeconds(1), future));
        }

        [Test]
        public void TryTakeToken_WithNoTokens_ReturnsFalse()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(0, time);
            Assert.IsFalse(bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time));
        }

        [Test]
        public void TryTakeToken_WithTokens_ReturnsTrue()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(1, time);
            Assert.IsTrue(bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time));
        }

        [Test]
        public void TryTakeToken_WithOneTokenButTwoCalls_ReturnsFalse()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(1, time);
            bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time);
            Assert.IsFalse(bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time));
        }

        [Test]
        public void TryTakeToken_WithMoreThanMax_ReturnsFalse()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(1, time);
            time = time.AddSeconds(1);
            bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time);
            Assert.IsFalse(bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time));
        }

        [Test]
        public void TryTakeToken_WithLargerMaximum_IncreasesMax()
        {
            var time = DateTime.Now;
            var bucket = new TokenBucket(1, time);
            bucket.TryTakeToken(1, TimeSpan.FromSeconds(1), time);
            time = time.AddSeconds(1);
            bucket.TryTakeToken(2, TimeSpan.FromSeconds(1), time);
            Assert.IsTrue(bucket.TryTakeToken(2, TimeSpan.FromSeconds(1), time));
        }
    }
}
