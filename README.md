mc-rtc-unity
==

Sample plugin + project to display mc_rtc robots in Unity

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

- Copy `Project/Assets/McRtc/` folder into your own asset folder
