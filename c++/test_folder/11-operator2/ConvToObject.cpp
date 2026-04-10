//형 변환 연산자
#include <iostream>
using namespace std;

class Number
{
private:
    int num;
public:
    Number(int n=0) : num(n)
    {
        cout << "Number(int n = 0)" << endl;
    }
    Number& operator=(const Number & ref)
    {
        cout << "operator=()"<<endl;
        num=ref.num;
        return *this;
    }
    void ShowNumber() { cout<<num<<endl; }
};

int main(void)
{
    Number num; // 임시 객체의 생성
    num=30;     // 임시객체를 대상으로 하는 대입 연산자의 호출
    num.ShowNumber();
    return 0;
}
// A형 객체가 와야 할 위치에 B형 데이터(또는 객체)가 왔을 경우, B형 데이터를 인자로 전달받는 A형 클래스의 생성자 호출을 통해서
// A형 임시객체를 생성한다.