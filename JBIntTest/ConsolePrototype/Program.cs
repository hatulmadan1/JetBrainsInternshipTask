using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BureaucracySimulator;

namespace ConsolePrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.Now;
            int n = 1500, m = 1500;
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(n, m);

            List<Task> addTasks = new List<Task>();
            for (int i = 1; i <= n; i++)
            {
                var i1 = i;
                addTasks.Add(new Task(() => api.AddDepartmentWithUnconditionalRule(i1, i1, (i1 + 1) % n + 1, (i1 + 1) % n + 1)));
            }
            foreach (var task in addTasks)
            {
                task.Start();
            }
            Task.WaitAll(addTasks.ToArray());

            api.SetStartEndDepartments(1, n);

            Console.WriteLine(DateTime.Now - start);

            List<Task> requestTasks = new List<Task>();
            for (int i = 1; i < n; i++)
            {
                var i1 = i;
                requestTasks.Add(new Task(() => api.ProcessRequest(i1)));
            }
            foreach (var task in requestTasks)
            {
                task.Start();
            }
            Task.WaitAll(requestTasks.ToArray());

            Console.WriteLine(DateTime.Now - start);
        }
    }
}
