using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BureaucracySimulator
{
    public class Organization
    {
        public int DepartmentsNumber { get; private set; }
        public int StumpsNumber { get; private set; }
        public List<Department> Departments { get; private set; }

        public Organization(int departmentsNumber, int stumpsNumber)
        {
            DepartmentsNumber = departmentsNumber;
            StumpsNumber = stumpsNumber;
            Departments = new List<Department>();
        }

        public void AddDepartment(Department department)
        {
            Departments.Add(department);
        }

        public void ProcessStumpList(StumpList stumpList, int start, int end)
        {
            List<Dictionary<int, List<BitArray>>> masks = new List<Dictionary<int, List<BitArray>>>(DepartmentsNumber);
            Dfs(start);

            void Dfs(int departmentId)
            {
                masks[departmentId] = new Dictionary<int, List<BitArray>>();
                int nextDepartment = Departments[departmentId].ProcessVisit(stumpList);
                int currentMaskHash = stumpList.GetHashCode();
                if (masks[departmentId][currentMaskHash] == null)
                {
                    masks[departmentId][currentMaskHash] = new List<BitArray>();
                }
                if (!masks[departmentId].ContainsKey(currentMaskHash) || !masks[departmentId][currentMaskHash].Contains(stumpList.StumpListArray)) //вот здесь надо проверять ещё и на уникальность не только по хэшу
                {
                    masks[departmentId][currentMaskHash].Add(stumpList.StumpListArray);
                    Dfs(nextDepartment);
                }
            }

            //ответом будет masks
        }
    }
}
