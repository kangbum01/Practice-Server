#include<iostream>
using namespace std;

class AAA
{
private:
    int num1;
public:
    virtual void Func1() { cout << "Func1"<< endl;}
    virtual void Func2() {cout << "Func2" << endl;}
};

class BBB : public AAA
{
private:
    int num2;
public:
    virtual void Func1() { cout << "BBB::Func1" << endl;}
    void Func3() { cout << "Func3" << endl;}
};

int main(void)
{
    AAA * aptr = new AAA();
    aptr -> Func1();

    BBB * bptr = new BBB();
    bptr -> Func1();

    return 0;
}

// 이때 컴파일러는 Virtual 테이블을 생성한다
// AAA에는 AAA::fun1, AAA::fun2
// BBB에는 BBB::func1, AAA::func2, BBB::func3