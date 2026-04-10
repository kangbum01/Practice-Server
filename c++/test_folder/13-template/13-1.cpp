//SwapData 템플릿으로 만들기
#include <iostream>
using namespace std;

class Point
{
private:
    int xpos, ypos;
public:
    Point(int x=0, int y=0) : xpos(x), ypos(y) {}
    void ShowPosition() const
    {
        cout << '[' << xpos << ", "<< ypos << ']' << endl;
    }
};

template <typename T>
T SumArray(T arr[], int len)
{
    T sum = 0;
    for(int i =0; i < len; i++)
    {
        sum+=arr[i];
    }
    return sum;
}

template <typename T2>
void SwapData(T2& pt1, T2& pt2)
{
    T2 pt3 = pt1;
    pt1 = pt2;
    pt2 = pt3; 
}

int main(void)
{
    // 1번
    Point pt1(10,20);
    Point pt2(30,40);
    SwapData(pt1,pt2);
    pt1.ShowPosition();
    pt2.ShowPosition();

    //2번
    int arr1[10];
    char arr2[10];
    double arr3[10];
    for(int i = 0; i < 10; i++)
    {
        arr1[i] = i+1;
        arr2[i] = i+2;
        arr3[i] = i +3; 
    }
    cout << SumArray(arr1,10) << endl;
    cout << SumArray(arr2,10) << endl;
    cout << SumArray(arr3,10) << endl;;

}
