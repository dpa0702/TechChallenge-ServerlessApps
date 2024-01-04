using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class DurableFunctionsTCF
    {
        [FunctionName("DurableFunctionsTCF")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>(nameof(BuscarProduto), "Bicicleta Caloi Strada"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(EfetuarPagamento), "3900"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(Entregar), "OK"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        // [FunctionName(nameof(BuscarProduto))] --> nameof linha abaixo tem o mesmo efeito
        [FunctionName("BuscarProduto")]
        public static string BuscarProduto([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("BuscarProduto {name}.", name);
            return $"Buscando o Produto {name}!";
        }
        
        [FunctionName(nameof(EfetuarPagamento))]
        public static string EfetuarPagamento([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("EfetuarPagamento {name}.", name);
            return $"Efetuando Pagamento {name}!";
        }
        
        [FunctionName(nameof(Entregar))]
        public static string Entregar([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Entregar {name}.", name);
            return $"Entregar {name}!";
        }

        [FunctionName("DurableFunctionsTCF_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunctionsTCF", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}