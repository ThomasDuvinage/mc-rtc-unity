mc-rtc-unity
==

Sample plugin + project to display mc_rtc robots in Unity

Requirements 
--
Here are the latest tool versions tested to compile the project.

|  mc_rtc | MSVC | Unity |
| -------- | ------- | ------- | 
| [v2.12.0](https://github.com/jrl-umi3218/mc_rtc/releases/tag/v2.12.0)  | 2022    | 2021.3.6f1 |

Usage
--

- Build the plugin

```bash
# From this repository's folder
cmake -B plugin/build -S plugin -DCMAKE_BUILD_TYPE=RelWithDebInfo
cmake --build plugin/build
```

- Open the `Project` folder in Unity hub

- If it does not open an `McRtcGUI` window, find the `McRtc` menu and click the `GUI` entry

- Start a local controller (e.g. `rosrun mc_rtc_ticker mc_rtc_ticker`) and you should robots appear in Unity

Building the plugin in your own project
--

- Build the plugin and provide your Unity project folder

```bash
cmake -B plugin/build -S plugin -DCMAKE_BUILD_TYPE=RelWithDebInfo -DUNITY_PROJECT_DIR=$HOME/MyProject
```

Be careful here, if you decide to build the plugin with MSVC make sure that the build type is correctly set to `RelWithDebInfo`. Otherwise, the plugin may fail to connect with the remotly running controller. 

- Copy `Project/Assets/McRtc/` folder into your own asset folder

Troubleshooting 
--

In order to test if the plugin, you can :
- Start a local controller (e.g. `rosrun mc_rtc_ticker mc_rtc_ticker`)
- Run test_plugin from `Project/Assets/Plugins`. (You may have to edit the ip address to match your)



