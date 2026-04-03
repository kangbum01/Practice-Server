#include <iostream>
using namespace std;

class Person
{
public:
    void Sleep()
    {
        cout << "Sleep" << endl;
    }
};

class Student : public Person
{
public:
    void Study() 
    {
        cout << "Study" << endl;
    }
};

class PartimeStudent : public Student
{
public:
    void WorkO()
    {
        cout << "Work" <<endl;
    }
};

int main(void)
{
    Person * ptr1 = new Student();
    Person * ptr2 = new PartimeStudent();
    Student * ptr3 = new PartimeStudent();
    ptr1 -> Sleep();
    ptr2 -> Sleep();
    ptr3 -> Study();
    delete ptr1, ptr2, ptr3;
    return 0;
}
