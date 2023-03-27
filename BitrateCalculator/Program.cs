internal class Program
{
    private static void Main(string[] args)
    {
        int pollingRate = 2;
        Transcoder response;

        //Infinite loop so I can test with multiple data sources without having to restart the app
        while (true)
        {
            response = GetJSONResponse();
            if (response != null)
                GenerateResponse(response, pollingRate);
        }
    }

    private static void GenerateResponse(Transcoder response, int pollingRate)
    {
        for (int i = 0; i < response.NIC.Length; i++)
        {
            if (i == 0)
                Console.WriteLine("Cannot calculate bitrate for the first member\n");
            else
            {
                var nic = response.NIC[i];
                var previousNic = response.NIC[i - 1];

                Console.WriteLine($"Bitrate at {nic.Timestamp} was:\n" +
                    $"Tx: {CalculateBitrate(double.Parse(nic.Tx), double.Parse(previousNic.Tx), pollingRate)} bits per second.\n" +
                    $"Rx: {CalculateBitrate(double.Parse(nic.Rx), double.Parse(previousNic.Rx), pollingRate)} bits per second.\n");
            }
        }
    }

    private static double CalculateBitrate(double currentOctets, double previousOctets, int pollingRate)
    {   
        //Octet represents a block of 8 bits (or a byte). To convert octets to bits, I multiply them by 8 (8 bits = 1 byte).
        //Polling rate of 2Hz means that the reading of data happens 2 times per second, or every 0.5s. To get Bits per second, I multiply the result by the polling rate.
        return (currentOctets - previousOctets) * 8.0 * pollingRate;
    }
    private static Transcoder GetJSONResponse()
    {
        Console.WriteLine("enter the JSON object:");
        string responseJSON = new string(Console.ReadLine());

        try
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Transcoder>(responseJSON);            
        }
        catch (Exception)
        {
            return null;
        }
    }
}


public class NIC
{
    public string Description { get; set; }
    public string MAC { get; set; }
    public DateTime Timestamp { get; set; }
    public string Rx { get; set; }
    public string Tx { get; set; }
}

public class Transcoder
{
    public string Device { get; set; }
    public string Model { get; set; }
    public NIC[] NIC { get; set; }
}