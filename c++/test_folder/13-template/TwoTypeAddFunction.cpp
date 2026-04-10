#include <iostream>
using namespace std;

//함수 템플릿
template <typename T>
T Add(T num1, T num2)
{
    cout << "T add(T num1, T num2)" << endl;
    return num1 + num2;
}

// 템플릿 함수
int Add(int num1, int num2)
{
    cout << "Add(int num1, int num2)" << endl;
    return num1+num2;
}

double Add(double num1, double num2)
{
    cout << "Add(double num1, double num2)" << endl;
    return num1 + num2;
}

int main(void)
{
    cout << Add(5,7) << endl; // 일반 함수
    cout << Add(3.7, 7.5) << endl; // 일반 함수
    cout << Add<int>(5,7) << endl; // template 함수
    cout << Add<double>(3.7, 7.5) <<endl; // template 함수
    return 0;
}