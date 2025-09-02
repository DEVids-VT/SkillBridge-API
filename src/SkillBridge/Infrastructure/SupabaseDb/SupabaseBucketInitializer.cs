using Supabase;
using Supabase.Storage;

namespace SkillBridge.Infrastructure.SupabaseDb
{
    /// <summary>
    /// Hosted service to ensure Supabase buckets exist at startup.
    /// </summary>
    public class SupabaseBucketInitializer : IHostedService
    {
        private readonly Supabase.Client _client;

        public SupabaseBucketInitializer(Supabase.Client client)
        {
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var storage = _client.Storage;
            var buckets = await storage.ListBuckets();
            if (buckets == null)
            {
                throw new InvalidOperationException("Failed to fetch buckets from Supabase.");
            }

            if (!buckets.Any(b => b.Name == "images"))
            {
                await storage.CreateBucket("images", new BucketUpsertOptions { Public = true });
            }

            if (!buckets.Any(b => b.Name == "files"))
            {
                await storage.CreateBucket("files", new BucketUpsertOptions { Public = false });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
