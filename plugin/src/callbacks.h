DEFINE_CALLBACK("Called when a robot appears in the scene", on_robot_callback, OnRobot, void (*)(const char * id), "id")
DEFINE_CALLBACK("Called to update a robot's body position",
                on_robot_body_callback,
                OnRobotBody,
                void (*)(const char * id, const char * body, McRtc::PTransform X_0_body),
                "id",
                "body",
                "X_0_body")
DEFINE_CALLBACK("Called to update a robot's body's mesh position",
                on_robot_mesh_callback,
                OnRobotMesh,
                void (*)(const char * id,
                         const char * body,
                         const char * name,
                         const char * path,
                         float scale,
                         McRtc::PTransform X_body_visual),
                "id",
                "body",
                "name",
                "path",
                "scale",
                "X_body_visual")
DEFINE_CALLBACK("Called when a trajectory made of 3D points is sent",
                on_trajectory_vector3d_callback,
                OnTrajectoryVector3d,
                void (*)(const char * id, float * data, size_t npoints),
                "id",
                "data",
                "npoints")
DEFINE_CALLBACK("Called when a transform is available",
                on_transform_callback,
                OnTransform,
                void (*)(const char * id, bool ro, McRtc::PTransform pt),
                "id",
                "ro",
                "pt")
DEFINE_CALLBACK("Called when any element is removed",
                on_remove_element_callback,
                OnRemoveElement,
                void (*)(const char * id, const char * type),
                "id",
                "type")

DEFINE_CALLBACK("Allows mc_rtc to print messages in Unity",
                debug_log_callback,
                DebugLogCallback,
                void (*)(const char * message),
                "message");

DEFINE_CALLBACK("Called when a checkbox is available",
                on_checkbox_callback,
                OnCheckbox,
                void (*)(const char * id, bool state),
                "id",
                "state")

DEFINE_CALLBACK("Called when an array input is available",
                on_array_input_callback,
                OnArrayInput,
                void (*)(const char * id, McRtc::StringArray labels, McRtc::FloatArray data),
                "id",
                "labels",
                "data")