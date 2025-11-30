// Doc: https://cloud.google.com/blog/topics/developers-practitioners/introducing-google-gen-ai-net-sdk

using Google.GenAI;
using Google.GenAI.Types;
using Environment = System.Environment;
using Type = Google.GenAI.Types.Type;

var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? throw new InvalidOperationException("GOOGLE_API_KEY environment variable is not set.");
var client = new Client(
    apiKey: apiKey,
    httpOptions: new HttpOptions
    {
        BaseUrl = "https://ai-proxy.lab.epam.com"//"https://ai-proxy.lab.epam.com/google",


        // Attempt 2: the below configuration fails with 'Bad Authorization header'
        //Headers = new Dictionary<string, string>
        //{
        //    { "Authorization", $"Bearer {apiKey}" }
        //}
    });

Schema countryInfo = new()
{
    Properties = new Dictionary<string, Schema>
    {
        {
            "name", new Schema { Type = Type.STRING, Title = "Name" }
        },
        {
            "population", new Schema { Type = Type.INTEGER, Title = "Population" }
        },
        {
            "capital", new Schema { Type = Type.STRING, Title = "Capital" }
        },
        {
            "language", new Schema { Type = Type.STRING, Title = "Language" }
        }
    },
    PropertyOrdering = ["name", "population", "capital", "language"],
    Required = ["name", "population", "capital", "language"],
    Title = "CountryInfo",
    Type = Type.OBJECT
};

// Define a generation config
GenerateContentConfig config = new()
{
    ResponseSchema = countryInfo,
    ResponseMimeType = "application/json",
    SystemInstruction = new Content
    {
        Parts =
        [
            new Part {Text = "Only answer questions on countries. For everything else, say I don't know."}
        ]
    },
    MaxOutputTokens = 1024,
    Temperature = 0.1,
    TopP = 0.8,
    TopK = 40,
};

var textResponse = await client.Models.GenerateContentAsync(
     model: "gemini-3-pro",
     contents: "Give me information about Cyprus",
     config: config);

GenerateImagesConfig imageConfig = new()
{
    NumberOfImages = 1,
    AspectRatio = "1:1",
    SafetyFilterLevel = SafetyFilterLevel.BLOCK_LOW_AND_ABOVE,
    PersonGeneration = PersonGeneration.DONT_ALLOW,
    IncludeSafetyAttributes = true,
    IncludeRaiReason = true,
    OutputMimeType = "image/jpeg",
};

var imageResponse = await client.Models.GenerateImagesAsync(
    model: "imagen-3.0-generate-002",
    prompt: "Red skateboard",
    config: imageConfig
);