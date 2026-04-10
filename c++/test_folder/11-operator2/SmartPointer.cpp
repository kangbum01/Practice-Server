#include <iostream>
using namespace std;

class Point
{
private:
    int xpos, ypos;
public:
    Point(int x = 0, int y = 0) : xpos(x), ypos(y)
    {
        cout << "Create Point Obj" << endl;
    }
    ~Point()
    {
        cout << "Delete Point Obj" << endl;
    }
    void SetPos(int x, int y)
    {
        xpos = x;
        ypos = y;
    }
    friend ostream& operator<<(ostream& os, const Point& pos);
};

ostream& operator<<(ostream& os, const Point& pos)
{
    os<<'[' << pos.xpos <<", " << pos.ypos << ']' << endl;
    return os;
}

class SmartPtr
{
private:
    Point * posptr;
public:
    SmartPtr(Point * ptr) : posptr(ptr){}

    // 스마트 포인터는 포인터 처럼 작동하는 객체이기 때문에 operator*과 ->에 대한 정의가 가장 중요하다
    Point& operator*() const
    {
        return *posptr;
    }
    Point* operator-> () const
    {
        return posptr;
    }
    ~SmartPtr()
    {
        delete posptr;
    }
};

int main(void)
{
    // Point 객체를 생성하면서 동시에 스마트 포인터 SmartPtr 객체가 이를 가리키게끔 하고 있다.
    // 즉 sptr1,sptr2,sptr3은 Point 객체를 가리키는 포인터처럼 동작한다.
    SmartPtr sptr1(new Point(1,2));
    SmartPtr sptr2(new Point(2,3));
    SmartPtr sptr3(new Point(4,5));
    cout<<*sptr1;
    cout<<*sptr2;
    cout<<*sptr3;

    sptr1->SetPos(10,20);
    sptr2->SetPos(30,40);
    sptr3->SetPos(50,60);
    cout<<*sptr1;
    cout<<*sptr2;
    cout<<*sptr3;
    return 0;
}

//스마트포인터의 장점은 객체의 소멸을 자동으로 처리해준다는 점에 있다. 위의 객체를 보면 SmartPtr이 소멸할 때 배열 또한 delete 하는 것을 알 수 있다.