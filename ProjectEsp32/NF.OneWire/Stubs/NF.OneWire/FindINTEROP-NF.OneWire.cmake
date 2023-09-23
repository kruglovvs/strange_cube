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
set(BASE_PATH_FOR_THIS_MODULE ${PROJECT_SOURCE_DIR}/InteropAssemblies/NF.OneWire)


# set include directories
list(APPEND NF.OneWire_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/CLR/Core)
list(APPEND NF.OneWire_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/CLR/Include)
list(APPEND NF.OneWire_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/HAL/Include)
list(APPEND NF.OneWire_INCLUDE_DIRS ${PROJECT_SOURCE_DIR}/src/PAL/Include)
list(APPEND NF.OneWire_INCLUDE_DIRS ${BASE_PATH_FOR_THIS_MODULE})


# source files
set(NF.OneWire_SRCS

    NF_OneWire.cpp


    NF_OneWire_NF_OneWire_YF923_mshl.cpp
    NF_OneWire_NF_OneWire_YF923.cpp

)

foreach(SRC_FILE ${NF.OneWire_SRCS})

    set(NF.OneWire_SRC_FILE SRC_FILE-NOTFOUND)

    find_file(NF.OneWire_SRC_FILE ${SRC_FILE}
        PATHS
	        ${BASE_PATH_FOR_THIS_MODULE}
	        ${TARGET_BASE_LOCATION}
            ${PROJECT_SOURCE_DIR}/src/NF.OneWire

	    CMAKE_FIND_ROOT_PATH_BOTH
    )

    if (BUILD_VERBOSE)
        message("${SRC_FILE} >> ${NF.OneWire_SRC_FILE}")
    endif()

    list(APPEND NF.OneWire_SOURCES ${NF.OneWire_SRC_FILE})

endforeach()

include(FindPackageHandleStandardArgs)

FIND_PACKAGE_HANDLE_STANDARD_ARGS(NF.OneWire DEFAULT_MSG NF.OneWire_INCLUDE_DIRS NF.OneWire_SOURCES)
