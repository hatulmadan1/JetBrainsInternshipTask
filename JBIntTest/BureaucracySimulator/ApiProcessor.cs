using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BureaucracySimulator
{
    public class ApiProcessor
    {
        private Organization _bureau;
        private StumpList _stumps;

        private Task precalc;
        private bool _isPrecalcReady;
        private Mutex precalcMutex = new Mutex();

        public void StartSettingConfiguration(int departmentsNumber, int stumpsNumber)
        {
            _bureau ??= new Organization(departmentsNumber);
            _stumps ??= new StumpList(stumpsNumber);
        }
        
        public void AddDepartmentWithConditionalRule(
            int departmentId,
            int conditionalStump, 
            int inStumpTrue, int outStumpTrue, int nextDepartmentTrue, 
            int inStumpFalse, int outStumpFalse, int nextDepartmentFalse)
        {
            if (!InRange(_stumps.StumpListArray.Count, conditionalStump) ||
                !InRange(_stumps.StumpListArray.Count, inStumpTrue) ||
                !InRange(_stumps.StumpListArray.Count, outStumpTrue) ||
                !InRange(_stumps.StumpListArray.Count, inStumpFalse) ||
                !InRange(_stumps.StumpListArray.Count, outStumpFalse))
            {
                throw new IndexOutOfRangeException("Incorrect stump number: now such stump in this organization.");
            }

            if (!InRange(_bureau.DepartmentsNumber, nextDepartmentTrue) ||
                !InRange(_bureau.DepartmentsNumber, nextDepartmentFalse))
            {
                throw new IndexOutOfRangeException("Incorrect next department: now such department in this organization.");
            }
            _bureau.AddDepartment(
                --departmentId,
                new Department(
                    new ConditionalRule(
                        --conditionalStump, 
                        new UnconditionalRule(
                            --inStumpTrue, --outStumpTrue, --nextDepartmentTrue), 
                        new UnconditionalRule(--inStumpFalse, --outStumpFalse, --nextDepartmentFalse)
                        ) 
                    )
                );
        }

        public void AddDepartmentWithUnconditionalRule(int departmentId, int inStump, int outStump, int nextDepartment)
        {
            if (!InRange(_stumps.StumpListArray.Count, inStump) ||
                !InRange(_stumps.StumpListArray.Count, outStump))
            {
                throw new IndexOutOfRangeException("Incorrect stump number: now such stump in this organization.");
            }

            if (!InRange(_bureau.DepartmentsNumber, nextDepartment))
            {
                throw new IndexOutOfRangeException("Incorrect next department: now such department in this organization.");
            }

            _bureau.AddDepartment(--departmentId, new Department(new UnconditionalRule(--inStump, --outStump, --nextDepartment)));
        }

        public void SetStartEndDepartments(int start, int end)
        {
            if (!InRange(_bureau.DepartmentsNumber, start))
            {
                throw new IndexOutOfRangeException("Incorrect start department: now such department in this organization.");
            }

            if (!InRange(_bureau.DepartmentsNumber, end))
            {
                throw new IndexOutOfRangeException("Incorrect end department: now such department in this organization.");
            }

            precalc = new Task(() => _bureau.ProcessStumpList(_stumps, --start, --end));
        }
        public ApiRespond ProcessRequest(int departmentId)
        {
            precalcMutex.WaitOne();
            
            if (!_isPrecalcReady)
            {
                if (!_bureau.IsConfigCorrect())
                {
                    throw new Exception("The declared number of departments does not match the actual number of departments.");
                }

                precalc.Start();
                precalc.Wait();
                _isPrecalcReady = true;
            }
            
            precalcMutex.ReleaseMutex();

            if (!InRange(_bureau.DepartmentsNumber, departmentId))
            {
                throw new IndexOutOfRangeException("Incorrect department: now such department in this organization.");
            }

            string resultString = "";
            bool eternalCycle = false;
            departmentId--;

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

        private bool InRange(int end, int ind)
        {
            return ind <= end && ind > 0;
        }

        public class ApiRespond
        {
            public int DepartmentId { get; }
            public bool EternalCycle { get; }
            public bool IsVisited { get; }

            public List<List<int>> UncrossedStumps { get; }

            public ApiRespond(int departmentId, bool eternalCycle, string stumpsData)
            {
                DepartmentId = departmentId + 1;
                EternalCycle = eternalCycle;
                UncrossedStumps = new List<List<int>>();
                if (stumpsData == "")
                {
                    IsVisited = false;
                }
                else
                {
                    IsVisited = true;
                    var data = JsonSerializer.Deserialize<KeyValuePair<int, Dictionary<int, List<string>>>>(stumpsData).Value.Values;
                    
                    foreach (var elem in data)
                    {
                        foreach (var stumpMask in elem)
                        {
                            List<int> existingStumps = Enumerable.Range(0, stumpMask.Length)
                                .Where(i => stumpMask[i] == '1')
                                .Select(x => x + 1)
                                .ToList();
                            UncrossedStumps.Add(existingStumps);
                        }
                    }
                }
            }
        }
    }
}
