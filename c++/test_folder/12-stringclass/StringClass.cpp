/*
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
*/
#include "StringClass.h"
String::String()
{
    len = 0;
    str = NULL;
}
String::String(const char *s)
{
    len = strlen(s)+1;
    str = new char[len];
    strcpy(str,s);
}
String::String(const String& s)
{
    len=s.len;
    str = new char[len];
    strcpy(str,s.str);
}

String::~String()
{
    if(str!=NULL)
        delete []str;
}

String& String::operator= (const String& s)
{
    if(str != NULL)
        delete []str;
    len = s.len;
    str = new char[len];
    strcpy(str, s.str);
    return *this;
}

String& String::operator+= (const String& s)
{
    len+=(s.len-1);
}