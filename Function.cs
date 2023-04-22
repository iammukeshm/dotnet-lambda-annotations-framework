using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld
{
    public class Function
    {
        private readonly IDynamoDBContext _context;
        public Function(IDynamoDBContext context)
        {
            _context = context;
        }
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext: false);

            return msg.Replace("\n", "");
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, template: "/hello/{name}")]
        public async Task<Dictionary<string, string>> GetCallingIpFunction(string name)
        {

            var location = await GetCallingIP();
            return new Dictionary<string, string>
            {
                { "message", $"Hello {name}" },
                { "location", location }
            };
        }

        [LambdaFunction(MemorySize = 1024, )]
        [HttpApi(LambdaHttpMethod.Post, template: "/developers")]
        public async Task<int> CreateDeveloper([FromBody] Developer request)
        {
            await _context.SaveAsync(request);
            return request.Id;
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, template: "/developers/{id}")]
        public async Task<Developer> GetDeveloper(int id)
        {
            var developer = await _context.LoadAsync<Developer>(id);
            return developer;
        }
    }
}
