#include <iostream>
using namespace std;

class BaseOne
{
public:
    void SimpleFunc() { cout << "BaseOne" << endl; }
};

class BaseTwo
{
public:
    void SimpleFunc() { cout << "BaseTwo" << endl;}
};

class MultiDerived : public BaseOne, protected BaseTwo
{
public:
    void ComplexFunc()
    {
        // 상속하는 클래스에 동일한 이름의 멤버 함수가 있을 경우 클래스를 직접 명시해서 해결한다.
        BaseOne::SimpleFunc();
        BaseTwo::SimpleFunc();
    }
};

int main(void)
{
    MultiDerived mtd;
    mtd.ComplexFunc();
    return 0;
}