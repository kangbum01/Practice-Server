#include <iostream>
#include <cstdlib>
using namespace std;

class BoundCHeckIntArray
{
private:
    int * arr;
    int arrlen;
    BoundCHeckIntArray(const BoundCHeckIntArray& arr) {}
    BoundCHeckIntArray& operator=(const BoundCHeckIntArray& arr) { }
public:
    BoundCHeckIntArray(int len) : arrlen(len) { arr=new int[len]; }
    int& operator[] (int idx)
    {
        if(idx<0 || idx >= arrlen)
        {
            cout << "Array index out of bound excpetion"<< endl;
            exit(1);
        }
        return arr[idx];
    }
    int operator[] (int idx) const
    {
        if(idx < 0 || idx >=arrlen)
        {
            cout << "Array index out of bound exception"<< endl;
            exit(1);
        }
        return arr[idx];
    }
    int GetArrLen() const { return arrlen; }
    ~BoundCHeckIntArray() {delete []arr;}
};

void ShowAllData(const BoundCHeckIntArray& ref)
{
    int len = ref.GetArrLen();
    for(int idx=0; idx<len; idx++)
        cout<<ref[idx]<<endl;
}

int main(void)
{
    BoundCHeckIntArray arr(5);
    for(int i =0; i < 5; i++)
    {
        arr[i] = (i+1)*11;
    }
    ShowAllData();
    return 0;
}