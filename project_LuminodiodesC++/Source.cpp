// copyright kruglovvs kruglov.valentine@gmail.com

#include "Header.hpp"

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <inttypes.h>

Luminodiodes::Luminodiodes(const int pin, const int count) : _pin(pin), _count(count) {}
void Luminodiodes::Send(const char* colors) {

}
void Luminodiodes::SetZero() {
	this->Send(new char[this->_count]);
}