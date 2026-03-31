///.\EmployeeManager.cpp:71:45: warning: ISO C++ forbids converting a string constant to 'char*' [-Wwrite-strings]
//  이런 오류를 char * 를 const char *로 변경해줘야 한다. 그 이유는 문자열 리터럴은 값이 변경되면 안되기 때문에 const char *로 작성해야 한다.
///
///
#include <iostream>
#include <cstring>
using namespace std;

class PermanentWorker
{
private:
    char name[100];
    int salary;
public:
    PermanentWorker(const char * name, int money):salary(money)
    {
        strcpy(this->name, name);
    }
    int GetPay() const
    {
        return salary;
    }
    void ShowSalaryInfo() const
    {
        cout << "name: " << name << endl;
        cout << "salary: " << GetPay() << endl;
    }
};

class EmployeeHandler
{
private:
    PermanentWorker* empList[50];
    int empNum;
public:
    EmployeeHandler() : empNum(0)
    { }
    void AddEmployee(PermanentWorker* emp)
    {
        empList[empNum++] = emp;
    }
    void ShowAllSalaryInfo() const
    {
        for (int i = 0; i < empNum; i++)
        {
            empList[i] -> ShowSalaryInfo();
        }
    }
    void ShowTotalSalary() const
    {
        int sum = 0;
        for (int i = 0; i < empNum; i++)
        {
            sum += empList[i] -> GetPay();
        }
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

    //직원 추가
    handler.AddEmployee(new PermanentWorker("Kim", 1000));
    handler.AddEmployee(new PermanentWorker("LEE", 1500));
    handler.AddEmployee(new PermanentWorker("JUN", 2000));

    // 이번 달 지불 급여 정보
    handler.ShowAllSalaryInfo();

    // 지불 급여 총 합 
    handler.ShowTotalSalary();
    return 0;
}
