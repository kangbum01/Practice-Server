#include<iostream>
using namespace std;

class First
{
public:
    void FirstFunc()
    {
        cout << "FistFunc()" << endl;
    }
    virtual void SimpleFunc()
    {
        cout << "First's SimpleFunc()" << endl;
    }
};

class Second : public First
{
public:
    void SecondFunc()
    {
        cout << "SecondFunc()" << endl;
    }    
    virtual void SimpleFunc()
    {
        cout << "Second's SimpleFunc() " << endl;
    }
};

class Third : public Second
{
public:
    void ThirdFunc()
    {
        cout << "ThirdFunc()" << endl;
    }    
    virtual void SimpleFunc()
    {
        cout << "Third's SimpleFunc() " << endl;
    }
};

int main(void)
{
    Third obj;
    obj.FirstFunc();
    obj.SecondFunc();
    obj.ThirdFunc();
    obj.SimpleFunc();

    Second & sref = obj;
    sref.FirstFunc();
    sref.SecondFunc();
    sref.SimpleFunc(); // obj는 Third 객체이고 현재 SimpleFunc()는 가상함수로 정의되었기 때문에 Third.SimpleFunc()가 출력

    First & fref = sref;
    fref.FirstFunc();
    fref.SimpleFunc(); // obj는 Third 객체이고 현재 SimpleFunc()는 가상함수로 정의되었기 때문에 Third.SimpleFunc()가 출력

    return 0;
}