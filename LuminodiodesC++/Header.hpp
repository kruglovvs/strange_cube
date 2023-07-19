// copyright kruglovvs kruglov.valentine@gmail.com

#ifndef HEADER_HPP
#define HEADER_HPP

extern "C" {
	__declspec(dllexport) class Luminodiodes {
	private:
		int _pin;
		int _count;
	public:
		__declspec(dllexport) __stdcall Luminodiodes(const int pin, const int count);
		__declspec(dllexport) void __stdcall Send(const char* colors);
		__declspec(dllexport) void __stdcall SetZero();
	};
}

#endif