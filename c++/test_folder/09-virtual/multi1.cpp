#include <iostream>
using namespace std;

class BaseOne
{
private:
public:
    void SimpleFuncOne() { cout << "BaseOne"<< endl;}
};

class BaseTwo
{
private:
public:
    void SimpleFuncTwo() {cout << "BaseTwo" << endl;}
};

class MultiDerived : public BaseOne, protected BaseTwo
{
public:
    void ComplexFunc()
    {
        SimpleFuncOne();
        SimpleFuncTwo();
    }
};

int main(void)
{
    MultiDerived mdr;
    mdr.ComplexFunc();
    return 0;
}