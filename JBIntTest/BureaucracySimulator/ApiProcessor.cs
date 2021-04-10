using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BureaucracySimulator
{
    public class ApiProcessor
    {
        private static Organization _bureau;
        private static StumpList _stumps;

        public void StartSettingConfiguration(int departmentsNumber, int stumpsNumber)
        {
            _bureau ??= new Organization(departmentsNumber);
            _stumps ??= new StumpList(stumpsNumber);
        }
        
        public int AddDepartmentWithConditionalRule(
            int conditionalStump, 
            int inStumpTrue, int outStumpTrue, int nextDepartmentTrue, 
            int inStumpFalse, int outStumpFalse, int nextDepartmentFalse)
        {
            if (conditionalStump >= _stumps.StumpListArray.Count ||
                inStumpTrue >= _stumps.StumpListArray.Count ||
                outStumpTrue >= _stumps.StumpListArray.Count ||
                inStumpFalse >= _stumps.StumpListArray.Count ||
                outStumpFalse >= _stumps.StumpListArray.Count)
            {
                throw new IndexOutOfRangeException("Incorrect stump number: now such stump in this organization.");
            }

            if (nextDepartmentTrue >= _bureau.DepartmentsNumber ||
                nextDepartmentFalse >= _bureau.DepartmentsNumber)
            {
                throw new IndexOutOfRangeException("Incorrect next department: now such department in this organization.");
            }
            _bureau.AddDepartment(
                new Department(
                    new ConditionalRule(
                        conditionalStump, 
                        new UnconditionalRule(
                            inStumpTrue, outStumpTrue, nextDepartmentTrue), 
                        new UnconditionalRule(inStumpFalse, outStumpFalse, nextDepartmentFalse)
                        ) 
                    )
                );
            return _bureau.Departments.Count - 1;
        }

        public int AddDepartmentWithUnconditionalRule(int inStump, int outStump, int nextDepartment)
        {
            if (inStump >= _stumps.StumpListArray.Count ||
                outStump >= _stumps.StumpListArray.Count)
            {
                throw new IndexOutOfRangeException("Incorrect stump number: now such stump in this organization.");
            }

            if (nextDepartment >= _bureau.DepartmentsNumber)
            {
                throw new IndexOutOfRangeException("Incorrect next department: now such department in this organization.");
            }

            _bureau.AddDepartment(new Department(new UnconditionalRule(inStump, outStump, nextDepartment)));
            return _bureau.Departments.Count - 1;
        }

        public void FinishSettingConfiguration(int start, int end)
        {
            if (_bureau.DepartmentsNumber != _bureau.Departments.Count)
            {
                throw new ArgumentNullException("The declared number of departments does not match the actual number of departments.");
                //
            }
            if (start >= _bureau.DepartmentsNumber)
            {
                throw new IndexOutOfRangeException("Incorrect start department: now such department in this organization.");
            }

            if (end >= _bureau.DepartmentsNumber)
            {
                throw new IndexOutOfRangeException("Incorrect end department: now such department in this organization.");
            }
            _bureau.ProcessStumpList(_stumps, start, end);
        }
        public ApiRespond ProcessRequest(int departmentId)
        {
            if (departmentId >= _bureau.DepartmentsNumber)
            {
                throw new IndexOutOfRangeException("Incorrect department: now such department in this organization.");
            }

            string resultString = "";
            bool eternalCycle = false;

            using (StreamReader sr = new StreamReader("data.json"))
            {
                while (!sr.EndOfStream)
                {
                    var currentString = sr.ReadLine();
                    if (currentString != null && currentString.StartsWith("{\"Key\":" + departmentId + ","))
                    {
                        resultString = currentString;
                        break;
                    }

                    if (currentString != null && !eternalCycle && currentString.StartsWith("EternalCycle"))
                    {
                        eternalCycle = true;
                    }
                }
                
            }

            ApiRespond result = new ApiRespond(departmentId, eternalCycle, resultString);
            return result;
        }

        public class ApiRespond
        {
            public int DepartmentId { get; }
            public bool EternalCycle { get; }
            public bool IsVisited { get; }

            public List<List<int>> UncrossedStumps { get; }

            public ApiRespond(int departmentId, bool eternalCycle, string stumpsData)
            {
                DepartmentId = departmentId;
                EternalCycle = eternalCycle;
                if (stumpsData == "")
                {
                    IsVisited = false;
                }
                else
                {
                    IsVisited = true;
                    var data = JsonSerializer.Deserialize<KeyValuePair<int, Dictionary<int, List<string>>>>(stumpsData).Value.Values;
                    UncrossedStumps = new List<List<int>>();
                    foreach (var elem in data)
                    {
                        foreach (var stumpMask in elem)
                        {
                            List<int> existingStumps = Enumerable.Range(0, stumpMask.Length)
                                .Where(i => stumpMask[i] == '1')
                                .ToList();
                            UncrossedStumps.Add(existingStumps);
                        }
                    }
                }
            }
        }
    }
}
