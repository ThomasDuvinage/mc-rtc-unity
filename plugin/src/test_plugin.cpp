#include <chrono>
#include <iostream>
#include <thread>

#include <mc_rtc/logging.h>

#include "types.h"

extern "C"
{

  extern void CreateClient(const char *);

  using on_robot_callback_t = void (*)(const char * id);
  extern void OnRobot(on_robot_callback_t robot);

  using on_remove_element_callback_t = void (*)(const char * id, const char * type);
  extern void OnRemoveElement(on_remove_element_callback_t callback);

  using on_transform_callback_t = void (*)(const char * id, bool ro, McRtc::PTransform pt);
  extern void OnTransform(on_transform_callback_t callback);

  extern void UpdateClient();

  extern void StopClient();
}

void OnRobotCb(const char * id)
{
}

void OnRemoveElementCb(const char * id, const char * type)
{
  mc_rtc::log::info("OnRemoveElement {} type: {}", id, type);
}

void OnTransformCb(const char * id, bool ro, McRtc::PTransform pt)
{
}

int main()
{
  CreateClient("localhost");
  OnRobot(OnRobotCb);
  OnTransform(OnTransformCb);
  OnRemoveElement(OnRemoveElementCb);
  while(true)
  {
    UpdateClient();
    std::this_thread::sleep_for(std::chrono::milliseconds(50));
  }
  StopClient();
  return 0;
}
