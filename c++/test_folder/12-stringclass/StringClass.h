#ifndef __StringClass_H__
#define __StringClass_H__
#include <iostream>
#include <cstring>
using namespace std;

class String
{
private:
    int len;
    char * str;
public:
    String();
    String(const char * s);
    String(const String& s);
    ~String();
    String& operator= (const String& s);
    String& operator+= (const String& s);
    bool operator== (const String& s);
    String operator+ (const String& s);

    friend ostream& operator<< (ostream& os, const String& s);
    friend ostream& operator>> (istream& is, const String& s);
};
#endif