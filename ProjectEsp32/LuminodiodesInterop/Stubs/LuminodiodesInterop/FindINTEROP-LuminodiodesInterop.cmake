#
# Copyright (c) .NET Foundation and Contributors
# See LICENSE file in the project root for full license information.
#

########################################################################################
# make sure that a valid path is set bellow                                            #
# this is an Interop module so this file should be placed in the CMakes module folder  #
# usually CMake\Modules                                                                #
########################################################################################

# native code directory
set(BASE_PATH_FOR_THIS_MODULE "${BASE_PATH_FOR_CLASS_LIBRARIES_MODULES}/LuminodiodesInterop")


# set include directories
list(APPEND LuminodiodesInterop_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/CLR/Core)
list(APPEND LuminodiodesInterop_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/CLR/Include)
list(APPEND LuminodiodesInterop_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/HAL/Include)
list(APPEND LuminodiodesInterop_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/PAL/Include)
list(APPEND LuminodiodesInterop_INCLUDE_DIRS ${BASE_PATH_FOR_THIS_MODULE})


# source files
set(LuminodiodesInterop_SRCS

    LuminodiodesInterop.cpp


    LuminodiodesInterop_LuminodiodesInterop_LuminodiodesInterop_mshl.cpp
    LuminodiodesInterop_LuminodiodesInterop_LuminodiodesInterop.cpp

)

foreach(SRC_FILE ${LuminodiodesInterop_SRCS})

    set(LuminodiodesInterop_SRC_FILE SRC_FILE-NOTFOUND)

    find_file(LuminodiodesInterop_SRC_FILE ${SRC_FILE}
        PATHS
	        ${BASE_PATH_FOR_THIS_MODULE}
	        ${TARGET_BASE_LOCATION}
            ${PROJECT_SOURCE_DIR}/src/LuminodiodesInterop

	    CMAKE_FIND_ROOT_PATH_BOTH
    )

    if (BUILD_VERBOSE)
        message("${SRC_FILE} >> ${LuminodiodesInterop_SRC_FILE}")
    endif()

    list(APPEND LuminodiodesInterop_SOURCES ${LuminodiodesInterop_SRC_FILE})

endforeach()

include(FindPackageHandleStandardArgs)

FIND_PACKAGE_HANDLE_STANDARD_ARGS(LuminodiodesInterop DEFAULT_MSG LuminodiodesInterop_INCLUDE_DIRS LuminodiodesInterop_SOURCES)
