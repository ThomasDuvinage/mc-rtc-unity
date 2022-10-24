DEFINE_CALLBACK(on_robot_callback, OnRobot, void (*)(const char * id), "id")
DEFINE_CALLBACK(on_robot_body_callback,
                OnRobotBody,
                void (*)(const char * id, const char * body, McRtc::PTransform X_0_body),
                "id",
                "body",
                "X_0_body")
DEFINE_CALLBACK(on_robot_mesh_callback,
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
DEFINE_CALLBACK(on_trajectory_vector3d_callback,
                OnTrajectoryVector3d,
                void (*)(const char * id, float * data, size_t npoints),
                "id",
                "data",
                "npoints")
DEFINE_CALLBACK(on_transform_callback,
                OnTransform,
                void (*)(const char * id, bool ro, McRtc::PTransform pt),
                "id",
                "ro",
                "pt")
DEFINE_CALLBACK(on_remove_element_callback, OnRemoveElement, void (*)(const char * id, const char * type), "id", "type")

DEFINE_CALLBACK(debug_log_callback, DebugLogCallback, void (*)(const char * message), "message");