using System;
using System.Text.Json;
using BureaucracySimulator;

namespace ConsolePrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(3, 2);
            api.AddDepartmentWithUnconditionalRule(0, 0, 1, 1);
            api.AddDepartmentWithUnconditionalRule(1,1, 0, 0);
            api.AddDepartmentWithUnconditionalRule(2,1, 1, 0);
            api.FinishSettingConfiguration(0, 1);

            var result = api.ProcessRequest(0);
            Console.WriteLine(JsonSerializer.Serialize(result));
            result = api.ProcessRequest(1);
            Console.WriteLine(JsonSerializer.Serialize(result));
            result = api.ProcessRequest(2);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
