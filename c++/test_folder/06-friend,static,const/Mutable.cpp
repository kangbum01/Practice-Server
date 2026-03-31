#include <iostream>
using namespace std;

class SoSimple
{
private:
    int num1;
    mutable int num2; // const 함수에 대한 예외
public:
    SoSimple(int n1, int n2):num1(n1), num2(n2)
    {}
    void ShowSimpleData() const
    {
        cout << num1 << ", " << num2 << endl;
    }
    void CopyToNum2() const
    {
        num2 = num1; // num2를 mutable로 설정하였기 때문에 const 변수로 설정하였지만 값 변경 가능
    }
};

int main(void)
{
     SoSimple sm(1,2);
     sm.ShowSimpleData();
     sm.CopyToNum2();
     sm.ShowSimpleData();
    return 0;
}