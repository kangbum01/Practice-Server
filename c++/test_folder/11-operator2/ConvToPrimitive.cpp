#include <iostream>
using namespace std;

class Number
{
private:
    int num;
public:
    Number(int n = 0) : num(n)
    {
        cout<<"Number(int n = 0)" << endl;
    }
    Number& operator=(const Number& ref)
    {
        cout << "operator=()" <<endl;
        num = ref.num;
        return *this;
    }
    // 형 변환 연산자
    // int형으로 형 변환해야 하는 상황에서 호출되는 함수
    operator int ()
    {
        return num;
    }
    void ShowNumber() { cout << num << endl; }
};

int main(void)
{
    Number num1;
    num1 = 30;
    Number num2 = num1 + 20;
    num2.ShowNumber();
    return 0;
}