/// Employee 클래서 도입(Base 클래스)
#include <iostream>
#include <cstring>
using namespace std;

class Employee
{
private:
    char name[100];
public:
    Employee(const char * name)
    {
        strcpy(this->name, name);
    }
    void ShowYourName() const
    {
        cout << "Name: " << name << endl;
    }
};
class PermanentWorker : public Employee
{
private:
    int salary;
public:
    PermanentWorker(const char * name, int money): Employee(name), salary(money)
    {}
    int GetPay() const
    {
        return salary;
    }
    void ShowSalaryInfo() const
    {
        ShowYourName();
        cout << "salary: " << GetPay() << endl;
    }
};

class TemporaryWorker : public Employee
{
private:
    int workTime;
    int payPerHour;
public:
    TemporaryWorker(const char * name, int pay) : Employee(name), workTime(0), payPerHour(pay)
    {}
    void AddWorkTime(int time)
    {
        workTime += time;
    }
    int GetPay() const
    {
        return workTime * payPerHour;
    }
    void ShowSalaryInfo() const
    {
        ShowYourName();
        cout << "salary: " << GetPay() << endl << endl;
    }
};

class SalesWorker : public PermanentWorker
{
private:
    int salesResult;
    double bonusRatio;
public:
    SalesWorker(const char * name, int money, double ratio) : PermanentWorker(name, money), salesResult(0), bonusRatio(ratio)
    {}
    void AddSalesResuilt(int value)
    {
        salesResult += value;
    }
    int GetPay() const
    {
        return PermanentWorker::GetPay() + (int)(salesResult*bonusRatio);
    }
    void ShowSalaryInfo() const
    {
        ShowYourName();
        cout << "salary: " << GetPay() << endl << endl;
    }
};
class EmployeeHandler
{
private:
    Employee* empList[50];
    int empNum;
public:
    EmployeeHandler() : empNum(0)
    { }
    void AddEmployee(Employee* emp)
    {
        empList[empNum++] = emp;
    }
    void ShowAllSalaryInfo() const
    {
        /*
        for (int i = 0; i < empNum; i++)
        {
            empList[i] -> ShowSalaryInfo();
        }*/
    }
    void ShowTotalSalary() const
    {
        int sum = 0;
        /*
        for (int i = 0; i < empNum; i++)
        {
            sum += empList[i] -> GetPay();
        }
        */
        cout << "salary sum: " << sum << endl;

    }
    ~EmployeeHandler()
    {
        for (int i = 0; i < empNum; i++)
        {
            delete empList[i];
        }
    }
};

int main(void)
{
    //직원 관리를 위한 handler 클래스 객체 생성
    EmployeeHandler handler;

    //정규직 추가
    handler.AddEmployee(new PermanentWorker("Kim", 1000));
    handler.AddEmployee(new PermanentWorker("LEE", 1500));

    // 임시직 추가
    TemporaryWorker * alba = new TemporaryWorker("Jung", 700);
    alba -> AddWorkTime(5);
    handler.AddEmployee(alba);

    //영업직 추가
    SalesWorker * seller = new SalesWorker("Hong", 1000, 0.1);
    seller -> AddSalesResuilt(7000);
    handler.AddEmployee(seller);


    // 이번 달 지불 급여 정보
    handler.ShowAllSalaryInfo();

    // 지불 급여 총 합 
    handler.ShowTotalSalary();
    return 0;
}
