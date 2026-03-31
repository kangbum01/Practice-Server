#include <iostream>
#include <cstring>
using namespace std;


class Person
{
private:
    char * name;
    int age;
public:
    Person(const char * myname, int myage)
    {
        int len = strlen(myname) + 1;
        name = new char[len];
        strcpy(name, myname);
        age = myage;
    }
    // 깊은 복사 적용
    Person(const Person &copy):name(copy.name),age(copy.age)
    {
        name = new char[strlen(copy.name)+1];
        strcpy(name, copy.name);
    }
    void ShowPersonInfo() const
    {
        cout << "name: " << name << endl;
        cout << "Age: " << age << endl;
    }
    ~Person()
    {
        delete []name;
        cout << "called destructor! " << endl;
    }
};

int main(void)
{
    Person man1("Lee dong woo", 29);
    Person man2 = man1;
    man1.ShowPersonInfo();
    man2.ShowPersonInfo();
    return 0;
}