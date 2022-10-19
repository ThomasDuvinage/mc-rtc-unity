cmake_minimum_required(VERSION 3.1)

set(CMAKE_CXX_STANDARD 17)
project(McRtcPlugin LANGUAGES CXX VERSION 1.0.0)

find_package(mc_rtc REQUIRED)

if(NOT DEFINED UNITY_PROJECT_DIR)
  set(UNITY_PROJECT_DIR "${PROJECT_SOURCE_DIR}/../Project/")
endif()
if(NOT EXISTS "${UNITY_PROJECT_DIR}")
  message(FATAL_ERROR "Provided Unity project folder (${UNITY_PROJECT_DIR}) does not exists, please set UNITY_PROJECT_DIR correctly.")
endif()

# C++ header generation
set(PREPROCESSOR "#pragma once
#define MC_RTC_CPP 1
#define MC_RTC_CSHARP 0
#include <SpaceVecAlg/SpaceVecAlg>
#include <SpaceVecAlg/Conversions.h>")
set(END_STRUCT ";")
set(PUBLIC_ATTRIBUTE "")
configure_file(src/types.in.h "${PROJECT_BINARY_DIR}/include/types.h")

# C# header generation
set(PREPROCESSOR "#define MC_RTC_CSHARP
using UnityEngine;")
set(END_STRUCT "")
set(PUBLIC_ATTRIBUTE "public")
configure_file(src/types.in.h "${UNITY_PROJECT_DIR}/Assets/McRtc/Types.cs")

add_library(${PROJECT_NAME} SHARED src/plugin.cpp)
set_target_properties(${PROJECT_NAME} PROPERTIES
  ARCHIVE_OUTPUT_DIRECTORY "${UNITY_PROJECT_DIR}/Assets/Plugins"
  LIBRARY_OUTPUT_DIRECTORY "${UNITY_PROJECT_DIR}/Assets/Plugins"
  RUNTIME_OUTPUT_DIRECTORY "${UNITY_PROJECT_DIR}/Assets/Plugins"
  PREFIX "")
target_link_libraries(${PROJECT_NAME} PUBLIC mc_rtc::mc_control_client)
target_include_directories(${PROJECT_NAME} PUBLIC "${PROJECT_BINARY_DIR}/include")
if(TARGET mc_rtc::mc_rtc_ros)
  target_compile_definitions(${PROJECT_NAME} PRIVATE MC_RTC_HAS_ROS_SUPPORT)
  target_link_libraries(${PROJECT_NAME} PRIVATE mc_rtc::mc_rtc_ros)
endif()

add_executable(test_plugin src/test_plugin.cpp)
target_link_libraries(test_plugin PUBLIC ${PROJECT_NAME})
set_target_properties(test_plugin PROPERTIES RUNTIME_OUTPUT_DIRECTORY "${UNITY_PROJECT_DIR}/Assets/Plugins")
