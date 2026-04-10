#include <iostream>
using namespace std;

class SortRule
{
public:
    // 추상 클래스로 정의된 SortRule의 순수가상함수 operator()
    virtual bool operator()(int num1, int num2) const = 0;
};

class AscendingSort : public SortRule //오름차순
{
public:
    bool operator()(int num1, int num2) const
    {
        if(num1 > num2)
            return true;
        else
            return false;
    }
};

class DescendingSort : public SortRule // 내림차순
{
public:
    bool operator()(int num1, int num2) const
    {
        if(num1 < num2)
            return true;
        else
            return false;
    }
};

class DataStorage
{
private:
    int * arr; // 동적 할당
    int idx;
    const int MAX_LEN; //상수화
public:
    DataStorage(int arrlen) : idx(0), MAX_LEN(arrlen)
    {
        arr=new int[MAX_LEN];
    }
    void AddData(int num)
    {
        if(MAX_LEN<=idx)
        {
            cout << "Full of list" << endl;
            return;
        }
        arr[idx++] = num;
    }
    void ShowAllData()
    {
        for(int i = 0; i <idx; i++)
            cout << arr[i] << ' ';
        cout << endl;
    }
    void SortData(const SortRule& functor) // 어떤 펑터를 사용(오름차순, 내림차순)할 지 선택해서 인자로 전달
    {
        // bubble 정렬 : 서로 인접한 두 요소를 비교하여 정렬하는 것
        for(int i = 0; i < (idx-1); i++) // 완료된 수, 버블 정렬은 맨 끝 부분부터 정렬이 완료된다.
        {
            for(int j =0; j < (idx-1); j++) // 비교 진행
            {
                if(functor(arr[j], arr[j+1]))
                {
                    int temp = arr[j];
                    arr[j] = arr[j+1];
                    arr[j+1] = temp;
                }
            }
        }
    }
};

int main(void)
{
    DataStorage storage(5);
    storage.AddData(40);
    storage.AddData(30);
    storage.AddData(50);
    storage.AddData(20);
    storage.AddData(10);

    storage.SortData(AscendingSort());
    storage.ShowAllData();

    storage.SortData(DescendingSort());
    storage.ShowAllData();
    return 0;
}