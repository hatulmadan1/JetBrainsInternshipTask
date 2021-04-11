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

        private Mutex _addDepartmentMutex = new Mutex();

        public Organization(int departmentsNumber)
        {
            DepartmentsNumber = departmentsNumber;
            Departments = new Department[departmentsNumber];
            _actualDepartmentsNumber = 0;
        }

        public void AddDepartment(int departmentId,  Department department)
        {
            _addDepartmentMutex.WaitOne();

            Departments[departmentId] = department;
            _actualDepartmentsNumber++;
            
            _addDepartmentMutex.ReleaseMutex();
        }

        public bool IsConfigCorrect()
        {
            return DepartmentsNumber == _actualDepartmentsNumber;
        }

        public void ProcessStampList(StampList stampList, int start, int end)
        {
            try
            {
                /*
                 * in order to avoid hash collision in case of more than 2^32 masks
                 * I keep masks grouped by their hash.
                 * (Dictionary use GetHashCode which returns int that means that we
                 * could only keep 2^32 different masks in Dictionary
                 * Mask is a bit sequence, all masks have same length (number of stamps)
                 * Number of possible masks with length x equals to 2^x
                 * So, default Dictionary could get a collision in case of 32 or more stumps
                 * and lots of departments (lots means at least integer, because if every
                 * department has a conditional rule, we get 2 * numberOfDepartments masks)
                 */
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
                    nextDepartmentId = Departments[departmentId].ProcessVisit(stampList);
                    
                    string currentMask = stampList.ToString();
                    int currentMaskHash = currentMask.GetHashCode();
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
