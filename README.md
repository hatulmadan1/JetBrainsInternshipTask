# JetBrainsInternshipTask

Доступ к API предоставляется через публичный класс `BureaucracySimulator.ApiProcessor`
Данный класс содержит следующие методы для задания конфигурации:
* `public void StartSettingConfiguration(int departmentsNumber, int stumpsNumber)` - метод, задающий число департаментов и печатей
* `public void AddDepartmentWithConditionalRule(
            int departmentId,
            int conditionalStump, 
            int inStumpTrue, int outStumpTrue, int nextDepartmentTrue, 
            int inStumpFalse, int outStumpFalse, int nextDepartmentFalse)` - метод, добавляющий в организацию департамент с условным правилом
* `public void AddDepartmentWithUnconditionalRule(int departmentId, int inStump, int outStump, int nextDepartment)` - метод, добавляющий в организацию департамент с безусловным правилом
* `public void SetStartEndDepartments(int start, int end)` - метод, задающий начальный и конечный департаменты

За обработку запросов отвечает метод `public ApiRespond ProcessRequest(int departmentId)`, 
возвращающий объект класса `ApiRespond`, содержащего
* `public int DepartmentId` - номер департамента, для которого дан этот ответ
* `public bool EternalCycle` - содержится ли в конфигурации на пути по департаментам вечный цикл
* `public bool IsVisited` - посещен ли данный департамент
* `public List<List<int>> UncrossedStumps` - список возможных списков незачеркнутых печатей

Ожидается, что: 
* число департаментов, добавляемых пользователем в конфигурацию, в точности соответствует заявленному
* департаменты нумеруются с 1
* печати нумеруются с 1

API поддерживает конкурентные запросы и конкурентное добавление департаментов. 

_______________________________________________________________________________________________________________________________________________

API access is provided through a public class `BureaucracySimulator.ApiProcessor`
This class contains the following methods for setting organiztion configuration:
* `public void StartSettingConfiguration(int departmentsNumber, int stumpsNumber)` - method that sets number of deparments and number of stumps
* `public void AddDepartmentWithConditionalRule(
            int departmentId,
            int conditionalStump, 
            int inStumpTrue, int outStumpTrue, int nextDepartmentTrue, 
            int inStumpFalse, int outStumpFalse, int nextDepartmentFalse)` - method that adds a department with a conditional rule into the organization
* `public void AddDepartmentWithUnconditionalRule(int departmentId, int inStump, int outStump, int nextDepartment)` - method that adds a department with an unconditional rule into the organization
* `public void SetStartEndDepartments(int start, int end)` - method that sets first and last departments of the journey

Method `public ApiRespond ProcessRequest(int departmentId)` processes requests.  
It returns an object of `ApiRespond` class, which contains the following fields and methods:
* `public int DepartmentId` - department number for which this answer is given
* `public bool EternalCycle` - does configuration contain an eternal loop on the way or not
* `public bool IsVisited` - is the department visited or not
* `public List<List<int>> UncrossedStumps` - the list of all possible sets of uncrossed stumps

Expected that:
* the number of departments added by the user to the configuration exactly corresponds to the declared one
* departments are numbered from 1
* stamps are numbered from 1

The API supports concurrent requests and concurrent adding new departments.
