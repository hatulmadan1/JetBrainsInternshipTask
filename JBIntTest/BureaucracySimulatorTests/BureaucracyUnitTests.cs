using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BureaucracySimulator;

namespace BureaucracySimulatorTests
{
    [TestClass]
    public class BureaucracyUnitTests
    {
        [TestMethod]
        public void SimpleConfigNoErrors()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(3, 3);
            api.AddDepartmentWithUnconditionalRule(0, 0, 1, 1);
            api.AddDepartmentWithUnconditionalRule(1, 1, 0, 0);
            api.AddDepartmentWithUnconditionalRule(2, 1, 1, 0);
            api.SetStartEndDepartments(0, 1);

            var result = api.ProcessRequest(0);
            Assert.IsTrue(result.IsVisited);
            Assert.IsTrue(result.UncrossedStumps.Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0].Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0][0] == 0);

            result = api.ProcessRequest(1);
            Assert.IsTrue(result.IsVisited);
            Assert.IsTrue(result.UncrossedStumps.Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0].Count == 1);

            result = api.ProcessRequest(2);
            Assert.IsFalse(result.IsVisited);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
            "The declared number of departments does not match the actual number of departments.")]
        public void EmptyConfigError()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(3, 2); 
            api.SetStartEndDepartments(0, 1);
            api.ProcessRequest(0);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException), 
        "Incorrect department: now such department in this organization.")]
        public void IncorrectRequest()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(3, 2);
            api.AddDepartmentWithUnconditionalRule(0, 0, 1, 1);
            api.AddDepartmentWithUnconditionalRule(1, 1, 0, 0);
            api.AddDepartmentWithUnconditionalRule(2, 1, 1, 0);
            api.SetStartEndDepartments(0, 1);

            api.ProcessRequest(5);

        }

        [TestMethod]
        public void EternalCycleInPath()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(3, 3);
            api.AddDepartmentWithUnconditionalRule(0, 0, 1, 1);
            api.AddDepartmentWithUnconditionalRule(1, 1, 0, 0);
            api.AddDepartmentWithUnconditionalRule(2, 1, 1, 0);
            api.SetStartEndDepartments(0, 2);

            var result = api.ProcessRequest(0);
            Assert.IsTrue(result.EternalCycle);
            Assert.IsTrue(result.UncrossedStumps.Count == 1);
            result = api.ProcessRequest(2);
            Assert.IsFalse(result.IsVisited);
        }

        [TestMethod]
        public void ConditionalRules()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(4, 6);
            api.AddDepartmentWithUnconditionalRule(0, 0, 4, 1);
            api.AddDepartmentWithConditionalRule(1, 2, 1, 5, 3, 5, 3, 2);
            api.AddDepartmentWithUnconditionalRule(2, 2, 3, 1);
            api.AddDepartmentWithUnconditionalRule(3, 3, 0, 0);
            api.SetStartEndDepartments(0, 3);

            /*
             * 0 after dep 0
             * 0,5 or 0,1,2 after dep 1
             * 0,2,5 after dep 2
             * 1,2,3 after dep 3
             */

            var result = api.ProcessRequest(0);
            result = api.ProcessRequest(1);
            Assert.IsTrue(result.UncrossedStumps.Count == 2 
                          && result.UncrossedStumps[0].Count == 2
                          && result.UncrossedStumps[1].Count == 3);
            result = api.ProcessRequest(2);
            Assert.IsFalse(result.EternalCycle);
            result = api.ProcessRequest(3);
        }

        [TestMethod]
        public void ConcurrentRequests()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(4, 6);
            api.AddDepartmentWithUnconditionalRule(0, 0, 4, 1);
            api.AddDepartmentWithConditionalRule(1, 2, 1, 5, 3, 5, 3, 2);
            api.AddDepartmentWithUnconditionalRule(2, 2, 3, 1);
            api.AddDepartmentWithUnconditionalRule(3, 3, 0, 0);
            Thread precalc = new Thread(() => api.SetStartEndDepartments(0, 3));
            precalc.Start();
            /*
             * 0 after dep 0
             * 0,5 or 0,1,2 after dep 1
             * 0,2,5 after dep 2
             * 1,2,3 after dep 3
             */
            Task<ApiProcessor.ApiRespond>[] tasks = new Task<ApiProcessor.ApiRespond>[4]
            {
                new Task<ApiProcessor.ApiRespond>(() => api.ProcessRequest(0)),
                new Task<ApiProcessor.ApiRespond>(() => api.ProcessRequest(1)),
                new Task<ApiProcessor.ApiRespond>(() => api.ProcessRequest(2)),
                new Task<ApiProcessor.ApiRespond>(() => api.ProcessRequest(3))
            };
            foreach (var task in tasks)
            {
                task.Start();
            }

            Assert.IsTrue(tasks[0].Result.IsVisited && tasks[0].Result.UncrossedStumps.Count == 1);
            Assert.IsTrue(tasks[1].Result.IsVisited && tasks[1].Result.UncrossedStumps.Count == 2);
            Assert.IsTrue(tasks[2].Result.IsVisited && tasks[2].Result.UncrossedStumps.Count == 1);
            Assert.IsTrue(tasks[3].Result.IsVisited && tasks[3].Result.UncrossedStumps.Count == 1);
        }

        [TestMethod]
        public void MultiThreadedAddingDepartments()
        {
            ApiProcessor api = new ApiProcessor();
            api.StartSettingConfiguration(3, 2);
            Thread thread1 = new Thread(() =>
                api.AddDepartmentWithUnconditionalRule(
                    0, 0, 1, 1)
                );
            thread1.Start();

            Thread thread2 = new Thread(() => 
                api.AddDepartmentWithUnconditionalRule(
                    1, 1, 0, 0)
                );
            thread2.Start();
            Thread thread3 = new Thread(() => 
                api.AddDepartmentWithUnconditionalRule(
                    2, 1, 1, 0)
                
                );
            thread3.Start();
            api.SetStartEndDepartments(0, 1);

            var result = api.ProcessRequest(0);
            Assert.IsTrue(result.IsVisited);
            Assert.IsTrue(result.UncrossedStumps.Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0].Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0][0] == 0);

            result = api.ProcessRequest(1);
            Assert.IsTrue(result.IsVisited);
            Assert.IsTrue(result.UncrossedStumps.Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0].Count == 1);
            Assert.IsTrue(result.UncrossedStumps[0][0] == 1);

            result = api.ProcessRequest(2);
            Assert.IsFalse(result.IsVisited);
        }
    }
}
