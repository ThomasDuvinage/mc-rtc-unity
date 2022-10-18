#include <chrono>
#include <iostream>
#include <thread>

extern "C"
{

extern void CreateClient(const char *);

using on_robot_callback_t = void (*)(const char * id);
extern void OnRobot(on_robot_callback_t robot);

extern void UpdateClient();

extern void StopClient();

}

void OnRobotCb(const char * id)
{
  std::cout << "::OnRobot " << id << "\n";
}

int main()
{
  CreateClient("localhost");
  OnRobot(OnRobotCb);
  while(true)
  {
    UpdateClient();
    std::this_thread::sleep_for(std::chrono::milliseconds(500));
  }
  StopClient();
  return 0;
}
