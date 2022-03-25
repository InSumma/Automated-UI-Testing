using InSummaFrontEndAutomatedTesting.BusinessEntities;
using InSummaFrontEndAutomatedTesting.Parser.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Action = InSummaFrontEndAutomatedTesting.BusinessEntities.Action;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Orchestrator")]
namespace InSummaFrontEndAutomatedTesting.Parser
{
    internal class JsonParser : IJsonParser
    {
        private readonly ILogger<JsonParser> _logger;

        public JsonParser(ILogger<JsonParser> logger)
        {
            _logger = logger;
        }

        public TestingTemplate? ParseJsonToTestingTemplate(Uri fileLocation)
        {
            try
            {
                if (fileLocation is null)
                {
                    throw new ArgumentNullException(nameof(fileLocation));
                }

                var deserializedObject = JsonConvert.DeserializeObject<TestingTemplate>(File.ReadAllText(fileLocation.LocalPath));

                if(deserializedObject is null)
                {
                    throw new ArgumentNullException(nameof(deserializedObject));
                }

                var containsNullValues = deserializedObject.GetType().GetProperties().Where(x => !x.Name.Equals(nameof(TestingTemplate.DriverPath), StringComparison.OrdinalIgnoreCase))
                                         .Any(p => p.GetValue(deserializedObject) == null);

                if (containsNullValues)
                {
                    throw new ApplicationException("Deserialized JSON object contains null values. Please check input file.");
                }

                ReplaceBluePrintWithActions(deserializedObject);

                return deserializedObject;
            }
            catch(Exception e)
            {
                _logger.LogError("Error occurred in JsonParser.ParseJsonToTestingTemplate: {message}", e.Message);
                return null;
            }
        }

        private TestingTemplate ReplaceBluePrintWithActions(TestingTemplate template)
        {
            DirectoryInfo d = new(Directory.GetCurrentDirectory() + "/TestCases/BluePrints");

            FileInfo[] Files = d.GetFiles("*.json");

            foreach (FileInfo file in Files)
            {
                try
                {
                    JObject bluePrint = JObject.Parse(File.ReadAllText(file.FullName));
                    bluePrint.TryGetValue("bluePrintName", out JToken? jsonBluePrintName);
                    bluePrint.TryGetValue("actions", out JToken? jsonActions);

                    if (jsonBluePrintName == null || jsonActions == null)
                    {
                        continue;
                    }

                    var bluePrintName = jsonBluePrintName.ToString();
                    var bluePrintActions = JsonConvert.DeserializeObject<IEnumerable<Action>>(jsonActions.ToString());

                    if(bluePrintActions == null)
                    {
                        continue;
                    }

                    var actionList = template.Actions.ToList();
                    var indexToReplace = actionList.FindIndex(a => a.BluePrintName != null && a.BluePrintName.Equals(bluePrintName));

                    actionList.RemoveAt(indexToReplace);
                    actionList.InsertRange(indexToReplace, bluePrintActions);

                    template.Actions = actionList;
                }
                catch (Exception e)
                {
                    _logger.LogError("Error in JsonParser.ReplaceBluePrintWithActions: {message}", e.Message);
                }
            }

            return template;
        }
    }
}