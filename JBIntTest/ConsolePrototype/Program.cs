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
            api.StartSettingConfiguration(4, 6);
            api.AddDepartmentWithUnconditionalRule(0, 0, 4, 1);
            api.AddDepartmentWithConditionalRule(1, 2, 1, 5, 3, 5, 3, 2);
            api.AddDepartmentWithUnconditionalRule(2, 2, 3, 1);
            api.AddDepartmentWithUnconditionalRule(3, 3, 0, 0);
            api.SetStartEndDepartments(0, 3);

            var result = api.ProcessRequest(0);
            Console.WriteLine(JsonSerializer.Serialize(result));
            result = api.ProcessRequest(1);
            Console.WriteLine(JsonSerializer.Serialize(result));
            result = api.ProcessRequest(2);
            Console.WriteLine(JsonSerializer.Serialize(result));
            result = api.ProcessRequest(3);
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
