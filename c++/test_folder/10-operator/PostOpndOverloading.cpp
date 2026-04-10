#include <iostream>
using namespace std;

class Point
{
private:
    int xpos, ypos;
public:
    Point(int x=0, int y=0):xpos(x), ypos(y) {}
    void ShowPosition() const
    {
        cout << "[" << xpos << ", " << ypos << "]" << endl;
    }
    Point& operator++()
    {
        xpos +=1;
        ypos +=1;
        return *this;
    }
    const Point& operator++(int)
    {
        const Point retobj(xpos, ypos);         // const Point retobj(*this)
        xpos +=1;
        ypos +=1;
        return retobj;
    }
    friend Point& operator--(Point &ref);
    friend const Point operator--(Point &ref, int);
};

Point& operator--(Point &ref)
{
    ref.xpos -= 1;
    ref.ypos -= 1;
    return ref;
}

const Point operator--(Point &ref, int)
{
    const Point retobj(ref); // 임시 객체 생성
    ref.xpos -=1; 
    ref.ypos -=1;
    return retobj; // 임시 객체 반환
}

int main(void)
{
    Point pos(3,5);
    Point cpy;
    cpy = pos--;
    cpy.ShowPosition();
    pos.ShowPosition();

    cpy=pos++;
    cpy.ShowPosition();
    pos.ShowPosition();
    return 0;
}

// 후위 연산자들을 const 객체로 만드는 이유
// 함수의 반환으로 인해서 생성된 임시 객체들을 상수화 시켜서 내부에 저장된 값의 변경을 막겠다는 의미
// 후위 연산자들의 경우 복사 생성자들을 통해 객체를 새로 만든다. 그렇게 만들어진 객체의 값은 변경되어서는 아니되기 때문에
// const객채로 생성하였다.
// 때문에 이를 통해 (x++)++와 같은 연산을 막을 수 있다.
// (x++)++ => (Point객체의 const형 임시 객체).operator++()
// operator++는 const로 정의하지 않았기 때문에 컴파일 오류가 발생한다.
// ++x => 증가 후 자기 자신을 반환
// x++ => 증가 전 값을 임시로 반환
