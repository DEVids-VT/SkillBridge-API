namespace SkillBridge.Infrastructure.Configuration;

/// <summary>
/// Represents Stripe configuration settings for payment processing.
/// </summary>
public class StripeSettings
{
    /// <summary>
    /// The Stripe API Secret Key for server-side operations.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// The Stripe Publishable Key for client-side operations.
    /// </summary>
    public string PublishableKey { get; set; } = string.Empty;
    
    /// <summary>
    /// The Stripe webhook secret for validating webhook requests.
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;
}