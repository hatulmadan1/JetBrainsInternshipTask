using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace BureaucracySimulator
{
    internal class Organization
    {
        public int DepartmentsNumber { get; }
        public Department[] Departments { get; }

        private int _actualDepartmentsNumber;

        private readonly Mutex _addDepartment;

        public Organization(int departmentsNumber)
        {
            DepartmentsNumber = departmentsNumber;
            Departments = new Department[departmentsNumber];
            _actualDepartmentsNumber = 0;
            _addDepartment = new Mutex();
        }

        public void AddDepartment(int departmentId,  Department department)
        {
            _addDepartment.WaitOne();
            lock (_addDepartment)
            {
                Departments[departmentId] = department;
                _actualDepartmentsNumber++;
            }
            _addDepartment.ReleaseMutex();
        }

        public bool IsConfigCorrect()
        {
            return DepartmentsNumber == _actualDepartmentsNumber;
        }

        public void ProcessStumpList(StumpList stumpList, int start, int end)
        {
            try
            {
                Dictionary<int, Dictionary<int, List<string>>> masks = new Dictionary<int, Dictionary<int, List<string>>>();
                int departmentId = start, nextDepartmentId = start;
                bool eternalCycle = false;

                do
                {
                    departmentId = nextDepartmentId;
                    if (!masks.ContainsKey(departmentId))
                    {
                        masks.Add(departmentId, new Dictionary<int, List<string>>());
                    }
                    nextDepartmentId = Departments[departmentId].ProcessVisit(stumpList);
                    int currentMaskHash = stumpList.GetHashCode();
                    string currentMask = stumpList.ToString();

                    if (!masks[departmentId].ContainsKey(currentMaskHash))
                    {
                        masks[departmentId].Add(currentMaskHash, new List<string>());
                    }

                    if (!masks[departmentId].ContainsKey(currentMaskHash) ||
                        !masks[departmentId][currentMaskHash].Contains(currentMask))
                    {
                        masks[departmentId][currentMaskHash].Add(currentMask);
                    } 
                    else
                    {
                        eternalCycle = true;
                        break;
                    }
                } while (departmentId != end);

                SavePrecalcResults(eternalCycle, masks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SavePrecalcResults(bool eternalCycle, Dictionary<int, Dictionary<int, List<string>>> masks)
        {
            using (StreamWriter sw = new StreamWriter("data.json"))
            {
                if (eternalCycle) sw.WriteLine("EternalCycle");
                foreach (var states in masks)
                {
                    sw.Write(JsonSerializer.Serialize(states));
                    sw.WriteLine();
                }
            }
        }
    }
}
