//-----------------------------------------------------------------------------
//
//                   ** WARNING! ** 
//    This file was generated automatically by a tool.
//    Re-running the tool will overwrite this file.
//    You should copy this file to a custom location
//    before adding any customization in the copy to
//    prevent loss of your changes when the tool is
//    re-run.
//
//-----------------------------------------------------------------------------

#include "LuminodiodesInterop.h"
#include "LuminodiodesInterop_LuminodiodesInterop_LuminodiodesInterop.h"
#include "CPU_GPIO_decl.h"

using namespace LuminodiodesInterop::LuminodiodesInterop;


void LuminodiodesInterop::OneWireSendLuminodiodes(CLR_RT_TypedArray_UINT8 param0, unsigned int param1, HRESULT& hr)
{

    (void)param0;
    (void)param1;
    (void)hr;


    ////////////////////////////////
    // implementation starts here //
    for (int i = 0; i < 100; ++i) {
        CPU_GPIO_SetPinState(27, GpioPinValue.High);
        for (int j = 0; j < 100; ++j) {}
        CPU_GPIO_SetPinState(27, GpioPinValue.High);
    }
    // implementation ends here   //
    ////////////////////////////////


}
