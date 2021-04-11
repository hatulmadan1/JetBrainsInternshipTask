using System;
using System.Collections.Concurrent;
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
        private StampList _stamps;

        private Task precalc;
        private bool _isPrecalcReady;
        private Mutex precalcMutex = new Mutex();

        public void StartSettingConfiguration(int departmentsNumber, int stampsNumber)
        {
            // ??= is here to prevent losing data in case of more than one attempt to set
            // number of departments and number of stamps
            _bureau ??= new Organization(departmentsNumber);
            _stamps ??= new StampList(stampsNumber);
        }
        
        public void AddDepartmentWithConditionalRule(
            int departmentId,
            int conditionalStamp, 
            int inStampTrue, int outStampTrue, int nextDepartmentTrue, 
            int inStampFalse, int outStampFalse, int nextDepartmentFalse)
        {
            if (!InRange(_stamps.StampListArray.Count, conditionalStamp) ||
                !InRange(_stamps.StampListArray.Count, inStampTrue) ||
                !InRange(_stamps.StampListArray.Count, outStampTrue) ||
                !InRange(_stamps.StampListArray.Count, inStampFalse) ||
                !InRange(_stamps.StampListArray.Count, outStampFalse))
            {
                throw new IndexOutOfRangeException("Incorrect stamp number: now such stamp in this organization.");
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
                        --conditionalStamp, 
                        new UnconditionalRule(
                            --inStampTrue, --outStampTrue, --nextDepartmentTrue), 
                        new UnconditionalRule(--inStampFalse, --outStampFalse, --nextDepartmentFalse)
                        ) 
                    )
                );
        }

        public void AddDepartmentWithUnconditionalRule(int departmentId, int inStamp, int outStamp, int nextDepartment)
        {
            if (!InRange(_stamps.StampListArray.Count, inStamp) ||
                !InRange(_stamps.StampListArray.Count, outStamp))
            {
                throw new IndexOutOfRangeException("Incorrect stamp number: now such stamp in this organization.");
            }

            if (!InRange(_bureau.DepartmentsNumber, nextDepartment))
            {
                throw new IndexOutOfRangeException("Incorrect next department: now such department in this organization.");
            }

            _bureau.AddDepartment(--departmentId, new Department(new UnconditionalRule(--inStamp, --outStamp, --nextDepartment)));
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

            precalc = new Task(() => _bureau.ProcessStampList(_stamps, --start, --end));
        }
        public ApiRespond ProcessRequest(int departmentId)
        {
            precalcMutex.WaitOne();
            //in case of no one called precalc, call precalc and wait for if
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

            public List<List<int>> UncrossedStamps { get; }

            ReaderWriterLock saveResults = new ReaderWriterLock();

            public ApiRespond(int departmentId, bool eternalCycle, string stampsData)
            {
                DepartmentId = departmentId + 1;
                EternalCycle = eternalCycle;
                UncrossedStamps = new List<List<int>>();
                if (stampsData == "")
                {
                    IsVisited = false;
                }
                else
                {
                    IsVisited = true;
                    var data = JsonSerializer.Deserialize<KeyValuePair<int, Dictionary<int, List<string>>>>(stampsData).Value.Values;

                    Parallel.ForEach(data, elem =>
                        Parallel.ForEach(elem, stampMask =>
                            {
                                //get positions of 1 in the mask
                                List<int> existingStamps = Enumerable.Range(0, stampMask.Length)
                                    .Where(i => stampMask[i] == '1')
                                    .Select(x => x + 1)
                                    .ToList();
                                
                                saveResults.AcquireWriterLock(10);

                                UncrossedStamps.Add(existingStamps);

                                saveResults.ReleaseWriterLock();
                            }

                        )
                    );
                }
            }
        }
    }
}
