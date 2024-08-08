using System.Text.Json.Serialization;

public class EntranceData
{
    [JsonPropertyName("member")]
    public string Member { get; set; }

    [JsonPropertyName("course")]
    public string Course { get; set; }

    [JsonPropertyName("entrances_done")]
    public int EntrancesDone { get; set; }

    [JsonPropertyName("entrances_number")]
    public int EntrancesNumber { get; set; }

    [JsonPropertyName("payment_completed")]
    public bool PaymentCompleted { get; set; }

    [JsonPropertyName("fee_paid")]
    public decimal FeePaid { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}

public class AddEntranceApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public EntranceData Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
