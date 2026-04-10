#include <iostream>
using namespace std;

template <typename T>
T Max(T a, T b)
{
    return a > b ? a : b ;
}

int main(void)
{
    cout << Max(11,15) << endl;
    cout << Max('T', 'Q') << endl;
    cout << Max(3.5, 7.5) << endl;
    cout << Max("Simple", "Best") << endl; // 주소값의 결과가 출력 길이를 원한다?(strlen(a) > strlen(b) / 사전편찬 순서(strcmp(a,b))
    return 0;
}